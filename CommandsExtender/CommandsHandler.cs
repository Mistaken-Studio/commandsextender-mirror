using InventorySystem.Items.Firearms;
using Mistaken.CommandsExtender.Commands;
using PlayerRoles;
using PlayerStatsSystem;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using PluginAPI.Events;

namespace Mistaken.CommandsExtender;

internal sealed class CommandsHandler
{
    public CommandsHandler()
    {
        EventManager.RegisterEvents(this);
    }

    ~CommandsHandler()
    {
        EventManager.UnregisterEvents(this);
    }

    [PluginEvent(ServerEventType.PlayerChangeRole)]
    private void OnPlayerChangeRole(Player player, PlayerRoleBase oldRole, RoleTypeId newRole, RoleChangeReason reason)
    {
        if (player is null)
            return;

        TryUncuff.Cooldowns.Remove(player.UserId);
        Suicide.InSuicidialState.Remove(player.UserId);
    }

    [PluginEvent(ServerEventType.PlayerDying)]
    private void OnPlayerDying(Player target, Player killer, DamageHandlerBase handler)
    {
        if (target is null)
            return;

        if (target.Role == RoleTypeId.NtfCaptain)
            TeslaOn.AlreadyUsed.Remove(target.UserId);
    }

    [PluginEvent(ServerEventType.PlayerShotWeapon)]
    private bool Player_Shooting(Player player, Firearm firearm)
    {
        if (Suicide.InSuicidialState.Contains(player.UserId))
        {
            /*if (CustomItem.TryGet(ev.Shooter.CurrentItem, out _))
            {
                ev.Shooter.SetGUI("Suicide_Fail", API.GUI.PseudoGUIPosition.MIDDLE, PluginHandler.Instance.Translation.CustomItemsSuicideInfo, 5);
                InSuicidialState.Remove(ev.Shooter.Id);
                return;
            }*/

            player.Kill(Plugin.Translations.SuicideDeathMessage);
            Suicide.InSuicidialState.Remove(player.UserId);
            return false;
        }

        return true;
    }

    [PluginEvent(ServerEventType.RoundRestart)]
    private void OnRoundRestart()
    {
        TeslaOn.AlreadyUsed.Clear();
        TryUncuff.Cooldowns.Clear();
        Suicide.InSuicidialState.Clear();
    }
}