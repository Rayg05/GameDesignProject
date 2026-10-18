using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public abstract class Boss : MonoBehaviour {
    public FSM state_machine { get; private set; }
    public GameObject projectile;
    public Transform target;
    public float leftEdgeX;
    public float rightEdgeX;

    protected List<IState> states = new List<IState>();
    int num_states;

    protected abstract void BuildStates();
    protected abstract IState GetStartState();
    public abstract void ChooseNextState(IState previous_state);

    protected virtual void Awake() {
        BuildStates();
        state_machine = new FSM(this);
    }

    void Start() {
        // Start in Larissa_Idle.
        state_machine.ChangeState(GetStartState());
    }

    void Update() {
        // Execute whatever state we're in, every frame.
        state_machine.Tick();
    }

    // If player is to the left, face the same way, and vice versa.
    public void FaceTarget() {
        if(target == null) { return;  }
        if(target.position.x < transform.position.x) {
            transform.localScale = new Vector2(1, transform.localScale.y);
        } else {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
    }
}