using UnityEngine;

public class Larissa_Dash : IState {
    Larissa_Boss larissa;
    float rEX;
    float lEX;
    float larissaDir;
    float timer;
    bool dashing = false;
    readonly Animator animator;
    GameObject hitbox;

    public Larissa_Dash(Larissa_Boss boss) {
        larissa = boss;
        rEX = larissa.rightEdgeX;
        lEX = larissa.leftEdgeX;
        animator = larissa.GetComponent<Animator>();
        hitbox = larissa.transform.Find("Hitbox").gameObject;
    }

    public void Enter() {
        animator.SetBool("Dash", true);
        timer = 0f;
        dashing = false;
        Debug.Log("Entering Larissa_Dash.");
    }

    public void Execute() {
        if(!dashing) {
            // Windup: hold still and keep facing the player so the dash can be seen coming.
            larissa.FaceTarget();
            timer += Time.deltaTime;
            if(timer < larissa.dashWindup) { return; }

            // Lock in the direction she's facing and start hurting on contact.
            larissaDir = larissa.transform.localScale.x;
            BossDamage.Apply(hitbox, larissa.dashDamage);
            // her body hits as hard as the dash while dashing, so it doesn't matter which collider touches first
            BossDamage.Apply(larissa.gameObject, larissa.dashDamage);
            hitbox.SetActive(true);
            dashing = true;
        } else {
            // Move Larissa (per second, not per frame, so it's the same speed at any frame rate).
            larissa.transform.position += new Vector3((-1)*larissaDir*larissa.dashSpeed*Time.deltaTime, 0.0f, 0.0f);

            // Have we arrived?
            bool arrived = (larissaDir < 0) ? (larissa.transform.position.x >= rEX) : (larissa.transform.position.x <= lEX);

            // Exit when arrived.
            if (arrived) { larissa.ChooseNextState(this); }
        }
    }

    public void Exit() {
        animator.SetBool("Dash", false);
        hitbox.SetActive(false);
        BossDamage.Apply(larissa.gameObject, larissa.contactDamage);
        dashing = false;
        Debug.Log("Exiting Larissa_Dash.");
    }
}
