using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Small key badge ("I", "Q", "L-Click") shown to the left of a HUD element
public class KeybindLabel : MonoBehaviour
{
    private TextMeshProUGUI keyText;
    private bool hasKey;

    // subscribed for the badge's whole life since hiding it deactivates the object
    private void Awake() => InventoryWindow.OpenChanged += OnInventoryOpenChanged;
    private void OnDestroy() => InventoryWindow.OpenChanged -= OnInventoryOpenChanged;

    private void OnInventoryOpenChanged(bool open) => UpdateVisible();

    public static KeybindLabel Create(RectTransform target, string key)
    {
        GameObject badge = new GameObject("Keybind", typeof(RectTransform));
        badge.transform.SetParent(target, false);

        // sits just left of the target, vertically centered
        RectTransform rect = (RectTransform)badge.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
        rect.pivot = new Vector2(1f, 0.5f);
        rect.anchoredPosition = new Vector2(-8f, 0f);

        // If the target is flipped (e.g. the Inventory button is rotated 180 on Y), its "left" is on the
        // right of the screen. Put the badge on its other side and un-flip it so it still shows on the left.
        Transform canvasRoot = target.GetComponentInParent<Canvas>().rootCanvas.transform;
        if (Vector3.Dot(target.right, canvasRoot.right) < 0f)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
            rect.anchoredPosition = new Vector2(8f, 0f);
            rect.localRotation = Quaternion.Euler(0f, 180f, 0f);
        }

        // undo any scale on the target so every badge is the same size
        float relativeScale = Mathf.Abs(target.lossyScale.x / canvasRoot.lossyScale.x);
        if (relativeScale > 0f) rect.localScale = Vector3.one / relativeScale;

        // own sorting so later HUD elements can't draw over the badge
        Canvas canvas = badge.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = 50;

        // white key cap like the E interact popup, not clipped by the circle slot masks
        Image background = badge.AddComponent<Image>();
        background.color = Color.white;
        background.maskable = false;
        background.raycastTarget = false;

        HorizontalLayoutGroup layout = badge.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(10, 10, 4, 4);
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = badge.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        GameObject textObject = new GameObject("Key Text", typeof(RectTransform));
        textObject.transform.SetParent(badge.transform, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = 24f;
        text.fontStyle = FontStyles.Bold;
        text.color = Color.black;
        text.alignment = TextAlignmentOptions.Center;
        text.textWrappingMode = TextWrappingModes.NoWrap;
        text.maskable = false;
        text.raycastTarget = false;

        KeybindLabel label = badge.AddComponent<KeybindLabel>();
        label.keyText = text;
        label.SetKey(key);
        return label;
    }

    // empty key hides the badge (passive gear has nothing to press)
    public void SetKey(string key)
    {
        keyText.text = key;
        hasKey = !string.IsNullOrEmpty(key);
        UpdateVisible();
    }

    // also hidden while the inventory is open so it doesn't show through it
    private void UpdateVisible()
    {
        gameObject.SetActive(hasKey && !InventoryWindow.IsOpen);
    }
}
