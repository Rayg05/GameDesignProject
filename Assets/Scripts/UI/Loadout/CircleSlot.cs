using UnityEngine;
using UnityEngine.UI;

public class CircleSlot : MonoBehaviour
{
    [SerializeField] private SlotType slotType;
    [SerializeField] private Image iconImage;

    private void OnEnable()
    {
        Loadout.Changed += OnLoadoutChanged;
        Refresh(Loadout.Get(slotType));
    }

    private void OnDisable()
    {
        Loadout.Changed -= OnLoadoutChanged;
    }

    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        if (type == slotType) Refresh(data);
    }

    private void Refresh(EquipmentData data)
    {
        bool hasIcon = data != null && data.icon != null;
        iconImage.sprite = hasIcon ? data.icon : null;
        iconImage.enabled = hasIcon;
    }
}