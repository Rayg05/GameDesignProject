using UnityEngine;

public class Larissa_Projectile : IState {
    Boss larissa;

    public Larissa_Projectile(Boss boss) { larissa = boss; }

    public void Enter() {
        Debug.Log("Entering Larissa_Projectile.");
    }

    public void Execute() {
        Debug.Log("Execute Larissa_Projectile!");
    }

    public void Exit() {
        Debug.Log("Exiting Larissa_Projectile.");
    }
}