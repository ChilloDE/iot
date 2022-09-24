// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Sh1106Commands
{
    /// <summary>
    /// Represents SetNormalDisplay command
    /// </summary>
    public class SetPumpVoltage : ISh1106Command
    {
        /// <summary>
        /// Specifies output voltage (VPP) of the internal charger pump.
        /// </summary>
        public SetPumpVoltage(VoltageLevel level = VoltageLevel.Vpp8_0)
        {
            if (level > VoltageLevel.Vpp9_0)
            {
                throw new ArgumentOutOfRangeException(nameof(level), "Value should be one of the predefined voltage levels");
            }

            Level = level;
        }

        /// <summary>
        /// The value that represents the command.
        /// </summary>
        public byte Id => (byte)(0x30 | (byte)Level);

        /// <summary>
        /// The output voltage (VPP) that the internal charger pump should be set to.
        /// </summary>
        public VoltageLevel Level { get; }

        /// <summary>
        /// Gets the bytes that represent the command.
        /// </summary>
        /// <returns>The bytes that represent the command.</returns>
        public byte[] GetBytes()
        {
            return new byte[] { Id };
        }

        /// <summary>
        /// Valid values for the voltage level.
        /// </summary>
        public enum VoltageLevel : byte
        {
            /// <summary>
            /// Specifies output voltage (VPP) of the internal charger pump to be 6.4 V
            /// </summary>
            Vpp6_4 = 0,

            /// <summary>
            /// Specifies output voltage (VPP) of the internal charger pump to be 7.4 V
            /// </summary>
            Vpp7_4 = 1,

            /// <summary>
            /// Specifies output voltage (VPP) of the internal charger pump to be 8 V
            /// </summary>
            Vpp8_0 = 2,

            /// <summary>
            /// Specifies output voltage (VPP) of the internal charger pump to be 9 V
            /// </summary>
            Vpp9_0 = 3,
        }
    }
}
