using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryEntry : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private GameObject equippedMarker; // shown when this item is equipped

    private EquipmentData current; // null while the slot is empty

    // fills slot with an item so it can be equipped
    public void Setup(EquipmentData data, bool isEquipped, Action<EquipmentData> onClick)
    {
        current = data;
        icon.sprite = data.icon;
        icon.preserveAspect = true;
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
        current = null;
        icon.enabled = false;
        equippedMarker.SetActive(false);

        button.interactable = false;
        button.onClick.RemoveAllListeners();
    }

    // name + description box while the mouse is over an item
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (current != null) ItemTooltip.Show(current, (RectTransform)transform);
    }

    public void OnPointerExit(PointerEventData eventData) => ItemTooltip.Hide();

    // closing the inventory doesn't send a pointer exit
    private void OnDisable() => ItemTooltip.Hide();
}
