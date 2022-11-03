// -----------------------------------------------------------------------
// <copyright file="CommandsHandler.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Mistaken.API.Diagnostics;
using Mistaken.API.Extensions;
using Mistaken.CommandsExtender.Commands;

namespace Mistaken.CommandsExtender
{
    internal sealed class CommandsHandler : Module
    {
        public static readonly HashSet<int> InSuicidialState = new();

        public CommandsHandler(PluginHandler plugin)
            : base(plugin)
        {
        }

        public override string Name => "CommandsExtender";

        public override void OnEnable()
        {
            Exiled.Events.Handlers.Server.RestartingRound += this.Server_RestartingRound;
            Exiled.Events.Handlers.Player.ChangingRole += this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Shooting += this.Player_Shooting;
            Exiled.Events.Handlers.Player.Dying += this.Player_Dying;
        }

        public override void OnDisable()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= this.Server_RestartingRound;
            Exiled.Events.Handlers.Player.ChangingRole -= this.Player_ChangingRole;
            Exiled.Events.Handlers.Player.Shooting -= this.Player_Shooting;
            Exiled.Events.Handlers.Player.Dying -= this.Player_Dying;
        }

        private static void KillPlayer(Player player, ItemType type = ItemType.None)
        {
            string reason = type switch
            {
                ItemType.GunCrossvec or ItemType.GunCOM15 or ItemType.GunCOM18 or ItemType.GunFSP9 => PluginHandler.Instance.Translation.DeadMsg9x19,
                ItemType.GunLogicer or ItemType.GunAK => PluginHandler.Instance.Translation.DeadMsg762x39,
                ItemType.GunE11SR => PluginHandler.Instance.Translation.DeadMsg556x45,
                ItemType.GunRevolver => PluginHandler.Instance.Translation.DeadMsg44cal,
                ItemType.GunShotgun => PluginHandler.Instance.Translation.DeadMsg12gauge,
                _ => PluginHandler.Instance.Translation.DeadMsgUnknown,
            };

            player.Kill(reason);
        }

        private void Player_ChangingRole(Exiled.Events.EventArgs.ChangingRoleEventArgs ev)
        {
            TryUnHandcuffCommand._cooldowns.Remove(ev.Player.UserId);
            InSuicidialState.Remove(ev.Player.Id);
        }

        private void Player_Dying(Exiled.Events.EventArgs.DyingEventArgs ev)
        {
            if (!ev.Target.IsReadyPlayer())
                return;

            if (ev.Target.Role.Type == RoleType.NtfCaptain)
                TeslaOnCommand._alreadyUsed.Remove(ev.Target.UserId);
        }

        private void Player_Shooting(Exiled.Events.EventArgs.ShootingEventArgs ev)
        {
            if (InSuicidialState.Contains(ev.Shooter.Id))
            {
                ev.IsAllowed = false;

                if (CustomItem.TryGet(ev.Shooter.CurrentItem, out _))
                {
                    ev.Shooter.SetGUI("Suicide_Fail", API.GUI.PseudoGUIPosition.MIDDLE, PluginHandler.Instance.Translation.CustomItemsSuicideInfo, 5);
                    InSuicidialState.Remove(ev.Shooter.Id);
                    return;
                }

                KillPlayer(ev.Shooter, ev.Shooter.CurrentItem.Type);
                InSuicidialState.Remove(ev.Shooter.Id);
            }
        }

        private void Server_RestartingRound()
        {
            TeslaOnCommand._alreadyUsed.Clear();
            TryUnHandcuffCommand._cooldowns.Clear();
            InSuicidialState.Clear();
        }
    }
}