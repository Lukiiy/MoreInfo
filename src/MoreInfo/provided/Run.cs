using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

namespace MoreInfo.provided;

public sealed class Run
{
    private readonly TextHud hud;

    public bool IsActive { get; private set; }

    public Run(Transform parent)
    {
        hud = new TextHud(parent, "RunHud", new Vector2(0, 1), new Vector2(28, -28), new Vector2(500, 250));

        hud.Text.alignment = TMPro.TextAlignmentOptions.TopLeft;
        IsActive = false;
    }

    public void Update()
    {
        if (!MapHandler.ExistsAndInitialized) return;

        List<string> lines = [];

        AddProgress(lines);
        AddHazardSpeed(lines);
        AddSpectators(lines);

        hud.SetText(string.Join("\n", lines));
    }

    public void Show()
    {
        IsActive = true;
        hud.SetActive(true);

        Update();
    }

    public void Hide()
    {
        IsActive = false;
        hud.SetActive(false);
    }

    private static void AddProgress(List<string> lines)
    {
        int segment = (int) MapHandler.CurrentSegmentNumber;
        if (segment + 1 >= MapHandler.Instance.segments.Length) return;

        Vector3 start = MapHandler.GetCampfireRoot(segment).transform.position;
        Character player = Character.localCharacter;
        if (player == null) return;

        float total = Vector3.Distance(start, MapHandler.GetCampfireRoot(segment + 1).transform.position);
        if (total <= 0f) return;

        float travelled = Vector3.Distance(start, player.Center);
        float progress = Mathf.Clamp01(travelled / total) * 100f;

        lines.Add($"{progress:F0}%");
    }

    private static void AddHazardSpeed(List<string> lines)
    {
        Segment segment = MapHandler.CurrentSegmentNumber;
        if (segment != Segment.TheKiln && segment != Segment.Void) return;

        foreach (LavaRising hazard in LavaRising.ALL_LAVA)
        {
            if (hazard.requiredSegment != segment || !hazard.started || hazard.ended) continue;

            float distance = Mathf.Abs(hazard.topTransform.position.y - hazard.startHeight);
            float speed = distance / hazard.travelTime;

            lines.Add($"Rising at {speed * CharacterStats.unitsToMeters:0.0} m/s");
            return;
        }
    }

    private static void AddSpectators(List<string> lines)
    {
        int spectators = 0;
        foreach (Character character in PlayerHandler.GetAllPlayerCharacters()) if (character.data.dead) spectators++;

        if (spectators > 0) lines.Add($"{spectators} spectators");
    }

    public void SetActive(bool active) => hud.SetActive(active);
}