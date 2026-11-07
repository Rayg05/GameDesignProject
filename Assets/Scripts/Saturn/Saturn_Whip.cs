using UnityEngine;

public class Saturn_Whip : IState {
    readonly Boss saturn;
    readonly Animator animator;
    //readonly GameObject hitbox;
    readonly SpriteRenderer spriteRenderer;
    float whipDuration;

    public Saturn_Whip(Boss boss) {
        //hitbox = boss.transform.Find("WhipHitbox").gameObject;
        saturn = boss;
        animator = saturn.GetComponent<Animator>();
        spriteRenderer = saturn.GetComponent<SpriteRenderer>();
    }

    public void Enter() {
        Debug.Log("Entering Saturn_Whip.");
        //animator.SetBool("Whip", true);
        saturn.FaceTarget();
    }

    public void Execute() {
        // % completion of the whip animation.
        whipDuration = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        Debug.Log("whipDuration: " + whipDuration);

        // If the animation is done, we're done.
        Debug.Log("Is " + whipDuration + " >= 1?");
        if(whipDuration >= 1.0f) { saturn.ChooseNextState(this); }
        Debug.Log("Yes!");
    }

    public void Exit() {
        //animator.SetBool("Whip", false);
        whipDuration = 0.0f;
        saturn.FaceTarget();
        Debug.Log("Exiting Saturn_Whip.");
    }

    // Called on animation events only.
    //public void WhipHitboxActivation(bool activate) {
        // Activate and deactivate whip at given keyframes.
    //    hitbox.SetActive(activate);
    //}
}