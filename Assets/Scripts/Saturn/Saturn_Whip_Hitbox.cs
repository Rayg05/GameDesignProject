using UnityEngine;

public class Saturn_Whip_Hitbox : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("TOUCHING " + collision.name);
        // Inflict damage.

        // Set knockback?
    }
}
