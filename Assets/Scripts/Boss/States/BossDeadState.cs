// Final state. The boss stops and never leaves this state.
// Put the death animation / loot / scene change in your boss's OnDied().
public class BossDeadState : BossState
{
    public BossDeadState(BossController boss) : base(boss) { }

    public override void Enter()
    {
        boss.Movement?.Stop();
    }
}
