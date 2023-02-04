using System;
using System.Linq;
using CommandSystem;
using Interactables.Interobjects;
using Interactables.Interobjects.DoorUtils;
using PlayerRoles;
using PluginAPI.Core;

namespace Mistaken.CommandsExtender.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
internal sealed class CheckpointOpen : ICommand
{
    public string Command => "checkpointopen";

    public string[] Aliases => new[] { "checkopen" };

    public string Description => "Otwiera przejścia HCZ-EZ na stałe";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Zrobione";
        var player = Player.Get(sender);

        if (player.Role != RoleTypeId.NtfCaptain)
        {
            response = "Musisz być kapitanem by użyć tej komendy";
            return false;
        }

        var checkpoints = DoorVariant.AllDoors.Where(x => x is CheckpointDoor door && x.transform.position.y < -900f).Cast<CheckpointDoor>();

        foreach (var door in checkpoints)
        {
            if (door._subDoors.Any(x => x.TargetState) && ((DoorLockReason)door.NetworkActiveLocks & DoorLockReason.Warhead) == DoorLockReason.Warhead)
            {
                response = "Drzwi od checkpoint'a są już otwarte";
                return false;
            }

            door.ToggleAllDoors(true);
            door.ServerChangeLock(DoorLockReason.Warhead, true);
            door.NetworkTargetState = true;
        }
        
        return true;
    }
}
