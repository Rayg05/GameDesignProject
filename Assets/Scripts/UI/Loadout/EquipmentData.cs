using UnityEngine;

public enum SlotType { Weapon, Item, Ability }

[CreateAssetMenu(menuName = "Game/Equipment")]
public class EquipmentData : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public SlotType slotType;
}