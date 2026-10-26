using UnityEngine;

public enum SlotType { Weapon, Item, Ability }

[CreateAssetMenu(menuName = "Game/Equipment")]
public class EquipmentData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public SlotType slotType;
    public string keybind; // shown on the HUD slot, e.g. "Q" or "L-Click"; leave empty for passive gear
}