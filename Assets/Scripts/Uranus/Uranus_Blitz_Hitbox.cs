using UnityEngine;

public class Uranus_Blitz_Hitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("TOUCHING " + collision.name);
    }
}
