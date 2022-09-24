// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetDisplayStartLine")]
    public class SetDisplayStartLine : Shared1x06Commands.SetDisplayStartLine
    {
        /// <inheritdoc cref="Shared1x06Commands.SetDisplayStartLine.SetDisplayStartLine(byte)"/>
        public SetDisplayStartLine(byte displayStartLine = 0x00)
            : base(displayStartLine)
        {
        }
    }
}
