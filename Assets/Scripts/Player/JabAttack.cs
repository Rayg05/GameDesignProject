using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonoBehaviour
{
    //base jab variables
    Animator attack_Animation;        // Player's own animator
    Animator attack_Effect_Animation; // Side hitbox's animator
    Animator attack_Up_Effect_Animation; // Up hitbox's animator
    Animator attack_Down_Effect_Animation; // Up hitbox's animator

    private GameObject attackArea = default;       // side hitbox
    private Collider2D attackAreaCollider;
    private SpriteRenderer attackAreaRenderer;

    private GameObject upAttackArea = default;      // up hitbox
    private Collider2D upAttackAreaCollider;
    private SpriteRenderer upAttackAreaRenderer;

    private GameObject downAttackArea = default;      // down hitbox
    private Collider2D downAttackAreaCollider;
    private SpriteRenderer downAttackAreaRenderer;

    private bool attacking = false;
    private float timeToAttack = 1.15f; //DO NOT CHANGE FOR CLEAN ANIMATION SAKE
    private float timer = 0f;

    void Start()
    {
        attack_Animation = GetComponent<Animator>(); 

        attackArea = transform.GetChild(1).gameObject;
        attack_Effect_Animation = attackArea.GetComponent<Animator>();
        attackAreaCollider = attackArea.GetComponent<Collider2D>();
        attackAreaRenderer = attackArea.GetComponent<SpriteRenderer>();

        upAttackArea = transform.GetChild(2).gameObject; 
        attack_Up_Effect_Animation = upAttackArea.GetComponent<Animator>();
        upAttackAreaCollider = upAttackArea.GetComponent<Collider2D>();
        upAttackAreaRenderer = upAttackArea.GetComponent<SpriteRenderer>();

        downAttackArea = transform.GetChild(3).gameObject; 
        attack_Down_Effect_Animation = downAttackArea.GetComponent<Animator>();
        downAttackAreaCollider = downAttackArea.GetComponent<Collider2D>();
        downAttackAreaRenderer = downAttackArea.GetComponent<SpriteRenderer>();

        attack_Animation.SetBool("Attack", false);
        attack_Animation.SetBool("Attack_Up", false);
        attack_Animation.SetBool("Attack_Down", false);
        attack_Effect_Animation.SetBool("Press_Attack", false);
        attack_Up_Effect_Animation.SetBool("Press_Up_Attack", false);
        attack_Down_Effect_Animation.SetBool("Press_Down_Attack", false);

        // Keep both hitbox objects active always — just start with visuals/hitboxes off
        attackArea.SetActive(true);
        attackAreaCollider.enabled = false;
        attackAreaRenderer.enabled = false;

        upAttackArea.SetActive(true);
        upAttackAreaCollider.enabled = false;
        upAttackAreaRenderer.enabled = false;

        downAttackArea.SetActive(true);
        downAttackAreaCollider.enabled = false;
        downAttackAreaRenderer.enabled = false;
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
                attack_Animation.SetBool("Attack", false);
                attack_Animation.SetBool("Attack_Up", false);
                attack_Animation.SetBool("Attack_Down", false);
                attack_Effect_Animation.SetBool("Press_Attack", false);
                attack_Up_Effect_Animation.SetBool("Press_Up_Attack", false);
                attack_Down_Effect_Animation.SetBool("Press_Down_Attack", false);
                timer = 0;
                attacking = false;

                attackAreaCollider.enabled = false;
                attackAreaRenderer.enabled = false;

                upAttackAreaCollider.enabled = false;
                upAttackAreaRenderer.enabled = false;

                downAttackAreaCollider.enabled = false;
                downAttackAreaRenderer.enabled = false;
            }
        }
    }

    private void Attack()
    {
        if (Input.GetKey(KeyCode.W))
        {
            //Debug.Log("Up attack branch hit");
            attack_Animation.SetBool("Attack_Up", true);
            attack_Up_Effect_Animation.SetBool("Press_Up_Attack", true);


            upAttackAreaCollider.enabled = true;
            upAttackAreaRenderer.enabled = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            //Debug.Log("Up attack branch hit");
            attack_Animation.SetBool("Attack_Down", true);
            attack_Down_Effect_Animation.SetBool("Press_Down_Attack", true);

            downAttackAreaCollider.enabled = true;
            downAttackAreaRenderer.enabled = true;
        }
        if(!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            attack_Animation.SetBool("Attack", true);
            attack_Effect_Animation.SetBool("Press_Attack", true);

            attackAreaCollider.enabled = true;
            attackAreaRenderer.enabled = true;
        }

        attacking = true;
    }
}