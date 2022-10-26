// -----------------------------------------------------------------------
// <copyright file="CheckpointOpenCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using Mistaken.API.Commands;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class CheckpointOpenCommand : IBetterCommand
    {
        public override string Description => base.Description;

        public override string[] Aliases => new string[] { "checkopen" };

        public override string Command => "checkpointopen";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            var player = Player.Get(sender);
            success = false;

            if (player.Role.Type != RoleType.NtfCaptain)
                return new string[] { "Musisz być kapitanem by użyć tej komendy" };

            var door = Door.List.First(x => x.Type == DoorType.CheckpointEntrance).Base as CheckpointDoor;

            if (door._subDoors.Any(x => x.TargetState))
                return new string[] { "Drzwi od checkpoint'a są już otwarte" };

            door.ToggleAllDoors(true);
            door.ServerChangeLock(DoorLockReason.Warhead, true);
            door.NetworkTargetState = true;

            success = true;
            return new string[] { "Zrobione" };
        }
    }
}
