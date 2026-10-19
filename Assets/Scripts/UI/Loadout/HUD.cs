using System;
using System.Collections;
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

    [Header("Healthbar effects")]
    [SerializeField] private Image damageFill;   // damagehp, sits behind healthFill
    [SerializeField] private Image healFill;     // healhp, sits behind healthFill
    [SerializeField] private Image restoreFill;  // fullhp copy that fades in over the healed section
    [SerializeField] private float flashHold = 0.2f; // how long the red/heal section stays solid
    [SerializeField] private float fadeTime = 0.4f;

    [Header("Status icon")]
    [SerializeField] private GameObject goodIcon;
    [SerializeField] private GameObject criticalIcon;
    [SerializeField, Range(0f, 1f)] private float criticalThreshold = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool debugKeys = true; // O = damage, P = heal
    [SerializeField] private float debugAmount = 20f;

    private Health target;
    private Coroutine damageRoutine, healRoutine;
    private bool healPending;   // next HealthChanged comes from Health.Heal
    private float healTarget;   // fill the heal animation is heading to

    //attach health to healthbar
    private void Awake()
    {
        SetAlpha(damageFill, 0f);
        SetAlpha(healFill, 0f);
        SetAlpha(restoreFill, 0f);

        inventoryButton.onClick.AddListener(() => InventoryToggled?.Invoke());
        if (playerHealth != null) Bind(playerHealth);
    }

    //toggling inventory
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            InventoryToggled?.Invoke();

        if (debugKeys && target != null)
        {
            if (Input.GetKeyDown(KeyCode.O)) target.Damage(debugAmount);
            if (Input.GetKeyDown(KeyCode.P)) target.Heal(debugAmount);
        }
    }

    //points healthbar to health component
    public void Bind(Health newTarget)
    {
        if (target != null)
        {
            target.HealthChanged -= SetHealth;
            target.Healed -= OnHealed;
        }

        target = newTarget;

        if (target != null)
        {
            target.HealthChanged += SetHealth;
            target.Healed += OnHealed;
            healthFill.fillAmount = Mathf.Clamp01(target.Current / target.Max); // no flash for the starting value
            SetHealth(target.Current, target.Max);
        }
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            target.HealthChanged -= SetHealth;
            target.Healed -= OnHealed;
        }
    }

    private void OnHealed(float amount) => healPending = true;

    // updates health bar with whatever it is
    public void SetHealth(float current, float max)
    {
        float fill = max > 0f ? Mathf.Clamp01(current / max) : 0f;
        float shown = healRoutine != null ? healTarget : healthFill.fillAmount;

        if (fill < shown)
            ShowDamage(fill);
        else if (healPending)
            ShowHeal(fill);
        else if (healRoutine != null)
            SetHealFills(fill);         // regen during a heal flash just extends it
        else
            healthFill.fillAmount = fill; // regen ticks update instantly

        healPending = false;

        bool critical = fill <= criticalThreshold;
        if (goodIcon != null) goodIcon.SetActive(!critical);
        if (criticalIcon != null) criticalIcon.SetActive(critical);

        if (healthText != null)
            //keeps health text in ints
            healthText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
    }

    private void ShowDamage(float fill)
    {
        FinishHeal();

        // Chained hits keep the red section reaching back to where the first one started
        float from = healthFill.fillAmount;
        if (damageRoutine != null) from = Mathf.Max(from, damageFill.fillAmount);

        damageFill.fillAmount = from;
        healthFill.fillAmount = fill;

        if (damageRoutine != null) StopCoroutine(damageRoutine);
        damageRoutine = StartCoroutine(DamageFlash());
    }

    private void ShowHeal(float fill)
    {
        // healthFill stays at the old value so the healed section shows healFill behind it
        if (healRoutine != null) StopCoroutine(healRoutine);
        SetHealFills(fill);
        SetAlpha(healFill, 1f);
        SetAlpha(restoreFill, 0f);
        healRoutine = StartCoroutine(HealFlash());
    }

    private void SetHealFills(float fill)
    {
        healTarget = fill;
        healFill.fillAmount = fill;
        restoreFill.fillAmount = fill;
    }

    // Snap a running heal animation to its end state
    private void FinishHeal()
    {
        if (healRoutine == null) return;
        StopCoroutine(healRoutine);
        healRoutine = null;
        healthFill.fillAmount = healTarget;
        SetAlpha(healFill, 0f);
        SetAlpha(restoreFill, 0f);
    }

    private IEnumerator DamageFlash()
    {
        SetAlpha(damageFill, 1f);
        yield return new WaitForSeconds(flashHold);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            SetAlpha(damageFill, 1f - t / fadeTime);
            yield return null;
        }

        SetAlpha(damageFill, 0f);
        damageRoutine = null;
    }

    private IEnumerator HealFlash()
    {
        yield return new WaitForSeconds(flashHold);

        // Fade the restored fullhp in over the heal sprite
        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            SetAlpha(restoreFill, t / fadeTime);
            yield return null;
        }

        healthFill.fillAmount = healTarget;
        SetAlpha(healFill, 0f);
        SetAlpha(restoreFill, 0f);
        healRoutine = null;
    }

    private static void SetAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}
