using System.Threading;
using UnityEngine;

public class Player_Hurtbox : MonoBehaviour
{

    private float itimer = 0.0f;
    private float imax = 1.5f;
    private bool invincible = false;

    Basic_Movement2 player;

    void Start() {
        player = GetComponentInParent<Basic_Movement2>();
    }

    void Update() {
        // Increase timer when necessary.
        if (itimer < imax) {
            itimer += Time.deltaTime;
            // If timer is done, set invincible to be false.
            if (itimer >= imax) { invincible = false; }
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("TOUCHING: " + other.name);

        // If not invincible, hurt player and set invincible.
        if(!invincible) { player.HurtPlayer(5.0f); invincible = true; itimer = 0.0f; }

        if(other.tag == "BossProjectile") {
            // Destroy the projectile.
            Destroy(other.gameObject);
        }
    }
}
