using UnityEngine;
using UnityEngine.UI;

public class CircleSlot : MonoBehaviour
{
    [SerializeField] private SlotType slotType;
    [SerializeField] private Image iconImage;

    private KeybindLabel keybindLabel;

    private void OnEnable()
    {
        // keys are only shown on the in-game side panel, not on the slots inside the inventory window
        if (keybindLabel == null && GetComponentInParent<InventoryWindow>(true) == null)
            keybindLabel = KeybindLabel.Create((RectTransform)transform, null);
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
        bool hasIcon = data != null && data.icon != null;
        iconImage.sprite = hasIcon ? data.icon : null;
        iconImage.preserveAspect = true;
        iconImage.enabled = hasIcon;
        if (keybindLabel != null) keybindLabel.SetKey(data != null ? data.keybind : null); // key to use whatever is equipped here
    }
}