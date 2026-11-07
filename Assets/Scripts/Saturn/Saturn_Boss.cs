using UnityEngine;

public class Saturn_Boss : Boss
{
    [Header("Attack Damage")]
    public float whipDamage = 5f;

    protected override void BuildStates() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Saturn_Idle(this));
        //states.Add(new Saturn_Whip(this));
        states.Add(new Saturn_Hat(this));
        //states.Add(new Saturn_Drill(this));
    }

    // Just Idle, in this case.
    protected override IState GetStartState() {
        return states[0];
    }

    // == IN PROGRESS ==
    public override void ChooseNextState(IState previous_state) {
        // If the previous state wasn't idle, just go to idling.
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
