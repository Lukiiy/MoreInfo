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

    private TextMeshProUGUI text;
    private GameObject root;
    private bool dumped;

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

        var textObj = new GameObject("biomedisplaytext");

        textObj.transform.SetParent(root.transform, false);

        text = textObj.AddComponent<TextMeshProUGUI>();
        text.alignment = TextAlignmentOptions.BottomLeft;
        text.fontSize = 28;

        var textRect = text.rectTransform;

        textRect.anchorMin = textRect.anchorMax = textRect.pivot = Vector2.zero;
        textRect.anchoredPosition = new Vector2(28, 28);
        textRect.sizeDelta = new Vector2(700, 160);

        root.SetActive(false);
    }

    private void Update()
    {
        bool inLobby = GameUtils.instance != null && GameUtils.instance.m_inAirport;

        root.SetActive(inLobby);
        if (!inLobby || dumped) return;

        PlayerConnectionLog fontGrabberTargetlol = FindAnyObjectByType<PlayerConnectionLog>();
        if (fontGrabberTargetlol != null && fontGrabberTargetlol.text != null) text.font = fontGrabberTargetlol.text.font;

        var baker = SingletonAsset<MapBaker>.Instance;
        var service = GameHandler.GetService<NextLevelService>();

        if (service?.Data.IsSome != true) return;

        int index = service.Data.Value.CurrentLevelIndex;
        var result = baker.selectedBiomes[index % baker.selectedBiomes.Count];

        List<string> labels = [];

        foreach (Biome.BiomeType type in result.biomeTypes) if (BiomeNames.TryGetValue(type, out var name)) labels.Add(name);

        text.text = string.Join(", ", labels);
        dumped = true;
    }
}