// Runs one BossAttack from start to finish, then asks the pattern what's next.
// Use it as: return AttackState.With(someAttack);
public class BossAttackState : BossState
{
    public BossAttack CurrentAttack { get; private set; }

    private BossAttack pendingAttack;

    public BossAttackState(BossController boss) : base(boss) { }

    public BossAttackState With(BossAttack attack)
    {
        pendingAttack = attack;
        return this;
    }

    public override void Enter()
    {
        CurrentAttack = pendingAttack;
        pendingAttack = null;
        if (CurrentAttack == null) return; // Tick will move on

        boss.Movement?.Stop();
        boss.FaceTarget();
        CurrentAttack.Begin(boss);
    }

    public override void Tick()
    {
        if (CurrentAttack == null || !CurrentAttack.IsRunning) Finish();
    }

    public override void Exit()
    {
        // Leaving early (death, stagger, forced state change) interrupts the attack
        if (CurrentAttack != null && CurrentAttack.IsRunning) CurrentAttack.Cancel();
        CurrentAttack = null;
    }
}
