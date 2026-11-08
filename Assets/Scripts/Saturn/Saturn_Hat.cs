using UnityEngine;

public class Saturn_Hat : IState
{
    readonly Boss saturn;
    readonly Animator animator;
    float stateDuration = 1f;
    float timer;
    bool fired;
    public Saturn_Hat(Boss boss)
    {
        saturn = boss; 
        animator = saturn.GetComponent<Animator>();
    }

    public void Enter ()
    {
        timer = 0f;
        fired = false;
        animator.SetBool("Hat", true);
    }
    public void Execute()
    {
        saturn.FaceTarget();
        timer += Time.deltaTime;

        if(!fired && timer >= stateDuration / 1.2)
        {
            fired = true;

            Transform lt = saturn.transform;
        }
    }

    public void Exit()
    {
        
    }

    
}
