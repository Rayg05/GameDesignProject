using UnityEngine;

// Base class for boss movement. Each boss gets its own subclass (walker, flyer, teleporter...)
// and puts it on the same GameObject as the BossController.
public abstract class BossMovement : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 3f;

    protected BossController boss;
    protected Rigidbody2D rb;

    // Public so phases / attacks can change it (e.g. faster in phase 2)
    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public virtual void Initialize(BossController owner)
    {
        boss = owner;
        rb = GetComponent<Rigidbody2D>();
    }

    // Called every FixedUpdate while the boss wants to move toward a point
    public abstract void MoveTowards(Vector2 destination);

    // Default stops horizontal movement only, so gravity keeps working. Override for flying bosses.
    public virtual void Stop()
    {
        if (rb != null)
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    // Flips the boss to face a point, same way Basic_Movement flips the player
    public virtual void FaceTowards(Vector2 point)
    {
        float dx = point.x - transform.position.x;
        if (Mathf.Abs(dx) < 0.01f) return;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(dx);
        transform.localScale = scale;
    }
}
