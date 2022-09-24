// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Iot.Device.Ssd13xx.Commands.Shared1x06Commands
{
    /// <summary>
    /// Represents SetNormalDisplay command
    /// </summary>
    public class SetNormalDisplay : IShared1x06Command
    {
        /// <summary>
        /// This command sets the display to be normal / inverted.
        /// Displays a RAM data of 1 indicates an ON pixel.
        /// </summary>
        public SetNormalDisplay()
        {
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => (byte)0xA6;

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return new byte[] { Id };
        }
    }
}
