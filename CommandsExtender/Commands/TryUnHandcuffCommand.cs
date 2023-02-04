using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using InventorySystem.Disarming;
using Mistaken.API.Extensions;
using Mistaken.PseudoGUI;
using PluginAPI.Core;
using Utils.Networking;

namespace Mistaken.CommandsExtender.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
internal sealed class TryUnHandcuffCommand : ICommand
{
    public static readonly Dictionary<string, DateTime> Cooldowns = new();

    public string Command => "try";

    public string[] Aliases => Array.Empty<string>();

    public string Description => "Próba rozkucia się";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        var player = Player.Get(sender);

        if (!player.ReferenceHub.inventory.IsDisarmed())
        {
            response = "Nie jesteś skuty";
            return false;
        }

        if (player.Position.y > 900f)
        {
            response = "Nie możesz próbować rozkuć się będąc na powierzchni";
            return false;
        }

        if (Cooldowns.TryGetValue(player.UserId, out var time))
        {
            var diff = (time - DateTime.Now).TotalSeconds;

            if (diff <= 0)
                Cooldowns.Remove(player.UserId);
            else
            {
                response = $"Musisz odczekać jeszcze {Math.Round(diff)}s zanim będziesz mógł użyć tej komendy ponownie";
                return false;
            }
        }

        Cooldowns.Add(player.UserId, DateTime.Now.AddSeconds(Plugin.Instance.Config.TryCooldown));

        if (UnityEngine.Random.Range(1, 101) <= Plugin.Instance.Config.TrySuccessChance)
        {
            DisarmedPlayers.Entries.Remove(DisarmedPlayers.Entries.First(x => x.DisarmedPlayer == player.NetworkId));
            new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
            player.EffectsManager.EnableEffect<CustomPlayerEffects.Concussed>(10);
            player.EffectsManager.EnableEffect<CustomPlayerEffects.Invigorated>(10);
            response = "Sukces";
        }
        else
        {
            var cuffer = Player.Get(DisarmedPlayers.Entries.First(x => x.DisarmedPlayer == player.NetworkId).Disarmer);
            player.EffectsManager.EnableEffect<CustomPlayerEffects.Concussed>(10);
            player.EffectsManager.EnableEffect<CustomPlayerEffects.Disabled>(5);
            cuffer.SetGUI("try", PseudoGUIPosition.BOTTOM, $"<b>!! {player.GetDisplayName()} <color=yellow>próbował</color> się rozkuć !!</b>", 10);
            response = "Nie udało ci się";
        }

        return true;
    }
}
