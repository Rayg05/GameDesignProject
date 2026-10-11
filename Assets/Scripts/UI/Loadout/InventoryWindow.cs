using System.Collections.Generic;
using UnityEngine;

public class InventoryWindow : MonoBehaviour
{
    private const int SlotsPerSection = 6;

    [SerializeField] private HUD hud;
    [SerializeField] private GameObject panel;           // the Content object
    [SerializeField] private InventoryEntry entryPrefab;
    [SerializeField] private Transform[] sectionSlots;   // Weapons, Items, Abilities (the "Slots" objects)
    [SerializeField] private List<EquipmentData> ownedEquipment;

    private InventoryEntry[][] entries;
    private bool isOpen;

    private void Awake()
    {
        entries = new InventoryEntry[sectionSlots.Length][];
        for (int s = 0; s < sectionSlots.Length; s++)
        {
            entries[s] = new InventoryEntry[SlotsPerSection];
            for (int i = 0; i < SlotsPerSection; i++)
                entries[s][i] = Instantiate(entryPrefab, sectionSlots[s]);
        }

        SetOpen(false);
    }

    private void OnEnable()
    {
        hud.InventoryToggled += Toggle;
        Loadout.Changed += OnLoadoutChanged;
    }

    private void OnDisable()
    {
        hud.InventoryToggled -= Toggle;
        Loadout.Changed -= OnLoadoutChanged;
        if (isOpen) Time.timeScale = 1f;
    }

    private void Update()
    {
        if (isOpen && Input.GetKeyDown(KeyCode.Escape)) SetOpen(false);
    }

    private void Toggle() => SetOpen(!isOpen);

    private void SetOpen(bool open)
    {
        isOpen = open;
        panel.SetActive(open);
        Time.timeScale = open ? 0f : 1f;
        if (open) Refresh();
    }

    private void Refresh()
    {
        for (int s = 0; s < entries.Length; s++)
        {
            SlotType type = (SlotType)s;
            EquipmentData equipped = Loadout.Get(type);

            // Gather what the player owns for this section
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

    private void OnEntryClicked(EquipmentData data)
    {
        // Clicking the equipped entry again unequips it
        bool alreadyEquipped = Loadout.Get(data.slotType) == data;
        Loadout.Equip(data.slotType, alreadyEquipped ? null : data);
    }

    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        if (isOpen) Refresh();   // update the equipped markers
    }
}