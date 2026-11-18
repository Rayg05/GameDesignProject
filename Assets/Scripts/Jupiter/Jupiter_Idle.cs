using UnityEngine;

public class Jupiter_Idle : IState {
    readonly Boss jupiter;
    readonly Animator animator;
    float timer;

    public Jupiter_Idle(Boss boss) {
        jupiter = boss;
        animator = jupiter.GetComponent<Animator>();
    }

    public void Enter() {
        Debug.Log("Entering Jupiter_Idle.");
        timer = 0f;
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetBool("Idle", true);
        jupiter.FaceTarget();
    }

    public void Execute() {
        timer += Time.deltaTime;
        if (timer > 3f) jupiter.ChooseNextState();
    }

    public void Exit() {
        if (animator != null && animator.runtimeAnimatorController != null) animator.SetBool("Idle", false);
        Debug.Log("Exiting Jupiter_Idle.");
    }
}
