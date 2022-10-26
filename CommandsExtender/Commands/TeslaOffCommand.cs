// -----------------------------------------------------------------------
// <copyright file="TeslaOffCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;

namespace Mistaken.CommandsExtender.Commands
{
    // [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class TeslaOffCommand : IBetterCommand
    {
        public override string Description => "Disabled all tesla gates";

        public override string Command => "teslaOff";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = Player.Get(sender);

            if (player.Role.Type != RoleType.NtfCaptain)
                return new string[] { "Nie jesteś kapitanem" };

            if (API.Utilities.Map.TeslaMode == API.Utilities.TeslaMode.DISABLED)
                return new string[] { "Tesle są już wyłączone" };

            if (TeslaOnCommand._alreadyUsed.Contains(player.UserId))
                return new string[] { "Możesz użyć .taslaOff lub .teslaOn tylko raz na runde" };

            API.Utilities.Map.TeslaMode = API.Utilities.TeslaMode.DISABLED;
            TeslaOnCommand._alreadyUsed.Add(player.UserId);
            Cassie.Message("Tesla gates deactivated by order of NINETAILEDFOX COMMANDER");

            success = true;
            return new string[] { "Zrobione" };
        }
    }
}
