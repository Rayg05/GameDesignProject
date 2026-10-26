using UnityEngine;

public class Larissa_Dash : IState {
    Larissa_Boss larissa;
    float rEX;
    float lEX;
    float targetX;
    float curY;
    float larissaDir;
    float larissaSpd = 0.1f;
    bool dashing = false;
    readonly Animator animator;
    Collider2D hitbox;

    public Larissa_Dash(Larissa_Boss boss) { 
        larissa = boss;
        rEX = larissa.rightEdgeX;
        lEX = larissa.leftEdgeX;
        curY = larissa.transform.position.y;
        animator = larissa.GetComponent<Animator>();
    }

    public void Enter() {
        animator.SetBool("Dash", true);
        // Activate hitbox child.
        GameObject hitbox = larissa.transform.Find("Hitbox").gameObject;
        BossDamage.Apply(hitbox, larissa.dashDamage);
        // her body hits as hard as the dash while dashing, so it doesn't matter which collider touches first
        BossDamage.Apply(larissa.gameObject, larissa.dashDamage);
        hitbox.SetActive(true);
        Debug.Log("Entering Larissa_Dash.");
    }

    public void Execute() {
        // Face player.
        larissa.FaceTarget();

        // Pick a point.
        if(!dashing) {
            larissaDir = larissa.transform.localScale.x;
            // Pick right or left point depending on where larissa is facing.
            if(larissaDir < 0) { targetX = rEX; } else { targetX = lEX; }
            dashing = true;
        } else {
            // Move Larissa.
            larissa.transform.position += new Vector3((-1)*larissaDir*larissaSpd, 0.0f, 0.0f);

            // Have we arrived?
            bool arrived = (larissaDir < 0) ? (larissa.transform.position.x >= rEX) : (larissa.transform.position.x <= lEX);

            // Exit when arrived.
            if (arrived) { larissa.ChooseNextState(this); }
        }
    }

    public void Exit() {
        animator.SetBool("Dash", false);
        larissa.transform.Find("Hitbox").gameObject.SetActive(false);
        BossDamage.Apply(larissa.gameObject, larissa.contactDamage);
        dashing = false;
        Debug.Log("Exiting Larissa_Dash.");
    }
}