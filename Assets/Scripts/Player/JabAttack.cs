using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonoBehaviour
{
    //base jab variables
    Animator attack_Animation;
    Animator attack_Effect_Animation; 
    private GameObject attackArea = default;
    private Collider2D attackAreaCollider;
    private SpriteRenderer attackAreaRenderer;
    private bool attacking = false;
    private float timeToAttack = 1.15f; //DO NOT CHANGE FOR CLEAN ANIMATION SAKE
    private float timer = 0f;

    void Start()
    {
        //setting initial animations and variable to needed declarations
        attack_Animation = GetComponent<Animator>();
        attackArea = transform.GetChild(1).gameObject;
        attack_Effect_Animation = attackArea.GetComponent<Animator>(); 
        attackAreaCollider = attackArea.GetComponent<Collider2D>();
        attackAreaRenderer = attackArea.GetComponent<SpriteRenderer>();

        attack_Effect_Animation.SetBool("Pressed_Attack", false);
        attack_Animation.SetBool("Attack", false);

        // Keep it active always — just start with visuals/hitbox off
        attackArea.SetActive(true);
        attackAreaCollider.enabled = false;
        attackAreaRenderer.enabled = false;
    }

    void Update()
    {
        //attack button
        if (Input.GetMouseButton(0))
        {
            //calls attack function
            Attack();
        }
        
        //sets up timer that will deactivate the attack animation 
        //and its area hitbox
        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= timeToAttack)
            {
                attack_Animation.SetBool("Attack", false);
                attack_Effect_Animation.SetBool("Pressed_Attack", false);
                timer = 0;
                attacking = false;
                attackAreaCollider.enabled = false;
                attackAreaRenderer.enabled = false;
            }
        }
    }

    //attack function that activates needed animations and 
    //area hitbox
    private void Attack()
    {
        attack_Animation.SetBool("Attack", true);
        attack_Effect_Animation.SetBool("Pressed_Attack", true);
        attacking = true;
        attackAreaCollider.enabled = true;
        attackAreaRenderer.enabled = true;
    }
}