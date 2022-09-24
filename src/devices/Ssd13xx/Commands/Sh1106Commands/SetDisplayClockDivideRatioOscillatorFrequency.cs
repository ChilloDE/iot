// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Sh1106Commands
{
    /// <inheritdoc />
    public class SetDisplayClockDivideRatioOscillatorFrequency : Shared1x06Commands.SetDisplayClockDivideRatioOscillatorFrequency
    {
        /// <inheritdoc cref="Shared1x06Commands.SetDisplayClockDivideRatioOscillatorFrequency.SetDisplayClockDivideRatioOscillatorFrequency(byte, byte)"/>
        public SetDisplayClockDivideRatioOscillatorFrequency(byte displayClockDivideRatio = 0x00, byte oscillatorFrequency = 0x05)
            : base(displayClockDivideRatio, oscillatorFrequency)
        {
        }
    }
}
