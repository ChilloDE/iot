// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetPreChargePeriod")]
    public class SetPreChargePeriod : Shared1x06Commands.SetPreChargePeriod
    {
        /// <inheritdoc cref="Shared1x06Commands.SetPreChargePeriod.SetPreChargePeriod(byte, byte)"/>
        public SetPreChargePeriod(byte phase1Period = 0x02, byte phase2Period = 0x02)
            : base(phase1Period, phase2Period)
        {
        }
    }
}
