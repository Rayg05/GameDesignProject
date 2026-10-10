using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Boss : MonoBehaviour {
    public FSM state_machine { get; private set; }

    List<IState> states = new List<IState>();
    int num_states;

    void Awake() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Larissa_Idle(this));
        states.Add(new Larissa_Projectile(this));
        states.Add(new Larissa_Dash(this));
        num_states = states.Count;

        state_machine = new FSM(this);
    }

    void Start() {
        // Start in Larissa_Idle.
        state_machine.ChangeState(states[0]);
    }

    void Update() {
        // Execute whatever state we're in, every frame.
        state_machine.Tick();
    }

    // Picks a random state (NOT IDLE) and goes there.
    // NOTE: All states should go to Idle on termination, at least for this boss.
    public void ChooseNextState(IState previous_state) {
        // If the previous state wasn't idle, just go to idling.
        // This can be handled differently in other bosses.
        if(previous_state != states[0]) {
            state_machine.ChangeState(states[0]);
            return;
        }

        // From Idle, pick a random state to transition to.
        int rand = Random.Range(1, num_states);
        Debug.Log("Rand = " + rand + ", num_states = " + num_states);
        state_machine.ChangeState(states[rand]);
    }
}