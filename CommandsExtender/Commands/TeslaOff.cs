using CommandSystem;
using Mistaken.API.Commands;
using PlayerRoles;
using PluginAPI.Core;

namespace Mistaken.CommandsExtender.Commands;

// [CommandHandler(typeof(ClientCommandHandler))]
internal sealed class TeslaOff : IBetterCommand
{
    public override string Description => "Disabled all tesla gates";

    public override string Command => "teslaOff";

    public override string[] Execute(ICommandSender sender, string[] args, out bool success)
    {
        success = false;
        var player = Player.Get(sender);

        if (player.Role != RoleTypeId.NtfCaptain)
            return new string[] { "Nie jesteś kapitanem" };

        if (API.Utilities.Map.TeslaMode == API.Utilities.TeslaMode.DISABLED)
            return new string[] { "Tesle są już wyłączone" };

        if (TeslaOn.AlreadyUsed.Contains(player.UserId))
            return new string[] { "Możesz użyć .taslaOff lub .teslaOn tylko raz na runde" };

        API.Utilities.Map.TeslaMode = API.Utilities.TeslaMode.DISABLED;
        TeslaOn.AlreadyUsed.Add(player.UserId);
        Cassie.Message("Tesla gates deactivated by order of NINETAILEDFOX COMMANDER");

        success = true;
        return new string[] { "Zrobione" };
    }
}
