using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractPrompt : MonoBehaviour
{
    public string label = "USE";
    [SerializeField] private float heightAboveTop = 0.4f;
    [SerializeField] private float fontSize = 4f;

    private static GameObject keyPopup;
    private static int visibleCount;

    private TextMeshPro hoverText;
    private bool visible;

    public static InteractPrompt GetOrAdd(GameObject target, string defaultLabel)
    {
        if (target.TryGetComponent(out InteractPrompt prompt)) return prompt;
        prompt = target.AddComponent<InteractPrompt>();
        prompt.label = defaultLabel;
        return prompt;
    }

    public void SetVisible(bool show)
    {
        if (show) Show();
        else Hide();
    }

    public void Show()
    {
        if (visible) return;
        visible = true;

        if (hoverText == null) BuildHoverText();
        hoverText.text = label;
        hoverText.transform.position = GetHoverPosition();
        hoverText.gameObject.SetActive(true);

        visibleCount++;
        if (keyPopup == null) BuildKeyPopup();
        keyPopup.SetActive(true);
    }

    public void Hide()
    {
        if (!visible) return;
        visible = false;

        if (hoverText != null) hoverText.gameObject.SetActive(false);

        visibleCount = Mathf.Max(visibleCount - 1, 0);
        if (keyPopup != null) keyPopup.SetActive(visibleCount > 0);
    }

    private void LateUpdate()
    {
        if (visible && hoverText != null)
            hoverText.transform.position = GetHoverPosition();
    }

    private void OnDisable()
    {
        Hide();
    }

    private void OnDestroy()
    {
        if (hoverText != null) Destroy(hoverText.gameObject);
    }

    private Vector3 GetHoverPosition()
    {
        float top = transform.position.y;
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
            top = spriteRenderer.bounds.max.y;
        else if (TryGetComponent(out Collider2D col))
            top = col.bounds.max.y;

        return new Vector3(transform.position.x, top + heightAboveTop, transform.position.z);
    }

    private void BuildHoverText()
    {
        GameObject textObject = new GameObject(name + " Prompt");
        hoverText = textObject.AddComponent<TextMeshPro>();
        hoverText.fontSize = fontSize;
        hoverText.fontStyle = FontStyles.Bold;
        hoverText.alignment = TextAlignmentOptions.Center;
        hoverText.color = Color.white;
        hoverText.outlineWidth = 0.25f;
        hoverText.outlineColor = Color.black;
        hoverText.textWrappingMode = TextWrappingModes.NoWrap;
        hoverText.rectTransform.sizeDelta = new Vector2(4f, 1f);
        textObject.GetComponent<MeshRenderer>().sortingOrder = 100;
        textObject.SetActive(false);
    }

    private static void BuildKeyPopup()
    {
        keyPopup = new GameObject("Interact Key Popup");
        Canvas canvas = keyPopup.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        CanvasScaler scaler = keyPopup.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        GameObject panel = CreateUIObject("Panel", keyPopup.transform);
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

        GameObject keyBox = CreateUIObject("Key", panel.transform);
        keyBox.AddComponent<Image>().color = Color.white;
        LayoutElement keySize = keyBox.AddComponent<LayoutElement>();
        keySize.preferredWidth = keySize.preferredHeight = 56f;
        TextMeshProUGUI keyText = CreateText("Key Text", keyBox.transform, "E", Color.black);
        RectTransform keyTextRect = keyText.rectTransform;
        keyTextRect.anchorMin = Vector2.zero;
        keyTextRect.anchorMax = Vector2.one;
        keyTextRect.offsetMin = keyTextRect.offsetMax = Vector2.zero;

        CreateText("Label", panel.transform, "Interact", Color.white);

        keyPopup.SetActive(false);
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
