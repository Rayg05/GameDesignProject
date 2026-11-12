using UnityEngine;

public class Saturn_Boss : Boss
{
    [Header("Attack Damage")]
    public float whipDamage = 5f;
    public float diveDamage = 5f;   // touching her at any point during the dive attack

    [Header("Dive Attack")]
    public int diveCount = 3;                  // how many dives per attack
    public float diveJumpHeight = 8f;          // apex height above the floor
    public float diveJumpDuration = 0.8f;      // time to reach the apex
    public float diveHangTime = 0.3f;          // pause above the player before diving
    public float diveSpeed = 30f;              // downward speed of the dive
    public float diveUndergroundDepth = 25f;   // how far below the floor she goes (enough to be hidden)
    public float diveUndergroundTime = 0.6f;   // time spent underground between dives
    public float diveResurfaceDuration = 0.4f; // time to rise from underground back onto the floor
    public float diveGroundedTime = 0.5f;      // pause on the floor before the next jump
    public float diveMinSurfaceDistance = 4f;  // try to resurface at least this far from the player

    public float whipDist = 35.0f;

    public GameObject whipHitbox;

    [Header("Hat damage")]

    public float hatDamage = 10f;

    protected override void BuildStates() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Saturn_Idle(this));
        states.Add(new Saturn_Whip(this));   // 1
        states.Add(new Saturn_Drill(this));  // 2
        states.Add(new Saturn_Hat(this));    // 3
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

        // If close, whip. If far, randomly drill (the jump closes the distance) or throw the hat.
        // The hat is only an option once a projectile prefab is assigned in the Inspector.
        if (dist < whipDist) {
            state_machine.ChangeState(states[1]);
        } else if (projectile != null && Random.value < 0.5f) {
            state_machine.ChangeState(states[3]);
        } else {
            state_machine.ChangeState(states[2]);
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
