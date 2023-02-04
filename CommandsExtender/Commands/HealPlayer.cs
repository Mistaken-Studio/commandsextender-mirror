using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CustomPlayerEffects;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Armor;
using InventorySystem.Items.ThrowableProjectiles;
using InventorySystem.Items.Usables;
using MEC;
using Mistaken.API;
using Mistaken.API.Extensions;
using Mistaken.PseudoGUI;
using PluginAPI.Core;
using UnityEngine;

namespace Mistaken.CommandsExtender.Commands;

[CommandHandler(typeof(ClientCommandHandler))]
internal sealed class HealPlayer : ICommand
{
    internal static readonly Dictionary<Player, DateTime> Cooldowns = new();

    public string Command => "healplayer";

    public string[] Aliases => new[] { "healp" };

    public string Description => "Leczy gracza na którego się patrzysz";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = "Zrobione";
        var player = Player.Get(sender);
        var curItem = player.CurrentItem;

        if (curItem == null || (curItem.ItemTypeId != ItemType.Medkit && curItem.ItemTypeId != ItemType.SCP500))
        {
            response = "Musisz mieć Apteczkę lub SCP-500 w ręce";
            return false;
        }

        if (Cooldowns.TryGetValue(player, out var datetime) && datetime > DateTime.Now)
        {
            response = $"Musisz odczekać jeszcze {Math.Ceiling((datetime - DateTime.Now).TotalSeconds)}s";
            return false;
        }
        else
            Cooldowns.Remove(player);

        Player nearby = null;
        var ray = new Ray(player.Camera.position, player.Camera.forward);

        if (Physics.Raycast(ray, out var hitInfo, 2))
            nearby = Player.Get(hitInfo.collider.transform.root.gameObject);

        if (nearby == null)
        {
            response = "Musisz patrzyć się na gracza z bliska, żeby móc go uleczyć";
            return false;
        }
        else if ((nearby.MaxHealth - nearby.Health) < 1)
        {
            response = "Gracz nie wymaga leczenia";
            return false;
        }

        Cooldowns.Add(player, DateTime.Now.AddSeconds(15));
        Timing.RunCoroutine(ExecuteHealing(player, nearby, curItem));

        return true;
    }

    private IEnumerator<float> ExecuteHealing(Player healer, Player healed, ItemBase item)
    {
        healer.EffectsManager.EnableEffect<Ensnared>();
        healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, true);
        healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(Plugin.Translations.GettingHealingMessage, 3), 5);

        yield return Timing.WaitForSeconds(1);
        Vector3 pos = healed.Position;
        for (int i = 2; i >= 0; i--)
        {
            if (!healed.IsConnected())
                break;

            if (pos != healed.Position)
            {
                healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, Plugin.Translations.HealingCancelledMessage, 5);
                healer.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, Plugin.Translations.HealingCancelledHealerMessage, 5);
                healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, false);
                healer.EffectsManager.DisableEffect<Ensnared>();
                yield break;
            }

            healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(Plugin.Translations.GettingHealingMessage, i));
            yield return Timing.WaitForSeconds(1f);
        }

        healed.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(Plugin.Translations.HealingSuccessHealedMessage, healer.DisplayNickname), 5);
        healer.SetGUI("healplayer", PseudoGUIPosition.MIDDLE, string.Format(Plugin.Translations.HealingSuccessHealerMessage, healed.DisplayNickname), 5);
        healer.RemoveItem(item);
        healer.EffectsManager.DisableEffect<Ensnared>();
        healer.SetSessionVariable(SessionVarType.BLOCK_INVENTORY_INTERACTION, false);

        try
        {
            if (healed.Items.Count < 8)
            {
                healed.AddItem(item.ItemTypeId);
                ((Consumable)item).ServerOnUsingCompleted();

                /*if (healed.Items.Contains(item))
                {
                    healed.RemoveItem(item, false);
                    healer.AddItem(item);
                }*/
            }
            else
            {
                var toDrop = healed.Items.First(x => x is not BodyArmor && x is not ThrowableItem);
                var pickup = healed.ReferenceHub.inventory.ServerDropItem(toDrop.ItemSerial);
                healed.AddItem(item.ItemTypeId);
                ((Consumable)item).ServerOnUsingCompleted();

                /*if (healed.Items.Contains(item))
                {
                    healed.RemoveItem(item, false);
                    healer.AddItem(item);
                }*/

                if (healed.Items.Count < 8)
                    healed.ReferenceHub.inventory.ServerAddItem(toDrop.ItemTypeId, toDrop.ItemSerial, pickup);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }
}
