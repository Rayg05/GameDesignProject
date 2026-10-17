using UnityEngine;

public class Uranus_Slash : IState
{
    readonly Boss uranus;

    public Uranus_Slash(Boss boss)
    {
        uranus = boss;
    }

    public void Enter()
    {
        Debug.Log("Entering Uranus_Slash.");
        uranus.ChooseNextState(this);
    }

    public void Execute() { }

    public void Exit()
    {
        Debug.Log("Exiting Uranus_Slash.");
    }
}