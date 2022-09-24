﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Iot.Device.Ssd13xx.Commands.Ssd1306Commands
{
    /// <inheritdoc />
    [Obsolete("Replaced by Sxx1x06Commands.SetEntireDisplayOn")]
    public class EntireDisplayOn : Shared1x06Commands.EntireDisplayOn
    {
        /// <inheritdoc cref="Shared1x06Commands.EntireDisplayOn.EntireDisplayOn(bool)"/>
        public EntireDisplayOn(bool entireDisplay)
            : base(entireDisplay)
        {
        }
    }
}
