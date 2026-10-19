using UnityEngine;

public class BasicProjectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 1f;

    private float timer = 0f;
    private float duration = 2f;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Shoot(Vector2 dir) {
        rb.AddForce(dir * speed, ForceMode2D.Impulse);
    }

    void Update() {
        timer += Time.deltaTime;
        if(timer >= duration) {
            Destroy(gameObject);
        }
    }
}
