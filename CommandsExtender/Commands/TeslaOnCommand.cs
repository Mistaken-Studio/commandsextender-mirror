// -----------------------------------------------------------------------
// <copyright file="TeslaOnCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class TeslaOnCommand : IBetterCommand
    {
        public override string Description => "Enables all tesla gates";

        public override string Command => "teslaOn";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            if (player.Role != RoleType.NtfCaptain)
                return new string[] { "Nie jesteś dowódcą" };
            if (API.Utilities.Map.TeslaMode == API.Utilities.TeslaMode.ENABLED)
                return new string[] { "Tesle są już włączone" };
            if (AlreadyUsed.Contains(player.UserId))
                return new string[] { "Możesz użyć .taslaOff lub .teslaOn tylko raz na runde" };
            API.Utilities.Map.TeslaMode = API.Utilities.TeslaMode.ENABLED;
            AlreadyUsed.Add(player.UserId);
            Cassie.Message("Tesla gates activated by order of NINETAILEDFOX COMMANDER");
            success = true;
            return new string[] { "Zrobione" };
        }

        internal static readonly HashSet<string> AlreadyUsed = new HashSet<string>();
    }
}
