// -----------------------------------------------------------------------
// <copyright file="TryUnHandcuffCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;

namespace Mistaken.CommandsExtender.Commands
{
    // [CommandHandler(typeof(ClientCommandHandler))]
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

            if (_tried.Contains(player.UserId))
                return new string[] { "Możesz próbować tylko raz na życie" };

            _tried.Add(player.UserId);

            if (UnityEngine.Random.Range(1, 101) < 6 && player.Position.y < 800)
            {
                player.Cuffer = null;
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
                player.Cuffer.SetGUI("try", PseudoGUIPosition.BOTTOM, $"<b>!! {player.Nickname} <color=yellow>próbował</color> się rozkuć !!</b>", 10);

                success = true;
                return new string[] { "Nie udało ci się" };
            }
        }

        internal static readonly HashSet<string> _tried = new();
    }
}
