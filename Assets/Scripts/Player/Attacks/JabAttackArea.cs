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
            bossHealth.Damage(damage);
            return;
        }

        Health health = collider.GetComponent<Health>();
        if (health != null)
        {
            health.Damage(damage);
            return;
        }
    }
}