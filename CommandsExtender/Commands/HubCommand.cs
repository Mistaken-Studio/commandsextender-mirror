// -----------------------------------------------------------------------
// <copyright file="HubCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using CommandSystem;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    internal class HubCommand : IBetterCommand
    {
        public override string Command => "hub";

        public string GetUsage()
        {
            return ".hub [server]";
        }

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            if (args.Length == 0 || !byte.TryParse(args[0], out byte serverId) || !(serverId == 1 || serverId == 2 || serverId == 3 || serverId == 4 || serverId == 14 || serverId == 15))
            {
                return new string[]
                {
                    "Bad arguments",
                    this.GetUsage(),
                    "Server:",

                    "1 - RP",
                    "2 - RP 2",
                    "3 - Casual",
                    "4 - Memes",
                };
            }

            var player = sender.GetPlayer();
            Exiled.API.Extensions.MirrorExtensions.SendFakeTargetRpc(player, player.Connection.identity, typeof(PlayerStats), nameof(PlayerStats.RpcRoundrestartRedirect), new object[] { 0.1f, (ushort)(7776 + serverId) });
            success = true;
            return new string[] { "Redirecting" };
        }
    }
}
