using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;

public class Hat_Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 30f;
    private string wallTag = "Wall";
    private float timer = 0f;
    private int bounces = 5;

    [SerializeField] Transform plutoPos;

   
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        Shoot(new Vector2(2, 0));


    }
     public void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D wall = collision.GetComponent<Rigidbody2D>();

        if(!collision.CompareTag(wallTag)) return;

        bounces--;

        if(wall != null)
        {
            if(bounces % 2 == 0)
            {
                Shoot((plutoPos.position - transform.position).normalized);
            }
            else
            {
                Shoot(new Vector2(1f, 0.8f).normalized);
            }
        }

        
    }

    public void Shoot(Vector2 dir) {
        rb.linearVelocity = dir * speed;
    }

    void Update() {
        if(bounces < 0)
        {
            Destroy(gameObject);
        }
    }
}
