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
    private Animator up_Attack_Animator;

    void Start()
    {
        //setting initial animations and variable to needed declarations
        up_Attack_Animator = GetComponent<Animator>();
        attack_Animation = GetComponent<Animator>();
        attackArea = transform.GetChild(1).gameObject;
        attack_Effect_Animation = attackArea.GetComponent<Animator>();
        attackAreaCollider = attackArea.GetComponent<Collider2D>();
        attackAreaRenderer = attackArea.GetComponent<SpriteRenderer>();

        up_Attack_Animator.SetBool("Attack_Up", false);
        attack_Effect_Animation.SetBool("Pressed_Attack", false);
        attack_Animation.SetBool("Attack", false);

        // Keep it active always — just start with visuals/hitbox off
        attackArea.SetActive(true);
        attackAreaCollider.enabled = false;
        attackAreaRenderer.enabled = false;

    }

    void Update()
    {
        if (Input.GetMouseButton(0) && !attacking)
        {
            Attack();
        }

        if (attacking)
        {
            timer += Time.deltaTime;
            if (timer >= timeToAttack)
            {
                up_Attack_Animator.SetBool("Attack_Up", false);
                attack_Animation.SetBool("Attack", false);
                attack_Effect_Animation.SetBool("Pressed_Attack", false);
                timer = 0;
                attacking = false;
                attackAreaCollider.enabled = false;
                attackAreaRenderer.enabled = false;
            }
        }
    }

    private void Attack()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Debug.Log("Up attack branch hit");
            up_Attack_Animator.SetBool("Attack_Up", true);
        }
        else
        {
            attack_Animation.SetBool("Attack", true);
        }

        attack_Effect_Animation.SetBool("Pressed_Attack", true);
        attacking = true;
        attackAreaCollider.enabled = true;
        attackAreaRenderer.enabled = true;
    }
}