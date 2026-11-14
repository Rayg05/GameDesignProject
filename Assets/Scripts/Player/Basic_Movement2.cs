using UnityEngine;

public class Basic_Movement2 : MonoBehaviour
{
    private Animator run_Animator;
    private Animator jump_Animator;
    private Animator fall_Animator;
    private Animator look_up_Animator;
    private Animator dash_Animator;


    private Rigidbody2D rb;
    private Collider2D bodyCollider;
    private GameObject hurtbox;
    private Health playerHealth;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] EquipmentData dash;
    [SerializeField] private float jumpHeight = 7f; // height of a full jump (space held the whole way up)
    [SerializeField] private float timeToApex = 0.35f; // seconds a full jump takes to reach the top
    [SerializeField, Min(1f)] private float fallGravityMultiplier = 2.5f; // stronger gravity when falling or when space is let go early
    private bool rising = false;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] public EquipmentData doubleJump;
    private float timeToJump = .7f;
    private float timeToFall = .2f;
    private bool jumping = false;
    private bool falling = false;
    private float jump_timer = 0f;
    private float fall_timer = 0f;
    private bool isGrounded;
    private int maxJump;
    private int jumpCount = 0;
    private float speedMultiplier = 1f;
    private float cooldown_duration = 4f;
    private bool dash_cooldown = false;
    private float dash_length = .2f;
    private float total_dash_time = 0f;
    private float waitTime = 0;
    private TrailRenderer[] dashTrails;
    public static EquipmentData DashData { get; private set; }

    void Awake()
    {
        if (dash != null) DashData = dash;
    }
    void Start()
    {
        look_up_Animator = GetComponent<Animator>();
        fall_Animator = GetComponent<Animator>();
        jump_Animator = GetComponent<Animator>();
        run_Animator = GetComponent<Animator>();
        dash_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        playerHealth = GetComponent<Health>();
        hurtbox = transform.Find("Hurtbox").gameObject;
        dashTrails = GetComponentsInChildren<TrailRenderer>();
    }

    void Update()
    {
        HandleDash();
        float currentSpeed = moveSpeed * speedMultiplier;
        float animationInput = Input.GetAxisRaw("Horizontal");
        run_Animator.SetFloat("Speed", Mathf.Abs(animationInput));
        if (animationInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (animationInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // horizontal movement dictated by input keys (A/D or Left/Right Arrow)
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput = 1f;
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput = -1f;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            look_up_Animator.SetBool("Look_Up", true);
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            look_up_Animator.SetBool("Look_Up", false);
            //up_Attack_Animator.SetBool("Up_Attack", false);
        }


        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        if (isGrounded)
        {
            jumpCount = 0;
        }
        maxJump = Loadout.IsEquipped(doubleJump) ? 1 : 0;

        // jump added with isGrounded check to prevent double jump
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || jumpCount < maxJump))
        {
            Jumping();
            jumpCount++;
        }

        // letting go of space (or reaching the top) switches to the stronger gravity, so the arc bends down sooner
        if (rising && (!Input.GetKey(KeyCode.Space) || rb.linearVelocity.y <= 0f))
        {
            rising = false;
        }

        if (jumping)
        {
            jump_timer += Time.deltaTime;
            if (jump_timer >= timeToJump)
            {
                jump_Animator.SetBool("Jump", false);
                jump_timer = 0;
                jumping = false;
                fall_Animator.SetBool("Falling", true);
                falling = true;
            }
        }
        if (falling)
        {
            fall_timer += Time.deltaTime;
            if (fall_timer >= timeToFall)
            {
                jump_Animator.SetBool("Falling", false);
                fall_timer = 0;
                falling = false;
            }
        }
    }
    void Jumping()
    {
        jump_Animator.SetBool("Jump", true);
        // launch speed that reaches jumpHeight after timeToApex seconds under the rising gravity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 2f * jumpHeight / timeToApex);
        rising = true;
        jumping = true;
    }
    private void FixedUpdate()
    {
        // gravity that brings a full jump to a stop exactly at jumpHeight
        float riseGravity = 2f * jumpHeight / (timeToApex * timeToApex);
        float gravity = rising ? riseGravity : riseGravity * fallGravityMultiplier;
        rb.gravityScale = gravity / -Physics2D.gravity.y;

        // thin box at the bottom of the player's collider, so it works at any player size
        Bounds b = bodyCollider.bounds;
        Vector2 feet = new Vector2(b.center.x, b.min.y);
        Vector2 checkSize = new Vector2(b.size.x * 0.9f, groundCheckRadius * 2f);
        isGrounded = Physics2D.OverlapBox(feet, checkSize, 0f, groundLayer);
    }

    public void HurtPlayer(float amt)
    {
        // Turn off hitbox for a set amount of time.
        Debug.Log("Player has taken damage!");
        playerHealth.Damage(amt);
        return;
    }
    private void HandleDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && !dash_cooldown && Time.time >= waitTime && Loadout.IsEquipped(dash))
        {
            total_dash_time = dash_length + Time.time;
            dash_cooldown = true;
            ItemTimers.StartActive(dash, dash_length);
        }
        if (dash_cooldown && Time.time >= total_dash_time && Loadout.IsEquipped(dash))
        {
            dash_cooldown = false;
            waitTime = Time.time + cooldown_duration;
            ItemTimers.StartCooldown(dash, cooldown_duration);
        }
        if (dash_cooldown)
        {
            speedMultiplier = 7f;
            SetTrails(true);
        }
        else
        {
            speedMultiplier = 1f;
            SetTrails(false);
        }
        dash_Animator.SetBool("Dash", dash_cooldown);
    }
    private void SetTrails(bool on)
    {
        foreach (var trail in dashTrails)
            trail.emitting = on;
    }

}