using UnityEngine;
using static UnityEditor.VersionControl.Asset;

public class Uranus_Boss : Boss
{

    protected override void BuildStates() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Uranus_Idle(this));
        states.Add(new Uranus_Blitz(this));
        states.Add(new Uranus_Slash(this));
    }

    // Just Idle, in this case.
    protected override IState GetStartState() {
        return states[0];
    }

    // Picks a random state (NOT IDLE) and goes there.
    // NOTE: All states should go to Idle on termination, at least for this boss.
    public override void ChooseNextState(IState previous_state) {
        // If the previous state wasn't idle, just go to idling.
        // This can be handled differently in other bosses.
        if (previous_state != states[0]) {
            state_machine.ChangeState(states[0]);
            return;
        }

        // From Idle, pick a random state to transition to.
        int rand = Random.Range(1, states.Count);
        Debug.Log("Rand = " + rand + ", num_states = " + states.Count);
        state_machine.ChangeState(states[rand]);
    }
}
