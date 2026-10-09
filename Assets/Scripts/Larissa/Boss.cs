using UnityEngine;

public class Boss : MonoBehaviour {
    public FSM state_machine { get; private set; }

    void Awake() {
        state_machine = new FSM();
    }

    void Start() {
        // Start in Idle.
        state_machine.ChangeState(new Idle(this));
    }

    void Update() {
        // Execute whatever state we're in, every frame.
        state_machine.Tick();
    }
}