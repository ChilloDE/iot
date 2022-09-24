// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <summary>
    /// Represents SetComOutputScanDirection command
    /// </summary>
    [Obsolete("Replaced by Sxx1x06Commands.SetComOutputScanDirection")]
    public class SetComOutputScanDirection : Shared1x06Commands.SetComOutputScanDirection
    {
        /// <inheritdoc cref="Shared1x06Commands.SetComOutputScanDirection.SetComOutputScanDirection(bool)"/>
        public SetComOutputScanDirection(bool normalMode = true)
            : base(normalMode)
        {
        }
    }
}
