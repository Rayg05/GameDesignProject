using UnityEngine;

public class Saturn_Hat : IState
{
    readonly Saturn_Boss saturn;
    readonly Animator animator;
    float stateDuration = 1f;
    float timer;
    bool fired;
    public Saturn_Hat(Saturn_Boss boss)
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
            GameObject projectile = ((Boss)saturn).projectile;
            GameObject p1 = GameObject.Instantiate(projectile, lt.position, lt.rotation);
            BossDamage.Apply(p1, saturn.hatDamage);
            p1.GetComponent<Hat_Projectile>().Shoot(new Vector2(-lt.localScale.x, 0));
        }

        if (timer >= stateDuration)
            saturn.ChooseNextState(this);
    }

    public void Exit()
    {
        
    }

    
}
