using UnityEngine;

// Optional: damages the player when they touch the boss. Put it on the boss or on any child
// with a collider (e.g. a spiky body part). Works with both solid and trigger colliders.
public class BossContactDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float hitCooldown = 1f;

    private BossController boss;
    private float nextHitTime;

    private void Awake()
    {
        boss = GetComponentInParent<BossController>();
    }

    private void OnCollisionStay2D(Collision2D collision) => TryDamage(collision.collider);
    private void OnTriggerStay2D(Collider2D other) => TryDamage(other);

    private void TryDamage(Collider2D other)
    {
        if (!enabled || Time.time < nextHitTime) return;
        if (boss != null && (boss.IsDead || !boss.IsFightActive)) return;

        Health health = other.GetComponentInParent<Health>();
        if (health == null) return;

        health.Damage(damage);
        nextHitTime = Time.time + hitCooldown;
    }
}
