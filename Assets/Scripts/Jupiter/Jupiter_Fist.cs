using UnityEngine;

// Jupiter's homing fist. Spawned by Jupiter_FistAttack.
// It flies horizontally toward the player and keeps steering toward them, so if the player dodges
// it turns around and comes back. It also drifts up or down toward the player's height, but only
// slowly (verticalTrackSpeed), so the player can jump over it or duck under it.
// Damage comes from the BossDamage component, which Player_Hurtbox reads on contact.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Jupiter_Fist : MonoBehaviour
{
    [HideInInspector] public Transform target;     // set by the attack when it spawns the fist

    [Header("Movement")]
    public float speed = 15f;               // top horizontal speed
    public float acceleration = 25f;        // how quickly it turns around after passing the player
    public float verticalTrackSpeed = 3f;   // how fast it follows the player's height (0 = flat line)
    public float lifetime = 4f;             // seconds before the fist disappears

    Rigidbody2D rb;
    SpriteRenderer sr;
    float velocityX;
    float timer;

    public bool Finished => timer >= lifetime;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        GetComponent<Collider2D>().isTrigger = true;
        sr = GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite == null) UsePlaceholderSprite();
    }

    // No fist art yet: draw a solid box the size of the hitbox so the fist is visible.
    // Assign a sprite on the prefab's SpriteRenderer and this is skipped.
    void UsePlaceholderSprite() {
        const int ppu = 8;
        // Local size, so it scales with the transform the same way the collider does.
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        Vector2 size = box != null ? box.size : Vector2.one;
        int w = Mathf.Max(1, Mathf.RoundToInt(size.x * ppu));
        int h = Mathf.Max(1, Mathf.RoundToInt(size.y * ppu));
        Texture2D tex = new Texture2D(w, h);
        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        sr.sprite = Sprite.Create(tex, new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), ppu);
    }

    // Called by the attack right after spawning. Starts the fist already moving toward the player.
    public void Launch(Transform player, float damage) {
        target = player;
        BossDamage.Apply(gameObject, damage);
        if (target != null) velocityX = Mathf.Sign(target.position.x - transform.position.x) * speed;
        FaceVelocity();
    }

    void FixedUpdate() {
        timer += Time.fixedDeltaTime;
        if (Finished) { Destroy(gameObject); return; }

        Vector2 pos = rb.position;
        if (target != null) {
            // Horizontal homing: accelerate toward the player's side, capped at `speed`.
            float desiredX = Mathf.Sign(target.position.x - pos.x) * speed;
            velocityX = Mathf.MoveTowards(velocityX, desiredX, acceleration * Time.fixedDeltaTime);

            // Slow vertical correction toward the player's height.
            pos.y = Mathf.MoveTowards(pos.y, target.position.y, verticalTrackSpeed * Time.fixedDeltaTime);
        }
        pos.x += velocityX * Time.fixedDeltaTime;
        rb.MovePosition(pos);
        FaceVelocity();
    }

    // Flip the sprite so the knuckles point the way it's moving (art is assumed to face right).
    void FaceVelocity() {
        if (sr != null && Mathf.Abs(velocityX) > 0.01f) sr.flipX = velocityX < 0f;
    }
}
