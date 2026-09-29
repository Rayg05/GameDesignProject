// Stands still for a set time (breather between attacks), then asks the pattern what's next.
public class BossIdleState : BossState
{
    public float Duration { get; set; }

    public BossIdleState(BossController boss, float duration) : base(boss)
    {
        Duration = duration;
    }

    public override void Enter()
    {
        boss.Movement?.Stop();
    }

    public override void Tick()
    {
        boss.FaceTarget();
        if (TimeInState >= Duration) Finish();
    }
}
