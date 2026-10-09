using UnityEngine;

public class Idle : IState {
    Boss larissa;
    float timer;

    public Idle(Boss boss) { larissa = boss; }

    public void Enter() {
        Debug.Log("Entering idle.");
        timer = 0f;
        return;
    }

    public void Execute() {
        // We could replace this and use a coroutine instead.
        // The coroutine would go in Enter().
        // This is just a little easier for right now.
        timer += Time.deltaTime;

        if (timer > 5f) { larissa.state_machine.ChangeState(new Larissa_Projectile(larissa)); }
        return;
    }

    public void Exit() {
        Debug.Log("Exiting idle.");
        return;
    }
}
