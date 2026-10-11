using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryEntry : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject equippedMarker;

    public void Setup(EquipmentData data, bool isEquipped, Action<EquipmentData> onClick)
    {
        icon.sprite = data.icon;
        icon.enabled = data.icon != null;

        equippedMarker.SetActive(isEquipped);

        button.interactable = true;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick(data));
    }

    public void SetEmpty()
    {
        icon.enabled = false;
        equippedMarker.SetActive(false);

        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }
}