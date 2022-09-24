// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    public class SetVcomhDeselectLevel : Shared1x06Commands.SetVcomhDeselectLevel
    {
        /// <inheritdoc cref="Shared1x06Commands.SetVcomhDeselectLevel.SetVcomhDeselectLevel(DeselectLevel)"/>
        public SetVcomhDeselectLevel(DeselectLevel level = DeselectLevel.Vcc0_77)
            : base(level)
        {
        }
    }
}
