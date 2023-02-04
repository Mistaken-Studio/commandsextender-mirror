namespace Mistaken.CommandsExtender;

internal sealed class Translations
{
    public string GettingHealingMessage { get; set; } = "<color=red><size=150%>Jesteś leczony</size></color><br>Stój w miejscu przez <color=yellow>{0}</color>s";

    public string HealingCancelledMessage { get; set; } = "<color=red><size=150%>Leczenie anulowane</size></color>";

    public string HealingCancelledHealerMessage { get; set; } = "<color=red><size=150%>Leczenie anulowane</size></color><br>Gracz <color=yellow>poruszył się</color>";

    public string HealingSuccessHealerMessage { get; set; } = "Pomyślnie <color=green>uleczyłeś</color> <color=yellow>{0}</color>";

    public string HealingSuccessHealedMessage { get; set; } = "Zostałeś <color=green>uleczony</color> przez <color=yellow>{0}</color>";

    public string SuicideEnter { get; set; } = "Strzel by się zabić";

    public string SuicideExit { get; set; } = "Teraz możesz strzelić bez zabicia się";

    public string SuicideDeathMessage { get; set; } = "Wygląda jakby miał dziurę w głowie spowodowaną wystrzałem z broni z bliskiej odległości";

    // public string CustomItemsSuicideInfo { get; set; } = "<size=120%>You can't commit suicide using a CustomItem</size>";
}
