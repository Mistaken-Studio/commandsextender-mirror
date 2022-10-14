// -----------------------------------------------------------------------
// <copyright file="SuicideCommandHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Exiled.API.Features;
using Mistaken.API.Commands;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal class SuicideCommandHandler : IBetterCommand
    {
        public override string Command => "suicide";

        public override string[] Aliases => new string[] { };

        public override string Description => "Commit suicide";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = Player.Get(sender);

            if (player.IsHuman)
            {
                success = true;
                if (!CommandsHandler.InSuicidialState.Contains(player.Id))
                {
                    CommandsHandler.InSuicidialState.Add(player.Id);
                    return new string[] { PluginHandler.Instance.Translation.SuicideEnter };
                }
                else
                {
                    CommandsHandler.InSuicidialState.Remove(player.Id);
                    return new string[] { PluginHandler.Instance.Translation.SuicideExit };
                }
            }

            return new string[] { "Only human's can commit suicide!" };
        }
    }
}
