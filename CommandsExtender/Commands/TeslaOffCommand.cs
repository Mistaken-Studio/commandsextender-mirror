// -----------------------------------------------------------------------
// <copyright file="TeslaOffCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;

namespace Mistaken.CommandsExtender.Commands
{
    // [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class TeslaOffCommand : IBetterCommand
    {
        public override string Description => "Disabled all tesla gates";

        public override string Command => "teslaOff";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            if (player.Role != RoleType.NtfCaptain)
                return new string[] { "Nie jesteś dowódcą" };
            if (API.Utilities.Map.TeslaMode == API.Utilities.TeslaMode.DISABLED)
                return new string[] { "Tesle są już wyłączone" };
            if (TeslaOnCommand.AlreadyUsed.Contains(player.UserId))
                return new string[] { "Możesz użyć .taslaOff lub .teslaOn tylko raz na runde" };
            API.Utilities.Map.TeslaMode = API.Utilities.TeslaMode.DISABLED;
            TeslaOnCommand.AlreadyUsed.Add(player.UserId);
            Cassie.Message("Tesla gates deactivated by order of NINETAILEDFOX COMMANDER");
            success = true;
            return new string[] { "Zrobione" };
        }
    }
}
