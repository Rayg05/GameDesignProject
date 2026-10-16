using UnityEngine;

public class Player_Hurtbox : MonoBehaviour
{

    Basic_Movement2 player;

    void Start() {
        player = GetComponentInParent<Basic_Movement2>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("TOUCHING: " + other.name);
        player.HurtPlayer();
    }
}
