using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JabAttack : MonoBehaviour
{
    Animator attack_Animation;        // Player's own animator
    Animator attack_Effect_Animation; // Weapon/hitbox's animator
    private GameObject attackArea = default;
    private Collider2D attackAreaCollider;
    private SpriteRenderer attackAreaRenderer;
    private bool attacking = false;
    private float timeToAttack = 1.5f;
    private float timer = 0f;

    void Start()
    {
        attack_Animation = GetComponent<Animator>();                    // Player's animator
        attackArea = transform.GetChild(1).gameObject;
        attack_Effect_Animation = attackArea.GetComponent<Animator>();  // Weapon's animator (has Pressed_Attack)
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Attack();
        }

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

    private void Attack()
    {
        attack_Animation.SetBool("Attack", true);
        attack_Effect_Animation.SetBool("Pressed_Attack", true);
        attacking = true;
        attackAreaCollider.enabled = true;
        attackAreaRenderer.enabled = true;
    }
}