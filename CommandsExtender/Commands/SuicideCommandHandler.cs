using CommandSystem;
using PluginAPI.Core;
using System;

namespace Mistaken.CommandsExtender.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
internal sealed class SuicideCommandHandler : ICommand
{
    public string Command => "suicide";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Pozwala popełnić samobójstwo za pomocą broni";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Tylko klasy ludzkie mogą popełnić samobójstwo";
        var player = Player.Get(sender);

        if (player.IsHuman)
        {
            if (!CommandsHandler.InSuicidialState.Contains(player.UserId))
            {
                CommandsHandler.InSuicidialState.Add(player.UserId);
                response = Plugin.Translations.SuicideEnter;
            }
            else
            {
                CommandsHandler.InSuicidialState.Remove(player.UserId);
                response = Plugin.Translations.SuicideExit;
            }

            return true;
        }

        return false;
    }
}
