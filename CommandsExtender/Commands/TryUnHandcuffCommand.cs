// -----------------------------------------------------------------------
// <copyright file="TryUnHandcuffCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using InventorySystem.Disarming;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using Utils.Networking;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class TryUnHandcuffCommand : IBetterCommand
    {
        public override string Description => "Try your luck";

        public override string Command => "try";

        public override string[] Aliases => new string[] { };

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = Player.Get(sender);

            if (!player.IsCuffed)
                return new string[] { "Nie jesteś skuty" };

            if (player.Position.y > 900)
                return new string[] { "Nie możesz próbować rozkuć się będąc na powierzchni" };

            if (_cooldowns.TryGetValue(player.UserId, out var time))
            {
                var diff = (time - DateTime.Now).TotalSeconds;

                if (diff <= 0)
                    _cooldowns.Remove(player.UserId);
                else
                    return new string[] { $"Musisz odczekać jeszcze {Math.Round(diff)}s zanim będziesz mógł użyć tej komendy ponownie" };
            }

            _cooldowns.Add(player.UserId, DateTime.Now.AddSeconds(PluginHandler.Instance.Config.TryCooldown));

            if (UnityEngine.Random.Range(1, 101) <= PluginHandler.Instance.Config.TrySuccessChance)
            {
                player.Cuffer = null;
                new DisarmedPlayersListMessage(DisarmedPlayers.Entries).SendToAuthenticated();
                player.EnableEffect<CustomPlayerEffects.Concussed>(10);
                player.EnableEffect<CustomPlayerEffects.Invigorated>(15);

                success = true;
                return new string[] { "Sukces" };
            }
            else
            {
                player.EnableEffect<CustomPlayerEffects.Concussed>(10);
                player.EnableEffect<CustomPlayerEffects.Disabled>(5);
                player.Cuffer.SetGUI("try", PseudoGUIPosition.BOTTOM, $"<b>!! {player.GetDisplayName()} <color=yellow>próbował</color> się rozkuć !!</b>", 10);

                success = true;
                return new string[] { "Nie udało ci się" };
            }
        }

        internal static readonly Dictionary<string, DateTime> _cooldowns = new();
    }
}
