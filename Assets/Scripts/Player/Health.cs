using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHP = 100f;
    [SerializeField] private float regen = 1f;       // base regen per second
    [SerializeField] private float regenDelay = 5f;  // delay after damage before regen starts
    [SerializeField] public float damageMult = 1f; //editable damage multiplier so that incoming damage can be edited

    [Header("On Death")]
    [SerializeField] private bool returnToHubOnDeath = true; // same fade back to the hub as beating a boss
    [SerializeField] private string returnSceneName = "HubWorld";
    [SerializeField] private float returnDelay = 2f;

    public event Action<float, float> HealthChanged;
    public event Action<float> Healed; // fired before HealthChanged for discrete heals (not regen)
    public float Current => health;
    public float Max => maxHP;

    private float regenMultiplier = 1f;
    private float startRegenTime = 0.0f;
    private bool needsRegen = false;

    private bool regenAllowed => Time.time > startRegenTime;
    [SerializeField] private float deathAnimTime = 30f; // how long to keep the object alive for the death animation
    private bool dead;

    public void SetRegenMultiplier(float multiplier)
    {
        regenMultiplier = multiplier;
    }

    private void OnTakeDamage()
    {
        needsRegen = true;
        startRegenTime = Time.time + regenDelay;
    }

    public void Damage(float amount)
    {
        if (dead) return;

        health -= amount * damageMult;
        HealthChanged?.Invoke(Mathf.Max(health, 0f), maxHP);

        if (health <= 0)
        {
            Die();
            return;
        }
        OnTakeDamage();
    }

    private void Die()
    {
        
        dead = true;
        needsRegen = false;

        if (returnToHubOnDeath) SceneFader.LoadScene(returnSceneName, returnDelay);

        Animator animator = GetComponent<Animator>();
        bool hasDeadParam = false;
        if (animator != null)
        {
            foreach (AnimatorControllerParameter p in animator.parameters)
            {
                
                if (p.name == "Dead") hasDeadParam = true;
            }
           
        }

        if (!hasDeadParam)
        {
            Destroy(gameObject);
            return;
        }

        animator.SetBool("Dead", true);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        foreach (MonoBehaviour script in GetComponentsInChildren<MonoBehaviour>())
            if (script != this) script.enabled = false;

        Destroy(gameObject, deathAnimTime);
    }

    public void Heal(float amount)
    {
        if (amount <= 0f || health >= maxHP) return;

        float before = health;
        health = Mathf.Min(health + amount, maxHP);
        if (health >= maxHP) needsRegen = false;

        Healed?.Invoke(health - before);
        HealthChanged?.Invoke(health, maxHP);
    }

    private void Regenerate()
    {
        health += regen * regenMultiplier * Time.deltaTime;

        if (health >= maxHP)
        {
            health = maxHP;
            needsRegen = false;
        }

        HealthChanged?.Invoke(health, maxHP);
    }

    void Update()
    {
        if (needsRegen && regenAllowed)
        {
            Regenerate();
        }
    }
}