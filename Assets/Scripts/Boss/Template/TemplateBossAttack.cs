using UnityEngine;

// TEMPLATE: one boss attack. Copy and rename it for each attack (SlamAttack, FireballAttack...).
// Timing, range, damage, hitbox and cooldown are set in the Inspector. The red box and
// yellow circle gizmos show the hitbox and range when the object is selected.
public class TemplateBossAttack : BossAttack
{
    protected override void OnWindup()
    {
        // TODO: telegraph the attack (flash, sound, charge-up VFX)
    }

    protected override void OnActive()
    {
        // TODO: the actual hit. Placeholder damages whatever is in the hitbox once.
        DamageInHitbox();
    }

    protected override void OnStageUpdate(AttackStage stage)
    {
        // TODO (optional): per-frame logic, e.g. lunge forward during Active,
        // or keep calling DamageInHitbox() for a lingering hitbox
    }

    protected override void OnRecovery()
    {
        // TODO: the boss is vulnerable here
    }

    protected override void OnEnd()
    {
        // TODO: clean up hitboxes / effects (runs whether the attack finished or was cancelled)
    }
}
