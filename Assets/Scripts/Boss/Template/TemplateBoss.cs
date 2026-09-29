using UnityEngine;

// TEMPLATE: copy this file (and the other Template files), rename the class, and fill in the TODOs.
//
// Setup on the boss GameObject:
//   - This script (or your copy of it)
//   - BossHealth           (added automatically)
//   - A BossMovement subclass, e.g. TemplateBossMovement
//   - Rigidbody2D + Collider2D (so the player's jab can hit it)
//   - One BossAttack subclass per attack, on the boss or on children
//   - Optional: Animator, BossContactDamage
public class TemplateBoss : BossController
{
    // Custom states for this boss (stagger, taunt, phase transition, special moves...)
    public TemplateBossCustomState CustomState { get; private set; }

    protected override void CreateStates()
    {
        CustomState = new TemplateBossCustomState(this);
        // TODO: create any other custom states here
    }

    // THE ATTACK PATTERN. Called every time a state finishes.
    // Useful things available: StateMachine.CurrentState, CurrentPhase, DistanceToTarget(),
    // SelectAttack(), GetAttack<T>(), Health.Normalized, and the generic states
    // IdleState / ChaseState / AttackState / DeadState.
    public override BossState ChooseNextState()
    {
        // TODO: replace with this boss's real pattern. The placeholder below loops:
        // idle -> attack if one is usable, otherwise chase -> attack -> idle -> ...

        if (StateMachine.CurrentState == AttackState)
            return IdleState;

        BossAttack attack = SelectAttack();
        if (attack != null)
            return AttackState.With(attack);

        return ChaseState;
    }

    protected override void OnFightStarted()
    {
        // Runs as soon as the scene starts. TODO: intro roar, show health bar...
    }

    protected override void OnDamaged(int amount)
    {
        // TODO: hit flash, hurt sound, stagger...
    }

    protected override void OnPhaseChanged(int newPhase)
    {
        Debug.Log($"{name} entered phase {newPhase} ({Phases[newPhase].name})");
        // TODO: speed up, unlock attacks (see BossAttack.minPhase), change music...
    }

    protected override void OnDied()
    {
        Debug.Log($"{name} defeated");
        // TODO: death animation, sound, VFX...
        // The return to the hub is automatic (After Defeat settings in the Inspector).
        // Don't Destroy(gameObject) here, or the return timer is destroyed with it.
        // Hide the sprite / disable colliders instead.
    }
}
