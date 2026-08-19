using TMPro;
using UnityEngine;

namespace MoreInfo;

public sealed class TextHud
{
    public readonly GameObject Object;
    public readonly TextMeshProUGUI Text;

    public TextHud(Transform parent, string name, Vector2 anchor, Vector2 position, Vector2 size, float fontSize = 28f)
    {
        Object = new GameObject(name);

        Object.transform.SetParent(parent, false);

        Text = Object.AddComponent<TextMeshProUGUI>();

        Text.fontSize = fontSize;
        Text.alignment = TextAlignmentOptions.TopLeft;

        RectTransform rect = Text.rectTransform;

        rect.anchorMin = rect.anchorMax = anchor;
        rect.pivot = anchor;
        rect.anchoredPosition = position;
        rect.sizeDelta = size;
    }

    public void SetText(string value) => Text.text = value;
    public void SetActive(bool active) => Object.SetActive(active);
}