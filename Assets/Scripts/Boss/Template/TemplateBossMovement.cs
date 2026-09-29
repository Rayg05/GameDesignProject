using UnityEngine;

// TEMPLATE: movement for one boss. Copy, rename, and replace MoveTowards with the boss's
// real movement (hopping, flying, dashing, teleporting...).
public class TemplateBossMovement : BossMovement
{
    [SerializeField] private float stoppingDistance = 1f;

    public override void MoveTowards(Vector2 destination)
    {
        if (rb == null) return;

        // TODO: placeholder walks straight left/right toward the destination
        float dx = destination.x - rb.position.x;
        if (Mathf.Abs(dx) <= stoppingDistance)
        {
            Stop();
            return;
        }

        rb.linearVelocity = new Vector2(Mathf.Sign(dx) * moveSpeed, rb.linearVelocity.y);
    }

    // TODO (optional): override Stop() for flying bosses so vertical velocity is zeroed too
}
