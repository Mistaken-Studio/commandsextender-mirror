using CommandSystem;
using PluginAPI.Core;
using RoundRestarting;
using System;

namespace Mistaken.CommandsExtender.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
internal sealed class Hub : ICommand
{
    public string Command => "hub";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Pozwala przełączyć między serwerami Mistaken";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Redirecting";
        var player = Player.Get(sender);

        if (arguments.Count == 0 || !byte.TryParse(arguments.At(0), out byte serverId) || !(serverId == 1 || serverId == 2 || serverId == 3 || serverId == 4 || serverId == 14 || serverId == 15))
        {
            response = string.Join("\n", new[]
            {
                "Bad arguments",
                ".hub [server]",
                "Server:",
                "1 - RP",
                "2 - RP 2",
                "3 - Casual",
                "4 - Memes",
            });

            return false;
        }

        player.Connection.Send(new RoundRestartMessage(RoundRestartType.RedirectRestart, 1f, (ushort)(7776 + serverId), true, false));
        return true;
    }
}
