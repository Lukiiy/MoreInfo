using HarmonyLib;

namespace MoreInfo;

internal static class MainMenuing
{
    [HarmonyPatch(nameof(MainMenu.Start))] // now this just feels like a lifehack... or rather, devhack
    [HarmonyPostfix]
    public static void Postfix() => Plugin.Instance.lobbyHud.Hide();
}