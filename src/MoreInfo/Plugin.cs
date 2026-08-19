using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zorro.Core;

namespace MoreInfo;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private GameObject root;
    private TextHud biomeHud = null!;
    private bool dumped = false;

    private static readonly Dictionary<Biome.BiomeType, string> BiomeNames = new()
    {
        [Biome.BiomeType.Tropics] = LocalizedText.GetText("TROPICS"),
        [Biome.BiomeType.Alpine] = LocalizedText.GetText("ALPINE"),
        [Biome.BiomeType.Mesa] = LocalizedText.GetText("MESA"),
        [Biome.BiomeType.Roots] = LocalizedText.GetText("ROOTS"),
        [Biome.BiomeType.Swamp] = LocalizedText.GetText("GLOOM") + " & " + LocalizedText.GetText("THE CITADEL"),
        [Biome.BiomeType.Volcano] = LocalizedText.GetText("CALDERA") + " & " + LocalizedText.GetText("THE KILN"),
    };

    private void Awake()
    {
        Log = Logger;

        Harmony harmony = new(Info.Metadata.GUID);

        harmony.PatchAll();

        root = new GameObject("biomedisplaylobbything");

        DontDestroyOnLoad(root);

        Canvas canvas = root.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        root.AddComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
        root.SetActive(false);

        biomeHud = new TextHud(root.transform, "Biomes", new Vector2(1, 1), new Vector2(-28, -28), new Vector2(700, 160));

        biomeHud.Text.alignment = TextAlignmentOptions.TopRight;
        biomeHud.SetActive(true);
    }

    private void Update()
    {
        bool inLobby = GameUtils.instance != null && GameUtils.instance.m_inAirport;

        root.SetActive(inLobby);

        if (inLobby && !dumped)
        {
            NextLevelService service = GameHandler.GetService<NextLevelService>();
            if (service?.Data.IsSome != true) return;

            MapBaker baker = SingletonAsset<MapBaker>.Instance;
            MapBaker.BiomeResult? result = baker.selectedBiomes[service.Data.Value.CurrentLevelIndex % baker.selectedBiomes.Count];
            List<string> labels = [];

            foreach (var type in result?.biomeTypes ?? []) if (BiomeNames.TryGetValue(type, out var name)) labels.Add(name);

            biomeHud.SetText(string.Join(", ", labels));
            dumped = true;
        }
    }
}