// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetDisplayOffset")]
    public class SetDisplayOffset : Shared1x06Commands.SetDisplayOffset
    {
        /// <inheritdoc cref="Shared1x06Commands.SetDisplayOffset.SetDisplayOffset(byte)"/>
        public SetDisplayOffset(byte displayOffset = 0x00)
            : base(displayOffset)
        {
        }
    }
}
