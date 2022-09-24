// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    public class SetPageStartAddressForPageAddressingMode : Shared1x06Commands.SetPageStartAddressForPageAddressingMode
    {
        /// <inheritdoc cref="Shared1x06Commands.SetPageStartAddressForPageAddressingMode.SetPageStartAddressForPageAddressingMode(Shared1x06Commands.PageAddress)"/>
        public SetPageStartAddressForPageAddressingMode(PageAddress startAddress = PageAddress.Page0)
            : base((Shared1x06Commands.PageAddress)startAddress)
        {
        }
    }
}
