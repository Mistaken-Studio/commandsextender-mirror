using PluginAPI.Core.Attributes;
using PluginAPI.Enums;

namespace Mistaken.CommandsExtender;

internal sealed class Plugin
{
    public static Plugin Instance { get; private set; }

    public static readonly Translations Translations = new();

    [PluginConfig]
    public Config Config;

    [PluginPriority(LoadPriority.Medium)]
    [PluginEntryPoint("Commands Extender", "1.0.0", "Plugin that adds more commands for players", "Mistaken Devs")]
    public void Load()
    {
        Instance = this;
        new CommandsHandler();
    }

    [PluginUnload]
    public void Unload()
    {
    }
}
