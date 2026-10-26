using System;
using System.Collections.Generic;
using UnityEngine;

// Builds the inventory screen, opens/closes it, and equips gear on click
public class InventoryWindow : MonoBehaviour
{
    private const int SlotsPerSection = 6;

    [SerializeField] private HUD hud; //hud
    [SerializeField] private GameObject panel;//actual inventory panel
    [SerializeField] private InventoryEntry entryPrefab; //inventory prefab
    [SerializeField] private Transform[] sectionSlots;   //holds the 3 rows of weapons/abilities/items
    [SerializeField] private List<EquipmentData> ownedEquipment; //holds equipment that can be used to trigger what is active

    private InventoryEntry[][] entries;
    private bool isOpen;

    // HUD key prompts listen to this so they hide behind the open inventory
    public static bool IsOpen { get; private set; }
    public static event Action<bool> OpenChanged;

    private void Awake()
    {
        // create all inventory slots thatll hold equipment 
        entries = new InventoryEntry[sectionSlots.Length][];
        for (int s = 0; s < sectionSlots.Length; s++)
        {
            entries[s] = new InventoryEntry[SlotsPerSection];
            for (int i = 0; i < SlotsPerSection; i++)
                entries[s][i] = Instantiate(entryPrefab, sectionSlots[s], false);
        }

        SetOpen(false);
    }

    //too lazy for comment, infer below
    private void OnEnable()
    {
        hud.InventoryToggled += Toggle;
        Loadout.Changed += OnLoadoutChanged;
    }

    private void OnDisable()
    {
        hud.InventoryToggled -= Toggle;
        Loadout.Changed -= OnLoadoutChanged;
        if (isOpen)
        {
            Time.timeScale = 1f;
            SetStaticOpen(false);
        }
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) SetOpen(false);
    }

    private void Toggle() => SetOpen(!isOpen);

    //shows or doesnt show the inventory and pauses game while its open
    //TODO: change so this can only be opened in the hub not during fights
    private void SetOpen(bool open)
    {
        isOpen = open;
        panel.SetActive(open);
        Time.timeScale = open ? 0f : 1f;
        SetStaticOpen(open);
        if (open) Refresh();
    }

    private static void SetStaticOpen(bool open)
    {
        if (IsOpen == open) return;
        IsOpen = open;
        OpenChanged?.Invoke(open);
    }

    // fills each row according to what kind of equipment it is and leaves whatever left as blank
    private void Refresh()
    {
        for (int s = 0; s < entries.Length; s++)
        {
            SlotType type = (SlotType)s;
            EquipmentData equipped = Loadout.Get(type);

            var owned = new List<EquipmentData>();
            foreach (EquipmentData data in ownedEquipment)
                if (data != null && data.slotType == type) owned.Add(data);

            for (int i = 0; i < SlotsPerSection; i++)
            {
                if (i < owned.Count)
                    entries[s][i].Setup(owned[i], owned[i] == equipped, OnEntryClicked);
                else
                    entries[s][i].SetEmpty();
            }
        }
    }

    // equips or unequips specific weapons/abilites/items
    private void OnEntryClicked(EquipmentData data)
    {
        Debug.Log($"Clicked {data.displayName} ({data.slotType})");
        bool alreadyEquipped = Loadout.Get(data.slotType) == data;
        Loadout.Equip(data.slotType, alreadyEquipped ? null : data);
    }

    //updates markers 
    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        if (isOpen) Refresh();
    }
}