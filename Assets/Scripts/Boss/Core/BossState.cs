// Base class for every boss state. Override only the methods you need.
public abstract class BossState
{
    protected readonly BossController boss;

    protected BossState(BossController boss)
    {
        this.boss = boss;
    }

    // Seconds since this state was entered
    protected float TimeInState => boss.StateMachine.TimeInState;

    public virtual void Enter() { }
    public virtual void Tick() { }       // called from Update
    public virtual void FixedTick() { }  // called from FixedUpdate (physics / movement)
    public virtual void Exit() { }

    // Hands control back to the boss's attack pattern (BossController.ChooseNextState)
    protected void Finish()
    {
        boss.StateMachine.ChangeState(boss.ChooseNextState());
    }

    public override string ToString() => GetType().Name;
}
