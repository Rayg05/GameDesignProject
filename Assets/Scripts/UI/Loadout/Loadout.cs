using System;
using UnityEngine;

public static class Loadout
{
    public static event Action<SlotType, EquipmentData> Changed;

    private static readonly EquipmentData[] equipped = new EquipmentData[3];

    public static EquipmentData Get(SlotType type) => equipped[(int)type];

    public static void Equip(SlotType type, EquipmentData data)
    {
        equipped[(int)type] = data;
        Changed?.Invoke(type, data);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetState()
    {
        Array.Clear(equipped, 0, equipped.Length);
        Changed = null;
    }
}