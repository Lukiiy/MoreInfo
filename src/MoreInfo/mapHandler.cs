using HarmonyLib;

namespace MoreInfo;

[HarmonyPatch(typeof(MapHandler))]
internal static class MapHandling
{
    [HarmonyPatch(nameof(MapHandler.InitializeMap))]
    [HarmonyPostfix]
    public static void Postfix()
    {
        Plugin.Instance.lobbyHud.Hide();
        Plugin.Instance.runHud.Show();
    }

    [HarmonyPatch(nameof(MapHandler.OnDestroy))]
    [HarmonyPostfix]
    public static void Postfix2() => Plugin.Instance.runHud.Hide();
}