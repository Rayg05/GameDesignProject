using UnityEngine;

public class Saturn_Boss : Boss
{
    [Header("Attack Damage")]
    public float whipDamage = 5f;

    public float whipDist = 35.0f;

    public GameObject whipHitbox;

    protected override void BuildStates() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Saturn_Idle(this));
        states.Add(new Saturn_Whip(this));
        //states.Add(new Saturn_Hat(this));
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

        // Distance between Saturn and player.
        float dist = Mathf.Abs(target.position.x - transform.position.x);

        // If close, whip.
        if (dist < whipDist) {
            state_machine.ChangeState(states[1]);
        } else {
            state_machine.ChangeState(states[0]);
        }

        // From Idle, pick a random state to transition to.
        /*
        int rand = Random.Range(1, states.Count);
        Debug.Log("Rand = " + rand + ", num_states = " + states.Count);
        state_machine.ChangeState(states[rand]);
        */
    }

    // Called on animation events only.
    public void WhipHitboxActivation(int activate) {
        // Activate and deactivate whip at given keyframes.
        whipHitbox.SetActive(activate != 0);
    }
}
