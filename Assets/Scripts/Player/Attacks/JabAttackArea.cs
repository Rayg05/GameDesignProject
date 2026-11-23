using UnityEngine;

public class JabAttackArea : MonoBehaviour
{
    [SerializeField] public int damage = 3;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        IDamageable target = collider.GetComponentInParent<IDamageable>();
        if (target != null)
        {
            target.Damage(damage + DamageModifiers.FlatBonus);
            return;
        }

        Health health = collider.GetComponent<Health>();
        if (health != null)
        {
            health.Damage(damage + DamageModifiers.FlatBonus);
            return;
        }
    }
}