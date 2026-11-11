using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;

public class Hat_Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 30f;
    private string wallTag = "Wall";
    private float timer = 0f;
    private int bounces = 8;

    [SerializeField] Transform plutoPos;

   
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        Shoot(new Vector2(2, 0));


    }

    void OnCollisionEnter2D(Collision2D collision)
    {


        //if(!collision.CompareTag(wallTag)) return;

        bounces--;


            if(bounces % 2 == 0)   //every other bounce goes towards the player so it doesnt bounce off randomly and get it stuck somewhere
            {
                Shoot((plutoPos.position - transform.position).normalized);
            }
            else
            {
                Vector2 incoming =  -collision.relativeVelocity.normalized;

               Shoot(Vector2.Reflect(incoming, collision.contacts[0].normal));
                
            }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
       
        
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
