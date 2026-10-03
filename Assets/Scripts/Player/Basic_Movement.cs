using UnityEngine;

public class Basic_Movement : MonoBehaviour
{
    private Animator run_Animator;
    private Animator jump_Animator;
    private Animator fall_Animator;

    private Rigidbody2D rb;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float jumpForce = 1f;

    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.1f;
    private float timeToJump = .7f;
    private float timeToFall = .2f;
    private bool jumping = false;
    private bool falling = false;
    private float jump_timer = 0f;
    private float fall_timer = 0f;
    private bool isGrounded;

    void Start()
    {
        fall_Animator = GetComponent<Animator>();
        jump_Animator = GetComponent<Animator>();
        run_Animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float animationInput = Input.GetAxisRaw("Horizontal");
        run_Animator.SetFloat("Speed", Mathf.Abs(animationInput));
        if (animationInput > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (animationInput < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // horizontal movement dictated by input keys (A/D or Left/Right Arrow)
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            moveInput = -1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // jump added with isGrounded check to prevent double jump
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            Jumping();
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
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // reset vertical velocity
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        jumping = true;
    }
    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}