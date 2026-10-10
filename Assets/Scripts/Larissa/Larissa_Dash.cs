using UnityEngine;

public class Larissa_Dash : IState {
    Boss larissa;

    public Larissa_Dash(Boss boss) { larissa = boss; }

    public void Enter() {
        Debug.Log("Entering Larissa_Dash.");
    }

    public void Execute() {
        //Debug.Log("Execute Larissa_Dash!");
        larissa.ChooseNextState(this);
    }

    public void Exit() {
        Debug.Log("Exiting Larissa_Dash.");
    }
}