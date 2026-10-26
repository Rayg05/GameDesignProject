using System;
using UnityEngine;

public static class Loadout
{
    // used for an item/ability/weapon that is true if its equipped
    public static bool IsEquipped(EquipmentData data) => data != null && Get(data.slotType) == data;

    //flag used whenever something changes
    public static event Action<SlotType, EquipmentData> Changed;

    //holds current equipment loadout 
    private static readonly EquipmentData[] equipped = new EquipmentData[3];

    public static EquipmentData Get(SlotType type) => equipped[(int)type];

    //true if at least one slot has something in it
    public static bool HasAnyEquipped()
    {
        foreach (EquipmentData data in equipped)
            if (data != null) return true;
        return false;
    }

    //equipes something to its slot
    public static void Equip(SlotType type, EquipmentData data)
    {
        equipped[(int)type] = data;
        Changed?.Invoke(type, data);
    }

}