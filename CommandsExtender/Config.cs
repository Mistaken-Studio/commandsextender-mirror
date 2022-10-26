// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CommandsExtender
{
    internal sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
    }
}
