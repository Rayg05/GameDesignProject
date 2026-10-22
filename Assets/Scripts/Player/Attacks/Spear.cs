using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private EquipmentData spear;
    Animator attack_Animation;        // Player's own animator
    Animator attack_Effect_Animation; // Side hitbox's animator
    Animator attack_Up_Effect_Animation; // Up hitbox's animator
    Animator attack_Down_Effect_Animation; // Up hitbox's animator

     private GameObject attackArea = default;       // side hitbox
    private Collider2D attackAreaCollider;
    private Collider2D attackBonusCollider;
    private SpriteRenderer attackAreaRenderer;

    private GameObject upAttackArea = default;      // up hitbox
    private Collider2D upAttackAreaCollider;
    private Collider2D upAttackBonusCollider;
    private SpriteRenderer upAttackAreaRenderer;

    private GameObject downAttackArea = default;      // down hitbox
    private Collider2D downAttackAreaCollider;
    private Collider2D downAttackBonusCollider;
    private SpriteRenderer downAttackAreaRenderer;

    private bool attacking = false;
    private float timeToAttack = 1.15f; //DO NOT CHANGE FOR CLEAN ANIMATION SAKE
    private float timer = 0f;

    void Start()
    {
        attack_Animation  = GetComponent<Animator>();

        attackArea = transform.GetChild(4).gameObject;
        attack_Effect_Animation = attackArea.GetComponent<Animator>();
        attackAreaCollider = attackArea.GetComponent<PolygonCollider2D>();
        attackBonusCollider = attackArea.GetComponent<BoxCollider2D>();
        attackAreaRenderer = attackArea.GetComponent<SpriteRenderer>();

        upAttackArea = transform.GetChild(5).gameObject;
        attack_Up_Effect_Animation = upAttackArea.GetComponent<Animator>();
        upAttackAreaCollider = upAttackArea.GetComponent<PolygonCollider2D>();
        upAttackBonusCollider =  upAttackArea.GetComponent<BoxCollider2D>();
        upAttackAreaRenderer = upAttackArea.GetComponent<SpriteRenderer>();

        downAttackArea = transform.GetChild(6).gameObject;
        attack_Down_Effect_Animation = downAttackArea.GetComponent<Animator>();
        downAttackAreaCollider = downAttackArea.GetComponent<PolygonCollider2D>();
        downAttackBonusCollider = downAttackArea.GetComponent<BoxCollider2D>();
        downAttackAreaRenderer = downAttackArea.GetComponent<SpriteRenderer>();

        attack_Animation.SetBool("Attack", false);
        attack_Animation.SetBool("Attack_Up", false);
        attack_Animation.SetBool("Attack_Down", false);
        attack_Effect_Animation.SetBool("Press_Attack", false);
        attack_Up_Effect_Animation.SetBool("Press_Up_Attack", false);
        attack_Down_Effect_Animation.SetBool("Press_Down_Attack", false);

        attackArea.SetActive(true);
        attackAreaCollider.enabled = false;
        attackBonusCollider.enabled = false;
        attackAreaRenderer.enabled = false;

        upAttackArea.SetActive(true);
        upAttackAreaCollider.enabled = false;
        upAttackBonusCollider.enabled = false;
        upAttackAreaRenderer.enabled = false;

        downAttackArea.SetActive(true);
        downAttackAreaCollider.enabled = false;
        downAttackBonusCollider.enabled =false;
        downAttackAreaRenderer.enabled = false;
    }

    private void Update()
    {
        if(Input.GetMouseButton(0) && !attacking && Loadout.IsEquipped(spear) && Time.timeScale > 0f)
        {
            Attack();
        }

        if(attacking)
        {
            timer += Time.deltaTime;

            if(timer >= timeToAttack)
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
                attackBonusCollider.enabled = false;
                attackAreaRenderer.enabled = false;

                upAttackAreaCollider.enabled = false;
                upAttackBonusCollider.enabled = false;
                upAttackAreaRenderer.enabled = false;

                downAttackAreaCollider.enabled = false;
                downAttackBonusCollider.enabled = false;
                downAttackAreaRenderer.enabled = false;
            }
        }
    }

    private void Attack()
    {
        if(Input.GetKey(KeyCode.W))
        {
            attack_Animation.SetBool("Attack_Up", true);
            attack_Up_Effect_Animation.SetBool("Press_Up_Attack", true);

            upAttackAreaCollider.enabled = true;
            upAttackBonusCollider.enabled = true;
            upAttackAreaRenderer.enabled = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            attack_Animation.SetBool("Attack_Down", true);
            attack_Down_Effect_Animation.SetBool("Press_Down_Attack", true);
            
            downAttackAreaCollider.enabled = true;
            downAttackBonusCollider.enabled = true;
            downAttackAreaRenderer.enabled = true;
        }
        if(!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            attack_Animation.SetBool("Attack", true);
            attack_Effect_Animation.SetBool("Press_Attack", true);

            attackAreaCollider.enabled = true;
            attackBonusCollider.enabled = true;
            attackAreaRenderer.enabled = true;
        }
        attacking = true;
    }

}
