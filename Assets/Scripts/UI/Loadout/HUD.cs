using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Image healthFill;
    [SerializeField] private TMP_Text healthText;   // optional
    [SerializeField] private Button inventoryButton;
    [SerializeField] private Health playerHealth;   // optional, see below

    public event Action InventoryToggled;

    private Health target;

    private void Awake()
    {
        inventoryButton.onClick.AddListener(() => InventoryToggled?.Invoke());
        if (playerHealth != null) Bind(playerHealth);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InventoryToggled?.Invoke();
    }

    public void Bind(Health newTarget)
    {
        if (target != null) target.HealthChanged -= SetHealth;

        target = newTarget;

        if (target != null)
        {
            target.HealthChanged += SetHealth;
            SetHealth(target.Current, target.Max);   // show the starting value immediately
        }
    }

    private void OnDestroy()
    {
        if (target != null) target.HealthChanged -= SetHealth;
    }

    public void SetHealth(float current, float max)
    {
        healthFill.fillAmount = Mathf.Clamp01(current / max);
        if (healthText != null)
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }
}