using UnityEngine;

public class Jupiter_Minions : IState
{
    readonly Boss jupiter;
    readonly Spawn_Minions attack;
    readonly Animator animator;

    public Jupiter_Minions(Boss boss, Spawn_Minions minion_spawner)
    {
        jupiter = boss;
        attack = minion_spawner;
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