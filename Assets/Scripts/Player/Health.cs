using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float health = 1000f;
    [SerializeField] private float maxHP = 1000f;
    [SerializeField] private float regen = 1f;       // base regen per second
    [SerializeField] private float regenDelay = 5f;  // delay after damage before regen starts

    private float regenMultiplier = 1f;
    private float startRegenTime = 0.0f;
    private bool needsRegen = false;

    private bool regenAllowed => Time.time > startRegenTime;

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
        health -= amount;
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        OnTakeDamage();
    }

    private void Regenerate()
    {
        health += regen * regenMultiplier * Time.deltaTime;

        if (health >= maxHP)
        {
            health = maxHP;
            needsRegen = false;
        }
    }

    void Update()
    {
        if (needsRegen && regenAllowed)
        {
            Regenerate();
        }
    }
}