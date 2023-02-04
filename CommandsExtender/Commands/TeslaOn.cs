using System.Collections.Generic;
using CommandSystem;
using Mistaken.API.Commands;
using PlayerRoles;
using PluginAPI.Core;

namespace Mistaken.CommandsExtender.Commands;

// [CommandHandler(typeof(ClientCommandHandler))]
internal sealed class TeslaOn : IBetterCommand
{
    public static readonly HashSet<string> AlreadyUsed = new();

    public override string Description => "Enables all tesla gates";

    public override string Command => "teslaOn";

    public override string[] Execute(ICommandSender sender, string[] args, out bool success)
    {
        success = false;
        var player = Player.Get(sender);

        if (player.Role != RoleTypeId.NtfCaptain)
            return new string[] { "Nie jesteś kapitanem" };

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
}
