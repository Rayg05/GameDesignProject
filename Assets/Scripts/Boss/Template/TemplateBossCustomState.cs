// TEMPLATE: example of a boss-specific state. Copy and rename it for each custom state.
// Enter it from ChooseNextState() with: return CustomState;
public class TemplateBossCustomState : BossState
{
    // Typed reference so you can reach boss-specific fields/methods
    protected readonly TemplateBoss templateBoss;

    public TemplateBossCustomState(TemplateBoss boss) : base(boss)
    {
        templateBoss = boss;
    }

    public override void Enter()
    {
        // TODO: runs once when the state starts
    }

    public override void Tick()
    {
        // TODO: runs every frame. Call Finish() when done to let the attack pattern pick the next state.
        Finish();
    }

    public override void FixedTick()
    {
        // TODO: physics / movement every FixedUpdate
    }

    public override void Exit()
    {
        // TODO: clean up
    }
}
