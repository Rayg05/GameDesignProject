using UnityEngine;

public class Saturn_Hat : IState
{
    readonly Boss saturn;
    readonly Animator animator;

    public Saturn_Hat(Boss boss)
    {
        saturn = boss; 
        animator = saturn.GetComponent<Animator>();
    }

    public void Enter ()
    {
        
    }
    public void Execute()
    {
        saturn.FaceTarget();
    }

    public void Exit()
    {
        
    }

    
}
