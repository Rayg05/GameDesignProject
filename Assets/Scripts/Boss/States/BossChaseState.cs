// Moves toward the target (using the boss's BossMovement) until an attack can be used
// or the timeout runs out, then asks the pattern what's next.
public class BossChaseState : BossState
{
    public float Timeout { get; set; }

    public BossChaseState(BossController boss, float timeout) : base(boss)
    {
        Timeout = timeout;
    }

    public override void Tick()
    {
        if (boss.HasUsableAttack() || TimeInState >= Timeout) Finish();
    }

    public override void FixedTick()
    {
        if (boss.Target == null || boss.Movement == null) return;

        boss.FaceTarget();
        boss.Movement.MoveTowards(boss.Target.position);
    }

    public override void Exit()
    {
        boss.Movement?.Stop();
    }
}
