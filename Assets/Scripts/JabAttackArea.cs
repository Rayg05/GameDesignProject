using UnityEngine;

public class JabAttackArea : MonoBehaviour
{
    private int damage = 3;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Trigger entered with: " + collider.gameObject.name);

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
            Debug.Log("Found Health component, applying damage");
            health.Damage(damage);
        }
    }
}
