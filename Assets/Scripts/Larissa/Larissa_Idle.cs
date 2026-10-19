using UnityEngine;

public class Larissa_Idle : IState {
    Boss larissa;
    float timer;
    readonly Animator animator;
    public Larissa_Idle(Boss boss) { 
        larissa = boss;
        animator = larissa.GetComponent<Animator>();
    }

    public void Enter() {
        animator.SetBool("Idle", true);
        Debug.Log("Entering Larissa_Idle.");
        // Reset timer.
        timer = 0f;

        // Face player.
        larissa.FaceTarget();
        return;
    }

    public void Execute() {
        // We could replace this and use a coroutine instead.
        // The coroutine would go in Enter().
        // This is just a little easier for right now.
        timer += Time.deltaTime;

        if (timer > 3f) { larissa.ChooseNextState(this); }
        return;
    }

    public void Exit() {
        animator.SetBool("Idle", false);
        Debug.Log("Exiting Larissa_Idle.");
        return;
    }
}
