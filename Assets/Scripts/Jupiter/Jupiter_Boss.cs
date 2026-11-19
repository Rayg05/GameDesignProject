using UnityEngine;

public class Jupiter_Boss : Boss
{
    [Header("Fist Attack")]
    public GameObject fistPrefab;                          // Assets/Prefab/Jupiter_Fist.prefab
    public float fistDamage = 5f;
    public Vector2 fistSpawnOffset = new Vector2(9f, 0f);  // x is toward the player, y is up
    public float fistAttackMaxDuration = 6f;               // safety cap in case the fist never despawns

    [Header("Wind Attack")]
    public float windDuration = 15f;
    public float windStrength = 5f;   // speed added to the player in the wind's direction (player moves at 10)
    public CeilingAttackController ceilingAttackController;

    protected override void BuildStates() {
        // IDLE SHOULD BE FIRST ON THE LIST!
        states.Add(new Jupiter_Idle(this));        // 0
        states.Add(new Jupiter_FistAttack(this));  // 1
        states.Add(new Jupiter_Wind(this));        // 2
        states.Add(new Jupiter_CeilingAttack(this, ceilingAttackController));
    }

    // Just Idle, in this case.
    protected override IState GetStartState() {
        return states[0];
    }

    // Placeholder pattern: Idle -> random attack -> Idle -> ...
    // TODO: wind should only be picked at half HP.
    public override void ChooseNextState() {
        IState prev = GetCurrentState();

        // If the previous state wasn't idle, just go to idling.
        if (prev != states[0]) {
            state_machine.ChangeState(states[0]);
            return;
        }

        // From Idle, pick a random attack.
        int rand = Random.Range(1, states.Count);
        state_machine.ChangeState(states[rand]);
    }
}
