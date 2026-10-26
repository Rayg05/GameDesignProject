using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Bar at the bottom of the screen listing controls, e.g. [E] Select  [ESC] Back.
// Same look as the "E Interact" popup from InteractPrompt.
public static class KeyHintBar
{
    public static GameObject Create(string name, params (string key, string label)[] hints)
    {
        GameObject bar = new GameObject(name);
        Canvas canvas = bar.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = bar.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUIObject("Panel", bar.transform);
        panel.AddComponent<Image>().color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 60f);
        HorizontalLayoutGroup layout = panel.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(16, 24, 12, 12);
        layout.spacing = 16f;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        foreach ((string key, string label) in hints)
        {
            // white key cap that grows to fit longer keys like ESC
            GameObject keyBox = CreateUIObject(key + " Key", panel.transform);
            keyBox.AddComponent<Image>().color = Color.white;
            HorizontalLayoutGroup keyLayout = keyBox.AddComponent<HorizontalLayoutGroup>();
            keyLayout.padding = new RectOffset(14, 14, 4, 4);
            keyLayout.childAlignment = TextAnchor.MiddleCenter;
            keyLayout.childControlWidth = keyLayout.childControlHeight = true;
            LayoutElement keySize = keyBox.AddComponent<LayoutElement>();
            keySize.minWidth = keySize.minHeight = 56f;
            CreateText("Key Text", keyBox.transform, key, Color.black);

            CreateText(label, panel.transform, label, Color.white);
        }

        bar.SetActive(false);
        return bar;
    }

    // Warning banner in the same style, sitting just above the hint bar, e.g. "No loadout equipped"
    public static GameObject CreateWarning(string name, string message, Color background)
    {
        GameObject warning = new GameObject(name);
        Canvas canvas = warning.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 101;
        CanvasScaler scaler = warning.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUIObject("Panel", warning.transform);
        panel.AddComponent<Image>().color = background;
        RectTransform panelRect = (RectTransform)panel.transform;
        panelRect.anchorMin = panelRect.anchorMax = new Vector2(0.5f, 0f);
        panelRect.pivot = new Vector2(0.5f, 0f);
        panelRect.anchoredPosition = new Vector2(0f, 170f); // hint bar is at 60 and ~80 tall
        HorizontalLayoutGroup layout = panel.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(32, 32, 14, 14);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = panel.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        TextMeshProUGUI text = CreateText("Message", panel.transform, message, Color.white);
        text.fontSize = 44f;
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;

        warning.SetActive(false);
        return warning;
    }

    private static GameObject CreateUIObject(string objectName, Transform parent)
    {
        GameObject go = new GameObject(objectName, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static TextMeshProUGUI CreateText(string objectName, Transform parent, string text, Color color)
    {
        TextMeshProUGUI tmp = CreateUIObject(objectName, parent).AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.color = color;
        tmp.fontSize = 36f;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.textWrappingMode = TextWrappingModes.NoWrap;
        return tmp;
    }
}
