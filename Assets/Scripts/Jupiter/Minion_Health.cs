using System;
using UnityEngine;

public class Minion_Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float health = 10f;
    [SerializeField] private float maxHP = 10f;

    public bool invincible = false;
    private float itimer = 0.0f;
    private float imax = .1f; 
    public event Action<float, float> HealthChanged;
    public event Action<float> Damaged;
    public float Current => health;
    public float Max => maxHP;

    private void Awake()
    {
        // Hit flash + damage numbers
        if (GetComponent<BossHitFeedback>() == null) gameObject.AddComponent<BossHitFeedback>();
    }

    public void Damage(float amount)
    {
        if (invincible) { return; }
        // Deal damage if not invincible.
        health -= amount;
        HealthChanged?.Invoke(Mathf.Max(health, 0f), maxHP);
        Damaged?.Invoke(amount);
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }
        // Reset invincible timer.
        itimer = 0.0f;
        invincible = true;
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
    }
}