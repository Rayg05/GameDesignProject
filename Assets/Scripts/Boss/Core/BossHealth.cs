using System;
using UnityEngine;

// Boss health. Unlike Health.cs it doesn't destroy the object on death. It fires events
// and BossController decides what happens (death animation, phase changes, health bar, etc).
public class BossHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invulnerabilityAfterHit = 0f; // i-frames, 0 = none

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public float Normalized => maxHealth > 0 ? (float)CurrentHealth / maxHealth : 0f;
    public bool IsDead => CurrentHealth <= 0;

    // Set by states/attacks for scripted invulnerability (intro, phase transition, etc)
    public bool Invulnerable { get; set; }

    public event Action<int> Damaged; // damage amount
    public event Action<int> Healed;  // heal amount
    public event Action Died;

    private float invulnerableUntil;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    // Returns true if the damage was applied
    public bool TakeDamage(int amount)
    {
        if (amount <= 0 || IsDead || Invulnerable || Time.time < invulnerableUntil) return false;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
        invulnerableUntil = Time.time + invulnerabilityAfterHit;

        Damaged?.Invoke(amount);
        if (IsDead) Died?.Invoke();
        return true;
    }

    public void Heal(int amount)
    {
        if (amount <= 0 || IsDead) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        Healed?.Invoke(amount);
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }
}
