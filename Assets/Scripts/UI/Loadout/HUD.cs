using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Health playerHealth; 
    public event Action InventoryToggled;

    private Health target;

    //attach health to healthbar
    private void Awake()
    {
        inventoryButton.onClick.AddListener(() => InventoryToggled?.Invoke());
        if (playerHealth != null) Bind(playerHealth);
    }

    //toggling inventory
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InventoryToggled?.Invoke();
    }

    //points healthbar to health component
    public void Bind(Health newTarget)
    {
        if (target != null) target.HealthChanged -= SetHealth;

        target = newTarget;

        if (target != null)
        {
            target.HealthChanged += SetHealth;
            SetHealth(target.Current, target.Max);   // show the starting value
        }
    }

    private void OnDestroy()
    {
        if (target != null) target.HealthChanged -= SetHealth;
    }

    // updates health bar with whatever it is
    public void SetHealth(float current, float max)
    {
        healthFill.fillAmount = Mathf.Clamp01(current / max);
        if (healthText != null)
            //keeps health text in ints
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}