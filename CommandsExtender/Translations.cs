// -----------------------------------------------------------------------
// <copyright file="Translations.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Interfaces;

namespace Mistaken.CommandsExtender
{
    internal class Translations : ITranslation
    {
        public string GettingHealingMessage { get; set; } = "<color=red><size=150%>You are being healed</size></color><br>Stand still for <color=yellow>{0}</color>s";

        public string HealingCancelledMessage { get; set; } = "<color=red><size=150%>Healing canceled</size></color>";

        public string HealingCancelledHealerMessage { get; set; } = "<color=red><size=150%>Healing canceled</size></color><br>Player <color=yellow>moved</color>";
    }
}
