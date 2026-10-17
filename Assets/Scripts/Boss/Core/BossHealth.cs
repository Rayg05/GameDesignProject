using UnityEngine;

public class BossHealth : MonoBehaviour {
    [SerializeField] private float health = 100f;
    [SerializeField] private float maxHP = 100f;

    public bool invincible = false;
    private float itimer = 0.0f;
    private float imax = 1.0f; // Amount of time the boss will be invincible.

    public void Damage(float amount) {
        if(invincible) { return; }
        // Deal damage if not invincible.
        health -= amount;
        if (health <= 0) {
            Destroy(gameObject);
            return;
        }
        // Reset invincible timer.
        itimer = 0.0f;
        invincible = true;
    }

    void Update() {
        // Increase timer when necessary.
        if(itimer < imax) { 
            itimer += Time.deltaTime;
            // If timer is done, set invincible to be false.
            if(itimer >= imax) { invincible = false; }
        }
    }
}