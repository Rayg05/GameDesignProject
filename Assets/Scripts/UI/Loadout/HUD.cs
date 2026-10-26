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

    [Header("Strength buff")]
    [SerializeField] private Color strengthGlowColor = new Color(1f, 0.55f, 0f, 1f); // orange
    [SerializeField] private float glowPulseSpeed = 4f;

    [Header("Debug")]
    [SerializeField] private bool debugKeys = true; // O = damage, P = heal
    [SerializeField] private float debugAmount = 20f;

    private Health target;
    private Coroutine damageRoutine, healRoutine;
    private bool healPending;   // next HealthChanged comes from Health.Heal
    private float healTarget;   // fill the heal animation is heading to

    // orange glow layers behind the bar + seconds left, while the strength item is active
    private static readonly float[] GlowPadding = { 6f, 12f, 20f };
    private static readonly float[] GlowAlpha = { 0.7f, 0.4f, 0.18f };
    private Image[] glowLayers;
    private TextMeshProUGUI strengthTimerText;
    private bool timerOutlineSet;

    //attach health to healthbar
    private void Awake()
    {
        SetAlpha(damageFill, 0f);
        SetAlpha(healFill, 0f);
        SetAlpha(restoreFill, 0f);

        inventoryButton.onClick.AddListener(() => InventoryToggled?.Invoke());
        KeybindLabel.Create((RectTransform)inventoryButton.transform, "I");
        if (playerHealth != null) Bind(playerHealth);
        BuildStrengthGlow(); // last, so the core HUD is set up even if this fails
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

        UpdateStrengthGlow();
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

    // Copies of the bar's outline drawn as solid orange, each a bit bigger and fainter = soft glow
    private void BuildStrengthGlow()
    {
        RectTransform bar = (RectTransform)healthFill.transform.parent;
        Transform emptyBar = bar.Find("EmptyBar");
        Image source = emptyBar != null ? emptyBar.GetComponent<Image>() : healthFill;

        Shader shader = Shader.Find("Custom/UISilhouette");
        Material silhouette = shader != null ? new Material(shader) : null;

        glowLayers = new Image[GlowPadding.Length];
        for (int i = 0; i < GlowPadding.Length; i++)
        {
            GameObject go = new GameObject("StrengthGlow" + i, typeof(RectTransform));
            go.transform.SetParent(bar, false);
            go.transform.SetAsFirstSibling(); // behind the bar, biggest layer furthest back
            RectTransform rect = (RectTransform)go.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(-GlowPadding[i], -GlowPadding[i]);
            rect.offsetMax = new Vector2(GlowPadding[i], GlowPadding[i]);

            Image glow = go.AddComponent<Image>();
            glow.sprite = source.sprite;
            glow.material = silhouette;
            glow.raycastTarget = false;
            glowLayers[i] = glow;
            go.SetActive(false);
        }

        GameObject textObject = new GameObject("StrengthTimer", typeof(RectTransform));
        textObject.transform.SetParent(bar, false);
        RectTransform textRect = (RectTransform)textObject.transform;
        textRect.anchorMin = textRect.anchorMax = new Vector2(1f, 0.5f);
        textRect.pivot = new Vector2(0f, 0.5f);
        textRect.anchoredPosition = new Vector2(GlowPadding[GlowPadding.Length - 1] + 8f, 0f);
        textRect.sizeDelta = new Vector2(120f, 60f);
        strengthTimerText = textObject.AddComponent<TextMeshProUGUI>();
        strengthTimerText.fontSize = 40f;
        strengthTimerText.fontStyle = FontStyles.Bold;
        strengthTimerText.color = strengthGlowColor;
        strengthTimerText.alignment = TextAlignmentOptions.Left;
        strengthTimerText.textWrappingMode = TextWrappingModes.NoWrap;
        strengthTimerText.raycastTarget = false;
        textObject.SetActive(false);
    }

    private void UpdateStrengthGlow()
    {
        float remaining = ItemTimers.ActiveRemaining(StrengthItem.Data);
        bool active = remaining > 0f;

        if (strengthTimerText.gameObject.activeSelf != active) strengthTimerText.gameObject.SetActive(active);
        foreach (Image glow in glowLayers)
            if (glow.gameObject.activeSelf != active) glow.gameObject.SetActive(active);
        if (!active) return;

        if (!timerOutlineSet && strengthTimerText.gameObject.activeInHierarchy)
        {
            // TMP's material only exists once the text is active (the healthbar is hidden in the hub),
            // so the outline is set the first time it shows instead of in BuildStrengthGlow
            strengthTimerText.outlineWidth = 0.25f;
            strengthTimerText.outlineColor = Color.black;
            timerOutlineSet = true;
        }

        strengthTimerText.text = Mathf.CeilToInt(remaining) + "s";

        // gentle pulse so it reads as a buff rather than part of the bar
        float pulse = Mathf.Lerp(0.6f, 1f, (Mathf.Sin(Time.unscaledTime * glowPulseSpeed) + 1f) * 0.5f);
        for (int i = 0; i < glowLayers.Length; i++)
        {
            Color c = strengthGlowColor;
            c.a = GlowAlpha[i] * pulse;
            glowLayers[i].color = c;
        }
    }

    private static void SetAlpha(Image image, float alpha)
    {
        if (image == null) return;
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}
