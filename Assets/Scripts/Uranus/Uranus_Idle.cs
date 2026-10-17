using UnityEngine;

public class Uranus_Idle : IState
{
    readonly Boss uranus;
    readonly Animator animator;
    float timer;

    public Uranus_Idle(Boss boss)
    {
        uranus = boss;
        animator = uranus.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("Entering Uranus_Idle.");
        timer = 0f;
        animator.SetBool("Idle", true);
        uranus.FaceTarget();
    }

    public void Execute()
    {
        timer += Time.deltaTime;
        if (timer > 3f) uranus.ChooseNextState(this);
    }

    public void Exit()
    {
        animator.SetBool("Idle", false);
        Debug.Log("Exiting Uranus_Idle.");
    }
}