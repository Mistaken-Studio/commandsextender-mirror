// -----------------------------------------------------------------------
// <copyright file="TimerCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using Mistaken.API.Commands;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandSystem.CommandHandler(typeof(CommandSystem.ClientCommandHandler))]
    [CommandSystem.CommandHandler(typeof(CommandSystem.RemoteAdminCommandHandler))]
    internal class TimerCommand : IBetterCommand
    {
        public override string Command => "timer";

        public override string Description => "Timer";

        public override string[] Execute(CommandSystem.ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            if (args.Length > 0 && float.TryParse(args[0], out float time))
            {
                Player player = sender.GetPlayer();
                string message = "TIME OUT";
                if (args.Length > 1)
                    message = string.Join(" ", args.Skip(1));
                Module.RunSafeCoroutine(this.DoTimer(player, time, message), "Timer.DoTimer");
                success = true;
                return new string[] { "Done" };
            }
            else
                return new string[] { this.GetUsage() };
        }

        public string GetUsage() => "TIMER [TIME] (MESSAGE)";

        private IEnumerator<float> DoTimer(Player player, float time, string message)
        {
            yield return MEC.Timing.WaitForSeconds(time);
            if (message.Trim().StartsWith("-c") && player.RemoteAdminAccess)
            {
                foreach (var item in message.Substring(2).Split(';'))
                    RemoteAdmin.CommandProcessor.ProcessQuery(item, player.Sender);
            }
            else
            {
                player.ClearBroadcasts();
                player.Broadcast("Timer", 10, message);
            }
        }
    }
}
