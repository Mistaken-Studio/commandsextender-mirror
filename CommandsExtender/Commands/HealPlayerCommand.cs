// -----------------------------------------------------------------------
// <copyright file="HealPlayerCommand.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using Exiled.API.Features.Items;
using InventorySystem.Items.Usables;
using MEC;
using Mistaken.API;
using Mistaken.API.Commands;
using Mistaken.API.Extensions;
using Mistaken.API.GUI;
using UnityEngine;

namespace Mistaken.CommandsExtender.Commands
{
    [CommandHandler(typeof(ClientCommandHandler))]
    internal sealed class HealPlayerCommand : IBetterCommand
    {
        public override string Description => "Enables all tesla gates";

        public override string[] Aliases => new string[] { "healp" };

        public override string Command => "healplayer";

        public override string[] Execute(ICommandSender sender, string[] args, out bool success)
        {
            success = false;
            var player = Player.Get(sender);
            var curItem = player.CurrentItem;

            if (curItem == null || (curItem.Type != ItemType.Medkit && curItem.Type != ItemType.SCP500))
                return new string[] { "Musisz mieć Apteczkę lub SCP-500 w ręce" };

            if (Cooldowns.TryGetValue(player, out var datetime) && datetime > DateTime.Now)
                return new string[] { $"Musisz odczekać jeszcze {Math.Ceiling((datetime - DateTime.Now).TotalSeconds)}s" };
            else
                Cooldowns.Remove(player);

            Player nearby = null;
            var ray = new Ray(player.CameraTransform.position, player.CameraTransform.forward);

            if (UnityEngine.Physics.Raycast(ray, out var hitInfo, 2))
                nearby = Player.Get(hitInfo.collider.transform.root.gameObject);

            if (nearby == null)
                return new string[] { "Musisz patrzyć się na gracza z bliska, żeby móc go uleczyć" };
            else if ((nearby.MaxHealth - nearby.Health) < 1)
                return new string[] { "Gracz nie wymaga leczenia" };

            Cooldowns.Add(player, DateTime.Now.AddSeconds(15));
            Timing.RunCoroutine(this.ExecuteHealing(player, nearby, curItem));

            success = true;
            return new string[] { "Zrobione" };
        }

        internal static readonly Dictionary<Player, DateTime> Cooldowns = new Dictionary<Player, DateTime>();

        private IEnumerator<float> ExecuteHealing(Player healer, Player healed, Item item)
        {
            healer.TryGetEffect(Exiled.API.Enums.EffectType.Ensnared, out var effect);
            effect.Intensity = 1;
            healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, true);
            healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(PluginHandler.Instance.Translation.GettingHealingMessage, 3), 5);
            yield return Timing.WaitForSeconds(1);
            Vector3 pos = healed.Position;

            for (int i = 2; i >= 0; i--)
            {
                if (!healed.IsConnected)
                    break;

                if (pos != healed.Position)
                {
                    healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, PluginHandler.Instance.Translation.HealingCancelledMessage, 5);
                    healer.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, PluginHandler.Instance.Translation.HealingCancelledHealerMessage, 5);
                    effect.Intensity = 0;
                    healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, false);
                    healer.DisableEffect<CustomPlayerEffects.Ensnared>();
                    yield break;
                }

                healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(PluginHandler.Instance.Translation.GettingHealingMessage, i));
                yield return Timing.WaitForSeconds(1f);
            }

            healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(PluginHandler.Instance.Translation.HealingSuccessHealedMessage, healer.DisplayNickname), 5);
            healer.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(PluginHandler.Instance.Translation.HealingSuccessHealerMessage, healed.DisplayNickname), 5);
            healer.RemoveItem(item, false);
            effect.Intensity = 0;
            healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, false);

            try
            {
                if (healed.Items.Count < 8)
                {
                    healed.AddItem(item);
                    ((Consumable)item.Base).ServerOnUsingCompleted();

                    if (healed.Items.Contains(item))
                    {
                        healed.RemoveItem(item, false);
                        healer.AddItem(item);
                    }
                }
                else
                {
                    var toDrop = healed.Items.First(x => !x.IsArmor && !x.IsThrowable);
                    healed.DropItem(toDrop);
                    healed.AddItem(item);
                    ((Consumable)item.Base).ServerOnUsingCompleted();

                    if (healed.Items.Contains(item))
                    {
                        healed.RemoveItem(item, false);
                        healer.AddItem(item);
                    }

                    if (healed.Items.Count < 8)
                        healed.AddItem(toDrop);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                Log.Error(ex.StackTrace);
            }
        }
    }
}
