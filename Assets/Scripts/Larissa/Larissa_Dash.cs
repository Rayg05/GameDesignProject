using UnityEngine;

public class Larissa_Dash : IState {
    Boss larissa;
    float rEX;
    float lEX;
    float curY;

    public Larissa_Dash(Boss boss) { 
        larissa = boss;
        rEX = larissa.rightEdgeX;
        lEX = larissa.leftEdgeX;
        curY = larissa.transform.position.y;
    }

    public void Enter() {
        Debug.Log("Entering Larissa_Dash.");
    }

    public void Execute() {
        // Face player.
        larissa.FaceTarget();

        // Pick a point.
        // If facing right,
        if(larissa.transform.localScale.x < 0) {
            //Move(left point)
        } else {
            //Move(right point)
        }
        //larissa.transform.position = new Vector3(rEX, curY, 0.0f);

        //Debug.Log("Execute Larissa_Dash!");
        larissa.ChooseNextState(this);
    }

    public void Exit() {
        Debug.Log("Exiting Larissa_Dash.");
    }
}