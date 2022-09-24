// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetSegmentReMap")]
    public class SetSegmentReMap : Shared1x06Commands.SetSegmentReMap
    {
        /// <summary>
        /// This command changes the mapping between the display data column address and the segment driver.
        /// It allows flexibility in OLED module design. This command only affects subsequent data input.
        /// Data already stored in GDDRAM will have no changes.
        /// </summary>
        /// <param name="columnAddress127">Column address 0 is mapped to SEG0 when FALSE.
        /// Column address 127 is mapped to SEG0 when TRUE.</param>
        public SetSegmentReMap(bool columnAddress127 = false)
            : base(columnAddress127)
        {
        }
    }
}
