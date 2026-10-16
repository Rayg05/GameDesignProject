using UnityEngine;

public class Larissa_Projectile : IState {
    Boss larissa;
    private float spread = 0.5f;

    public Larissa_Projectile(Boss boss) { larissa = boss; }

    public void Enter() {
        Debug.Log("Entering Larissa_Projectile.");
    }

    public void Execute() {
        // Face the player.
        larissa.FaceTarget();
        Transform lt = larissa.transform;

        // Spawn the projectiles.
        GameObject p1 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
        p1.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, spread));

        GameObject p2 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
        p2.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, 0));

        GameObject p3 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
        p3.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, -spread));

        // Play the Larissa animation.

        // Go back to Idle.
        larissa.ChooseNextState(this);
    }

    public void Exit() {
        Debug.Log("Exiting Larissa_Projectile.");
    }
}