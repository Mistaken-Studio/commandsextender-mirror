// -----------------------------------------------------------------------
// <copyright file="TryUnHandcuffCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class TryUnHandcuffCommand : IBetterCommand
    {
        public override string Description => "Try your luck";

        public override string Command => "try";

        public override string[] Aliases => new string[] { };

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = sender.GetPlayer();
            if (!player.IsCuffed)
                return new string[] { "Nie jesteś skuty" };
            if (Tried.Contains(player.UserId))
                return new string[] { "Możesz próbować tylko raz na życie" };
            Tried.Add(player.UserId);
            if (UnityEngine.Random.Range(1, 101) < 6 && player.Position.y < 800)
            {
                player.CufferId = -1;
                player.EnableEffect<CustomPlayerEffects.Amnesia>(10);
                player.EnableEffect<CustomPlayerEffects.Disabled>(10);
                player.EnableEffect<CustomPlayerEffects.Concussed>(10);
                player.EnableEffect<CustomPlayerEffects.Bleeding>();
                success = true;
                return new string[] { "Sukces" };
            }
            else
            {
                player.EnableEffect<CustomPlayerEffects.Amnesia>(10);
                player.EnableEffect<CustomPlayerEffects.Disabled>(15);
                player.EnableEffect<CustomPlayerEffects.Concussed>(30);
                player.EnableEffect<CustomPlayerEffects.Bleeding>();
                Player cuffer = RealPlayers.Get(player.CufferId);
                cuffer.SetGUI("try", PseudoGUIPosition.BOTTOM, $"<b>!! {player.Nickname} <color=yellow>próbował</color> się rozkuć !!</b>", 10);
                success = true;
                return new string[] { "Nie udało ci się" };
            }
        }

        internal static readonly HashSet<string> Tried = new HashSet<string>();
    }
}
