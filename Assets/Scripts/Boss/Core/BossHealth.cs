using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float regen = 1f;       // regen per second
    [SerializeField] private float regenDelay = 5f;  // delay after damage before regen starts

    public bool invincible = false;
    private float itimer = 0.0f;
    private float imax = 1.0f; // Amount of time the boss will be invincible.
    public event Action<float, float> HealthChanged;
    public float Current => health;
    public float Max => maxHP;
    private float startRegenTime = 0.0f;
    private bool needsRegen = false;
    private bool regenAllowed => Time.time > startRegenTime;

    [Header("After Defeat")]
    [SerializeField] private string returnSceneName = "HubWorld";
    [SerializeField] private float returnDelay = 5f;

    private void OnTakeDamage()
    {
        needsRegen = true;
        startRegenTime = Time.time + regenDelay;
    }

    public void Damage(float amount)
    {
        if (invincible) { return; }
        // Deal damage if not invincible.
        health -= amount;
        HealthChanged?.Invoke(Mathf.Max(health, 0f), maxHP);
        if (health <= 0)
        {
            SceneFader.LoadScene(returnSceneName, returnDelay);
            Destroy(gameObject);
            return;
        }
        // Start/restart the regen delay.
        OnTakeDamage();
        // Reset invincible timer.
        itimer = 0.0f;
        invincible = true;
    }

    private void Regenerate()
    {
        health += regen * Time.deltaTime;
        if (health >= maxHP)
        {
            health = maxHP;
            needsRegen = false;
        }
        HealthChanged?.Invoke(health, maxHP);
    }

    void Update()
    {
        // Increase timer when necessary.
        if (itimer < imax)
        {
            itimer += Time.deltaTime;
            // If timer is done, set invincible to be false.
            if (itimer >= imax) { invincible = false; }
        }

        // Regenerate after the delay has passed.
        if (needsRegen && regenAllowed)
        {
            Regenerate();
        }
    }
}
