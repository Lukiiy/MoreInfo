using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

namespace MoreInfo.provided;

public sealed class Lobby
{
    private readonly TextHud hud;

    private static readonly Dictionary<Biome.BiomeType, string> BiomeNames = new()
    {
        [Biome.BiomeType.Tropics] = LocalizedText.GetText("TROPICS"),
        [Biome.BiomeType.Alpine] = LocalizedText.GetText("ALPINE"),
        [Biome.BiomeType.Mesa] = LocalizedText.GetText("MESA"),
        [Biome.BiomeType.Roots] = LocalizedText.GetText("ROOTS"),
        [Biome.BiomeType.Swamp] = LocalizedText.GetText("GLOOM") + " & " + LocalizedText.GetText("THE CITADEL"),
        [Biome.BiomeType.Volcano] = LocalizedText.GetText("CALDERA") + " & " + LocalizedText.GetText("THE KILN"),
    };

    public Lobby(Transform parent)
    {
        hud = new TextHud(parent, "Biomes", new Vector2(1, 1), new Vector2(-28, -28), new Vector2(700, 160));

        hud.Text.alignment = TMPro.TextAlignmentOptions.TopRight;
    }

    public void Show()
    {
        hud.SetActive(true);
        Update();
    }

    public void Hide() => hud.SetActive(false);

    public void Update()
    {
        NextLevelService service = GameHandler.GetService<NextLevelService>();
        if (service?.Data.IsSome != true) return;

        MapBaker baker = SingletonAsset<MapBaker>.Instance;
        MapBaker.BiomeResult? result = baker.selectedBiomes[service.Data.Value.CurrentLevelIndex % baker.selectedBiomes.Count];
        List<string> labels = [];

        foreach (var type in result?.biomeTypes ?? []) if (BiomeNames.TryGetValue(type, out var name)) labels.Add(name);

        hud.SetText(string.Join(", ", labels));
    }

    public void SetActive(bool active) => hud.SetActive(active);
}