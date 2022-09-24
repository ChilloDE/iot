// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Iot.Device.Ssd13xx.Commands.Sh1106Commands
{
    /// <summary>
    /// The panel display must be off while issuing this command!
    /// Represents the Set DC-DC OFF/ON command.
    /// </summary>
    public class SetDcDcOnOff : ISh1106Command
    {
        private const byte _dcDcOnValue = 0x8B;
        private const byte _dcDcOffValue = 0x8A;

        /// <summary>
        /// The panel display must be off while issuing this command!
        /// This command is to control the DC-DC voltage converter.
        /// The converter will be turned on by issuing this command then display ON command.
        /// </summary>
        /// <param name="dcDcOn">When <c>true</c> the DC-DC voltage converter is turned on, otherwise it's turned off.</param>
        public SetDcDcOnOff(bool dcDcOn = true)
        {
            DcDcOn = dcDcOn;
        }

        /// <inheritdoc />
        public byte Id => 0xAD;

        /// <summary>
        /// When <c>true</c> the DC-DC voltage converter is turned on, otherwise it's turned off.
        /// </summary>
        public bool DcDcOn { get; }

        /// <inheritdoc />
        public byte[] GetBytes()
        {
            return new[] { Id, DcDcOn ? _dcDcOnValue : _dcDcOffValue };
        }
    }
}
