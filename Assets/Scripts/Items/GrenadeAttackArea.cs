using UnityEngine;

public class GrenadeAttackArea : MonoBehaviour
{
    [SerializeField] public int damage = 12;
    [SerializeField] private float radius = 15f;
    [SerializeField, Range(0f, 1f)] private float minDamagePercent = 0.2f;
    [SerializeField] private LayerMask hitLayers = ~0;
    [SerializeField] private Sprite explosionSprite;
    [SerializeField] private float explosionScale = 1f;
    [SerializeField] private float explosionDuration = 0.3f;
    private bool exploded;

    public void StartFuse(float seconds)
    {
        Invoke(nameof(Explode), seconds);
    }

    public void Detonate()
    {
        CancelInvoke(nameof(Explode));
        Explode();
    }

    private void Explode()
    {
        if (exploded) return;
        exploded = true;

        foreach (Collider2D hit in Physics2D.OverlapCircleAll(transform.position, radius, hitLayers))
        {
            BossHealth boss = hit.GetComponentInParent<BossHealth>();
            if (boss == null) continue;

            // 0 at the grenade, 1 at the edge of the blast
            float distance = Vector2.Distance(transform.position, hit.ClosestPoint(transform.position));
            float t = Mathf.Clamp01(distance / radius);

            boss.Damage(Mathf.RoundToInt(damage * Mathf.Lerp(1f, minDamagePercent, t)));
        }

        ShowExplosion();
        Destroy(gameObject, explosionDuration);
    }

    private void ShowExplosion()
    {
        // freeze the grenade in place so the explosion stays where it blew up
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        // upright, so the explosion doesn't inherit the grenade's spin
        transform.rotation = Quaternion.identity;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && explosionSprite != null)
        {
            sr.sprite = explosionSprite;
            transform.localScale = Vector3.one * explosionScale;
        }
    }
}