using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEntry : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject equippedMarker; // shown when this item is equipped

    // fills slot with an item so it can be equipped
    public void Setup(EquipmentData data, bool isEquipped, Action<EquipmentData> onClick)
    {
        icon.sprite = data.icon;
        icon.enabled = data.icon != null;

        equippedMarker.SetActive(isEquipped);

        //clear old listeners so slots reused across refreshes don't stack up clicks
        button.interactable = true;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data));
    }

    // if theres no item set it to empty 
    public void SetEmpty()
    {
        icon.enabled = false;
        equippedMarker.SetActive(false);

        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }
}