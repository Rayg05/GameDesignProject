using UnityEngine;
using UnityEngine.UI;

public class CircleSlot : MonoBehaviour
{
    [SerializeField] private SlotType slotType;
    [SerializeField] private Image iconImage;

    private KeybindLabel keybindLabel;
    private Image cooldownOverlay; // white circle that wipes away radially as the item cools down
    private EquipmentData current;

    private void OnEnable()
    {
        // keys are only shown on the in-game side panel, not on the slots inside the inventory window
        if (keybindLabel == null && GetComponentInParent<InventoryWindow>(true) == null)
            keybindLabel = KeybindLabel.Create((RectTransform)transform, null);
        if (cooldownOverlay == null) BuildCooldownOverlay();
        //references OnLoadoutChanged to add to subscriber list
        Loadout.Changed += OnLoadoutChanged;
        Refresh(Loadout.Get(slotType)); // show the current gear
    }

    private void OnDisable()
    {
        Loadout.Changed -= OnLoadoutChanged;
    }

    //redraw when gear in this slot's type changes
    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        if (type == slotType) Refresh(data);
    }

    //shows the icon of equipment if there or not
    private void Refresh(EquipmentData data)
    {
        current = data;
        bool hasIcon = data != null && data.icon != null;
        iconImage.sprite = hasIcon ? data.icon : null;
        iconImage.preserveAspect = true;
        iconImage.enabled = hasIcon;
        if (keybindLabel != null) keybindLabel.SetKey(data != null ? data.keybind : null); // key to use whatever is equipped here
    }

    private void Update()
    {
        float fraction = ItemTimers.CooldownFraction(current);
        bool show = fraction > 0f;
        if (cooldownOverlay.enabled != show) cooldownOverlay.enabled = show;
        if (show) cooldownOverlay.fillAmount = fraction;
    }

    // sits over the icon, inside the slot's circle mask
    private void BuildCooldownOverlay()
    {
        GameObject go = new GameObject("Cooldown", typeof(RectTransform));
        go.transform.SetParent(transform, false);
        go.transform.SetSiblingIndex(iconImage.transform.GetSiblingIndex() + 1);
        RectTransform rect = (RectTransform)go.transform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = rect.offsetMax = Vector2.zero;

        cooldownOverlay = go.AddComponent<Image>();
        Image circle = GetComponent<Image>(); // the slot's own circle sprite
        if (circle != null) cooldownOverlay.sprite = circle.sprite;
        cooldownOverlay.color = new Color(1f, 1f, 1f, 0.4f);
        cooldownOverlay.type = Image.Type.Filled;
        cooldownOverlay.fillMethod = Image.FillMethod.Radial360;
        cooldownOverlay.fillOrigin = (int)Image.Origin360.Top;
        cooldownOverlay.fillClockwise = false; // shrinks clockwise from the top as fillAmount drops
        cooldownOverlay.raycastTarget = false;
        cooldownOverlay.enabled = false;
    }
}
