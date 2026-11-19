using UnityEngine;

public class Jupiter_CeilingAttack : IState
{
    readonly Boss jupiter;
    readonly CeilingAttackController attack;
    readonly Animator animator;

    public Jupiter_CeilingAttack(Boss boss, CeilingAttackController attackController)
    {
        jupiter = boss;
        attack = attackController;
        animator = jupiter.GetComponent<Animator>();
    }

    public void Enter()
    {
        Debug.Log("Entering Jupiter_CeilingAttack.");
        jupiter.FaceTarget();
        attack.StartAttack();
    }

    public void Execute()
    {
        // stay in this state until the controller reports it's finished
        if (!attack.IsRunning) jupiter.ChooseNextState();
    }

    public void Exit()
    {
        Debug.Log("Exiting Jupiter_CeilingAttack.");
    }
}