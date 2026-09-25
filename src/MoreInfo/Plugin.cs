using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MoreInfo.provided;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MoreInfo;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    public static Plugin Instance { get; private set; } = null!;

    public static Lobby LobbyHud { get; private set; } = null!;
    public static Run RunHud { get; private set; } = null!;

    private GameObject root = null!;

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
        Instance = this;

        Harmony harmony = new(Info.Metadata.GUID);

        harmony.PatchAll();

        root = new GameObject("biomedisplaylobbything");

        DontDestroyOnLoad(root);

        Canvas canvas = root.AddComponent<Canvas>();

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        root.AddComponent<CanvasScaler>().referenceResolution = new Vector2(1920, 1080);
    }

    private void OnEnable() => SceneManager.sceneLoaded += SceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= SceneLoaded;
    private void SceneLoaded(Scene scene, LoadSceneMode mode) => StartCoroutine(InitializeHud());

    private IEnumerator<object?> InitializeHud()
    {
        yield return null;

        LobbyHud ??= new Lobby(root.transform);
        RunHud ??= new Run(root.transform);

        if (GameUtils.instance?.m_inAirport == true) LobbyHud?.Show();
    }
}