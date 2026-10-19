using UnityEngine;

public class Larissa_Projectile : IState {
    Boss larissa;
    private float spread = 0.5f;
    float stateDuration = 1f;
    float timer;
    bool fired;
    readonly Animator animator;
    public Larissa_Projectile(Boss boss) 
    { 
        larissa = boss;
        animator = larissa.GetComponent<Animator>();
    }

    public void Enter() {
        timer = 0f;
        fired = false;
        animator.SetBool("Ice", true);
        Debug.Log("Entering Larissa_Projectile.");
    }

    public void Execute() {
        // Face the player.
        larissa.FaceTarget();
        timer += Time.deltaTime;
        if (!fired && timer >= stateDuration/1.2)
        {
            fired = true;
            Transform lt = larissa.transform;

            // Spawn the projectiles.
            GameObject p1 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
            p1.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, spread));

            GameObject p2 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
            p2.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, 0));

            GameObject p3 = GameObject.Instantiate(larissa.projectile, lt.position, lt.rotation);
            p3.GetComponent<BasicProjectile>().Shoot(new Vector2(-lt.localScale.x, -spread));
        }
        // Play the Larissa animation.

        // Go back to Idle.
        if (timer >= stateDuration)
            larissa.ChooseNextState(this);
    }

    public void Exit() {
        animator.SetBool("Ice", false);
        Debug.Log("Exiting Larissa_Projectile.");
    }
}