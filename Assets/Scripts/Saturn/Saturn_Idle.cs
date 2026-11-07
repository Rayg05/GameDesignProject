using UnityEngine;

public class Saturn_Idle : IState {
    readonly Boss saturn;
    readonly Animator animator;
    float timer;

    public Saturn_Idle(Boss boss) {
        saturn = boss;
        animator = saturn.GetComponent<Animator>();
    }

    public void Enter() {
        Debug.Log("Entering Saturn_Idle.");
        timer = 0f;
        //animator.SetBool("Idle", true);
        saturn.FaceTarget();
    }

    public void Execute() {
        timer += Time.deltaTime;
        if (timer > 3f) saturn.ChooseNextState(this);
    }

    public void Exit() {
        //animator.SetBool("Idle", false);
        Debug.Log("Exiting Saturn_Idle.");
    }
}