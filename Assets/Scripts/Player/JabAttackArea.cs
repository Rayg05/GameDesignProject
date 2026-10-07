using UnityEngine;

public class JabAttackArea : MonoBehaviour
{
    [SerializeField] public int damage = 3;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Bosses use BossHealth instead of Health
        BossHealth bossHealth = collider.GetComponentInParent<BossHealth>();
        if (bossHealth != null)
        {
            bossHealth.TakeDamage(damage);
            return;
        }

        Health health = collider.GetComponentInParent<Health>();
        if (health != null)
        {
            health.Damage(damage);
        }
    }
}
