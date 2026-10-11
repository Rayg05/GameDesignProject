using UnityEngine;

public class DoubleRegen : MonoBehaviour
{
    [SerializeField] private float multiplier = 2f;
    [SerializeField] private EquipmentData ability;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        Loadout.Changed += OnLoadoutChanged;
        Apply();   // covers the case where it's already equipped on load
    }

    void OnDisable()
    {
        Loadout.Changed -= OnLoadoutChanged;
        if (health != null) health.SetRegenMultiplier(1f);
    }

    private void OnLoadoutChanged(SlotType type, EquipmentData data)
    {
        if (ability != null && type == ability.slotType) Apply();
    }

    private void Apply()
    {
        health.SetRegenMultiplier(Loadout.IsEquipped(ability) ? multiplier : 1f);
    }
}