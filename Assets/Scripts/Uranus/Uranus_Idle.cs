using UnityEngine;

public class Uranus_Idle : IState {
    Boss uranus;
    float timer;

    public Uranus_Idle(Boss boss) { uranus = boss; }

    public void Enter() {
        Debug.Log("Entering Uranus_Idle.");
        // Reset timer.
        timer = 0f;

        // Face player.
        uranus.FaceTarget();
        return;
    }

    public void Execute() {
        // We could replace this and use a coroutine instead.
        // The coroutine would go in Enter().
        // This is just a little easier for right now.
        timer += Time.deltaTime;

        if (timer > 3f) { uranus.ChooseNextState(this); }
        return;
    }

    public void Exit() {
        Debug.Log("Exiting Uranus_Idle.");
        return;
    }
}
