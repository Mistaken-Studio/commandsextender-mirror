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

        public string HealingSuccessHealerMessage { get; set; } = "You have <color=green>healed</color> <color=yellow>{0}</color>";

        public string HealingSuccessHealedMessage { get; set; } = "You have been <color=green>healed</color> by <color=yellow>{0}</color>";

        public string SuicideEnter { get; set; } = "Shoot to kill yourself";

        public string SuicideExit { get; set; } = "You can now shoot without killing yourself";

        public string DeadMsg556x45 { get; set; } = "There is a hole in the head. It looks like 5.56x45 caliber";

        public string DeadMsg762x39 { get; set; } = "There is a hole in the head. It looks like 7.62x39 caliber";

        public string DeadMsg9x19 { get; set; } = "There is a hole in the head. It looks like 9x19 caliber";

        public string DeadMsg12gauge { get; set; } = "There is a hole in the head. It looks like 12 gauge";

        public string DeadMsg44cal { get; set; } = "There is a hole in the head. It looks like .44 caliber";

        public string DeadMsgUnknown { get; set; } = "There is a hole in the head but it's unknown what caused it";

        public string CustomItemsSuicideInfo { get; set; } = "<size=120%>You can't commit suicide using a CustomItem</size>";
    }
}
