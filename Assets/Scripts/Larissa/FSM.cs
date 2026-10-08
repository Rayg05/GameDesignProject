using UnityEngine;

public interface IState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public class FSM : MonoBehaviour
{
    IState current_state;

    public void ChangeState(IState newstate) {
        if(current_state != null) { current_state.Exit(); }

        current_state = newstate;
        current_state.Enter();
    }
}

public class Boss : MonoBehaviour {
    FSM state_machine = new FSM();

    void Start() {
        state_machine.ChangeState(new Idle());
    }
}

public class Idle : IState {
    float timer;
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
        return;
    }

    public void Exit() {
        Debug.Log("Exiting idle.");
        return;
    }
}
