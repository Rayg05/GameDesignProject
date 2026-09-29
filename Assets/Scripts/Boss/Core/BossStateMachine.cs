using System;

// Plain C# finite state machine. The BossController owns one and ticks it every frame.
public class BossStateMachine
{
    public BossState CurrentState { get; private set; }
    public BossState PreviousState { get; private set; }
    public float TimeInState { get; private set; }

    // (previous, next)
    public event Action<BossState, BossState> StateChanged;

    public void ChangeState(BossState next)
    {
        if (next == null) return;

        CurrentState?.Exit();
        PreviousState = CurrentState;
        CurrentState = next;
        TimeInState = 0f;
        CurrentState.Enter();

        StateChanged?.Invoke(PreviousState, CurrentState);
    }

    public void Tick(float deltaTime)
    {
        if (CurrentState == null) return;

        TimeInState += deltaTime;
        CurrentState.Tick();
    }

    public void FixedTick()
    {
        CurrentState?.FixedTick();
    }
}
