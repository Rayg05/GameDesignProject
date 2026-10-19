using UnityEngine;
using UnityEngine.UI;

public class CircleSlot : MonoBehaviour
{
    [SerializeField] private SlotType slotType;
    [SerializeField] private Image iconImage;

    private void OnEnable()
    {
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
    }
}