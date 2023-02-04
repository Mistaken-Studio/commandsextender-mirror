using System.ComponentModel;

namespace Mistaken.CommandsExtender;

internal sealed class Config
{
    [Description("A chance for the player to become uncuffed after using this command (0 - 100)")]
    public int TrySuccessChance { get; set; } = 15;

    [Description("Cooldown after which this command can be used again (in seconds)")]
    public float TryCooldown { get; set; } = 200;
}
