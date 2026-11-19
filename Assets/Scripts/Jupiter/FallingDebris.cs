using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FallingDebris : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float lifetimeAfterLanding = 1.5f;
    bool landed;
    void Start()
    {
        Destroy(gameObject, 10f);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (landed) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            // deal damage here
            landed = true;
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            landed = true;
            Destroy(gameObject, lifetimeAfterLanding);
        }
    }
}