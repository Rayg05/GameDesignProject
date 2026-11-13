
using Unity.AppUI.Core;
using Unity.AppUI.UI;
using UnityEngine;

public class Hat_Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 30f;
    private int bounces = 8;

    private float rotateSpeed = 200f;

    [SerializeField] Transform plutoPos;

    public Transform saturn;

   
    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        Shoot(new Vector2(2, 0));

    
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if(bounces > 0)
        {
          bounces--;  
        }
        

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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(bounces == 0 && collision.gameObject.CompareTag("Boss"))
        {
            Debug.Log("entered boss");
            Destroy(gameObject);
        }
    }



    public void Shoot(Vector2 dir) {
        rb.linearVelocity = dir * speed;
    }

    void Update() {
        if(bounces == 0)
        {
            Vector2 direction = (Vector2)saturn.position - rb.position;

            direction.Normalize();

            float rotate = Vector3.Cross(direction, transform.up).z;

            rb.angularVelocity = -rotate * rotateSpeed;

            rb.linearVelocity = transform.up * speed;
        }
    }
}
