﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Iot.Device.Nmea0183.Sentences;

namespace Iot.Device.Nmea0183
{
    /// <summary>
    /// This server distributes all incoming messages via UDP. The advantage is that clients do not need to
    /// know the IP of the server, which is useful if DHCP keeps reassigning addresses.
    /// </summary>
    public class NmeaUdpServer : NmeaSinkAndSource
    {
        private readonly int _port;
        private UdpClient? _server;
        private NmeaParser? _parser;
        private UdpClientStream? _clientStream;

        /// <summary>
        /// Create an UDP server with the given name on the default port 10110
        /// </summary>
        /// <param name="name">The network source name</param>
        public NmeaUdpServer(string name)
        : this(name, 10110)
        {
        }

        /// <summary>
        /// Create an UDP server with the given name on the given port
        /// </summary>
        /// <param name="name">The network source name</param>
        /// <param name="port">The network port to use</param>
        public NmeaUdpServer(string name, int port)
        : base(name)
        {
            _port = port;
        }

        /// <summary>
        /// Get the default IP address to bind to
        /// </summary>
        /// <returns></returns>
        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }

            return IPAddress.Loopback;
        }

        /// <inheritdoc />
        public override void StartDecode()
        {
            if (_server != null)
            {
                throw new InvalidOperationException("Server already started");
            }

            _server = new UdpClient(_port);

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // This is unsupported on MacOS (https://github.com/dotnet/runtime/issues/27653), but this shouldn't
                // hurt, since true is the default.
                try
                {
                    _server.DontFragment = true;
                }
                catch (Exception x) when (x is NotSupportedException || x is SocketException)
                {
                    // Ignore
                }
            }
            else
            {
                // On MacOs, instead we need to set up a timeout, or we end in a deadlock when terminating
                _server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 1000);
            }

            _clientStream = new UdpClientStream(_server, _port, this);
            _parser = new NmeaParser($"{InterfaceName} (Port {_port})", _clientStream, _clientStream);
            _parser.OnNewSequence += OnSentenceReceivedFromClient;
            _parser.OnParserError += ParserOnParserError;
            _parser.StartDecode();
        }

        private void ParserOnParserError(NmeaSinkAndSource source, string message, NmeaError errorCode)
        {
            FireOnParserError(message, errorCode);
        }

        private void OnSentenceReceivedFromClient(NmeaSinkAndSource source, NmeaSentence sentence)
        {
            DispatchSentenceEvents(sentence);
        }

        /// <summary>
        /// Sends the sentence to all our clients.
        /// If it is needed to make distinctions for what needs to be sent to which client, create
        /// multiple server instances. This will allow for proper filtering.
        /// </summary>
        /// <param name="source">The original source of the message, used i.e. for logging</param>
        /// <param name="sentence">The sentence to send</param>
        public override void SendSentence(NmeaSinkAndSource source, NmeaSentence sentence)
        {
            if (_parser == null)
            {
                return;
            }

            try
            {
                _parser.SendSentence(source, sentence);
            }
            catch (IOException x)
            {
                FireOnParserError($"Error sending message to {_parser.InterfaceName}: {x.Message}",
                    NmeaError.PortClosed);
            }
        }

        /// <inheritdoc />
        public override void StopDecode()
        {
            if (_parser != null)
            {
                _parser.StopDecode();
                _parser.Dispose();
            }

            if (_server != null && _clientStream != null)
            {
                _server.Dispose();
                _clientStream.Dispose();
                _server = null;
                _clientStream = null;
            }

            _parser = null;
        }

        private sealed class UdpClientStream : Stream, IDisposable
        {
            private readonly UdpClient _client;
            private readonly int _port;
            private readonly NmeaUdpServer _parent;
            private readonly Queue<byte> _data;

            private object _disposalLock = new object();

            private Stopwatch _lastUnsuccessfulSend;
            private Dictionary<IPAddress, bool> _knownSenders;
            private CancellationTokenSource _cancellationSource;
            private CancellationToken _cancellationToken;

            public UdpClientStream(UdpClient client, int port, NmeaUdpServer parent)
            {
                _client = client;
                _port = port;
                _parent = parent;
                _data = new Queue<byte>();
                _knownSenders = new();
                _lastUnsuccessfulSend = new Stopwatch();
                _cancellationSource = new CancellationTokenSource();
                _cancellationToken = _cancellationSource.Token;
            }

            public override void Flush()
            {
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                int bytesRemaining = count;
                int bytesAdded = 0;
                while (_data.Count > 0 && bytesRemaining > 0)
                {
                    buffer[offset++] = _data.Dequeue();
                    bytesAdded++;
                    bytesRemaining--;
                }

                if (bytesAdded > 0)
                {
                    return bytesAdded;
                }

                IPEndPoint pt;
                byte[]? datagram = null;
                bool isself;
                while (!_cancellationSource.IsCancellationRequested)
                {
                    pt = new IPEndPoint(IPAddress.Any, _port);
                    try
                    {
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
#if NET6_O_OR_GREATER
                            var result = _client.ReceiveAsync(_cancellationToken).GetAwaiter().GetResult();
                            datagram = result.Buffer;
#else
                            var result = _client.ReceiveAsync().GetAwaiter().GetResult();
                            datagram = result.Buffer;
#endif
                        }
                        else
                        {
                            datagram = _client.Receive(ref pt);
                        }
                    }
                    catch (SocketException x)
                    {
                        if (x.SocketErrorCode == SocketError.TimedOut)
                        {
                            continue;
                        }

                        _parent.FireOnParserError($"Udp server error: {x.Message}", NmeaError.PortClosed);
                        return 0;
                    }
                    catch (ObjectDisposedException x)
                    {
                        _parent.FireOnParserError($"Udp server error: {x.Message}", NmeaError.PortClosed);
                        return 0;
                    }

                    if (_knownSenders.TryGetValue(pt.Address, out isself))
                    {
                        if (isself)
                        {
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Check whether the given address is ours (new IPs can be added at runtime, if interfaces go up)
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    if (host.AddressList.Contains(pt.Address))
                    {
                        _knownSenders.Add(pt.Address, true);
                    }
                    else
                    {
                        _knownSenders.Add(pt.Address, false);
                    }
                }

                if (_cancellationSource.IsCancellationRequested || datagram == null)
                {
                    return 0;
                }

                // Does the whole message fit in the buffer?
                if (bytesRemaining >= datagram.Length)
                {
                    Array.Copy(datagram, 0, buffer, offset, datagram.Length);
                    return datagram.Length;
                }

                foreach (var b in datagram)
                {
                    _data.Enqueue(b);
                }

                // Shouldn't normally happen here
                if (_data.Count == 0)
                {
                    return 0;
                }

                // Recurse to execute the first part of this method
                return Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException("Cannot seek on a Udp Stream");
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException("Cannot set length");
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                if (_lastUnsuccessfulSend.IsRunning && _lastUnsuccessfulSend.Elapsed < TimeSpan.FromMinutes(1))
                {
                    return;
                }

                lock (_disposalLock)
                {
                    if (_client == null)
                    {
                        throw new ObjectDisposedException("Udp Server is disposed");
                    }

                    byte[] tempBuf = buffer;
                    if (offset != 0)
                    {
                        tempBuf = new byte[count];
                        Array.Copy(buffer, offset, tempBuf, 0, count);
                    }

                    try
                    {
                        IPEndPoint pt = new IPEndPoint(IPAddress.Broadcast, _port);
                        _client.Send(tempBuf, count, pt);
                        _lastUnsuccessfulSend.Stop();
                    }
                    catch (SocketException x)
                    {
                        // This is normal if no network connection is available.
                        _parent.FireOnParserError($"Udp server send error: {x.Message}", NmeaError.None);
                        _lastUnsuccessfulSend.Reset();
                        _lastUnsuccessfulSend.Start();
                    }
                }
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => true;
            public override long Length => 0;
            public override long Position { get; set; }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    lock (_disposalLock)
                    {
                        if (!_cancellationToken.IsCancellationRequested)
                        {
                            _cancellationSource.Cancel();
                        }

                        _client.Dispose();
                        _cancellationSource.Dispose();
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}
