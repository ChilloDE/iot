// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetComPinsHardwareConfiguration")]
    public class SetComPinsHardwareConfiguration : Shared1x06Commands.SetComPinsHardwareConfiguration
    {
        /// <inheritdoc cref="Shared1x06Commands.SetComPinsHardwareConfiguration.SetComPinsHardwareConfiguration(bool, bool)"/>
        public SetComPinsHardwareConfiguration(bool alternativeComPinConfiguration = true, bool enableLeftRightRemap = false)
            : base(alternativeComPinConfiguration, enableLeftRightRemap)
        {
        }
    }
}
