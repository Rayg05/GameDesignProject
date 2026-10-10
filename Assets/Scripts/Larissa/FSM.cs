using NUnit.Framework;
using UnityEngine;

// Every state needs:
// Enter() - Calls once when state is changed.
// Execute() - Calls every frame.
// Exit() - Calls once during ChangeState.
public interface IState {
    public void Enter();
    public void Execute();
    public void Exit();
}

public class FSM
{
    IState current_state;
    Boss boss;
    public FSM(Boss b) { boss = b; }

    // Call Exit of old state, call Enter of new state.
    public void ChangeState(IState newstate) {
        if(current_state != null) { current_state.Exit(); }

        current_state = newstate;
        current_state.Enter();
    }

    // Call state's execute every frame.
    public void Tick() {
        // We can change this later..
        current_state.Execute();
    }
}