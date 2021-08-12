// -----------------------------------------------------------------------
// <copyright file="CommandsHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Interactables.Interobjects.DoorUtils;
using Mistaken.API;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.CommandsExtender.Commands;
using Respawning;
using UnityEngine;

namespace Mistaken.CommandsExtender
{
    internal class CommandsHandler : Module
    {
        public CommandsHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => "CommandsExtender";

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RestartingRound += this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Player.ChangingRole += this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
            Exiled.Events.Handlers.Server.RoundStarted += this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Player.Dying += this.Handle<Exiled.Events.EventArgs.DyingEventArgs>((ev) => this.Player_Dying(ev));
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Handle(() => this.Server_RestartingRound(), "RoundRestart");
            Exiled.Events.Handlers.Player.ChangingRole -= this.Handle<Exiled.Events.EventArgs.ChangingRoleEventArgs>((ev) => this.Player_ChangingRole(ev));
            Exiled.Events.Handlers.Server.RoundStarted -= this.Handle(() => this.Server_RoundStarted(), "RoundStart");
            Exiled.Events.Handlers.Player.Dying -= this.Handle<Exiled.Events.EventArgs.DyingEventArgs>((ev) => this.Player_Dying(ev));
        }

        private void Server_RoundStarted()
        {
            this.CallDelayed(
                5,
                () =>
                {
                    foreach (var item in SwapSCPCommand.SwapCooldown.ToArray())
                    {
                        if (RealPlayers.List.Any(p => p.UserId == item.Key))
                        {
                            var player = RealPlayers.List.First(p => p.UserId == item.Key);
                            if (player.Team == Team.SCP)
                            {
                                if (item.Value == 1)
                                    SwapSCPCommand.SwapCooldown.Remove(player.UserId);
                                else
                                    SwapSCPCommand.SwapCooldown[player.UserId]--;
                            }
                        }
                    }
                },
                "RoundStart");
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            TryUnHandcuffCommand.Tried.Remove(ev.Player.UserId);
        }

        private void Player_Dying(Exiled.Events.EventArgs.DyingEventArgs ev)
        {
            if (!ev.Target.IsReadyPlayer())
                return;
            if (ev.Target.Role == RoleType.NtfCommander)
                TeslaOnCommand.AlreadyUsed.Remove(ev.Target.UserId);
        }

        private void Server_RestartingRound()
        {
            SwapSCPCommand.AlreadyChanged.Clear();
            TeslaOnCommand.AlreadyUsed.Clear();
        }
    }
}