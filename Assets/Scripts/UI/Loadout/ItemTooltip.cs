using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Box with an item's name and a smaller description under it, shown while hovering the item in the inventory.
// Built in code the first time it's needed, one shared box per canvas.
public class ItemTooltip : MonoBehaviour
{
    private const float Width = 380f;
    private const float Gap = 12f; // space between the slot and the box

    private static ItemTooltip instance;

    private RectTransform rect;
    private TextMeshProUGUI nameText, descriptionText;

    public static void Show(EquipmentData data, RectTransform slot)
    {
        // the canvas is destroyed on scene change, which also clears instance
        if (instance == null) instance = Create(slot.GetComponentInParent<Canvas>().rootCanvas);
        instance.Display(data, slot);
    }

    public static void Hide()
    {
        if (instance != null) instance.gameObject.SetActive(false);
    }

    private void Display(EquipmentData data, RectTransform slot)
    {
        nameText.text = string.IsNullOrEmpty(data.displayName) ? data.name : data.displayName;
        descriptionText.text = data.description;
        descriptionText.gameObject.SetActive(!string.IsNullOrEmpty(data.description));

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rect); // so the height is right before placing it

        // centered above the slot, or below it if it would go off the top of the screen
        RectTransform canvasRect = (RectTransform)rect.parent;
        Vector3[] corners = new Vector3[4];
        slot.GetWorldCorners(corners); // bottom-left, top-left, top-right, bottom-right
        Vector2 slotTop = canvasRect.InverseTransformPoint((corners[1] + corners[2]) * 0.5f);
        Vector2 slotBottom = canvasRect.InverseTransformPoint((corners[0] + corners[3]) * 0.5f);

        bool above = slotTop.y + Gap + rect.rect.height <= canvasRect.rect.yMax;
        rect.pivot = new Vector2(0.5f, above ? 0f : 1f);
        Vector2 position = above ? slotTop + Vector2.up * Gap : slotBottom + Vector2.down * Gap;

        // keep it on screen at the sides
        float halfWidth = Width * 0.5f;
        position.x = Mathf.Clamp(position.x, canvasRect.rect.xMin + halfWidth, canvasRect.rect.xMax - halfWidth);
        rect.localPosition = position;
    }

    private static ItemTooltip Create(Canvas canvas)
    {
        GameObject box = new GameObject("ItemTooltip", typeof(RectTransform));
        box.transform.SetParent(canvas.transform, false);
        RectTransform rect = (RectTransform)box.transform;
        rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(Width, 0f);

        // own sorting so it draws over the inventory panel
        Canvas boxCanvas = box.AddComponent<Canvas>();
        boxCanvas.overrideSorting = true;
        boxCanvas.sortingOrder = 100;

        // no raycasts anywhere on the box, or it would steal the hover from the slot under it and flicker
        Image background = box.AddComponent<Image>();
        background.color = new Color(0.08f, 0.08f, 0.12f, 0.92f);
        background.raycastTarget = false;

        // fixed width, height grows with the description
        VerticalLayoutGroup layout = box.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 12, 14);
        layout.spacing = 4f;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
        ContentSizeFitter fitter = box.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        ItemTooltip tooltip = box.AddComponent<ItemTooltip>();
        tooltip.rect = rect;
        tooltip.nameText = CreateText(box.transform, "Name", 30f, FontStyles.Bold, Color.white);
        tooltip.descriptionText = CreateText(box.transform, "Description", 22f, FontStyles.Normal, new Color(0.8f, 0.8f, 0.85f));
        return tooltip;
    }

    private static TextMeshProUGUI CreateText(Transform parent, string name, float size, FontStyles style, Color color)
    {
        GameObject textObject = new GameObject(name, typeof(RectTransform));
        textObject.transform.SetParent(parent, false);
        TextMeshProUGUI text = textObject.AddComponent<TextMeshProUGUI>();
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = TextAlignmentOptions.Left;
        text.textWrappingMode = TextWrappingModes.Normal;
        text.raycastTarget = false;
        return text;
    }
}
