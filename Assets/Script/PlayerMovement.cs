using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public Animator animator;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    [Header("Ground Check Settings")]
    public Transform groundCheckPoint; // Assign an empty GameObject positioned at the player's feet
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;      // Set this in the Inspector to your "Ground" layer

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    private float horizontalInput;
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isJumping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (rb != null)
        {
            rb.gravityScale = 2f; // Adjust gravity to your liking
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        if (groundCheckPoint == null)
        {
            Debug.LogError("GroundCheckPoint not assigned to PlayerMovement script!");
            // Optionally create one dynamically if you want, but manual assignment is better for clarity
            // groundCheckPoint = new GameObject(gameObject.name + "_GroundCheck").transform;
            // groundCheckPoint.SetParent(transform);
            // groundCheckPoint.localPosition = new Vector3(0, -0.5f, 0); // Adjust y based on sprite
        }
    }

    void Update()
    {
        // --- Input ---
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // --- Ground Check ---
        CheckIfGrounded();

        // --- Jump Input ---
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
            isJumping = true;
        }else{
            isJumping = false;
        }

        // --- Animation ---
        UpdateAnimations();

        // --- Sprite Flipping ---
        FlipSprite();
    }

    void FixedUpdate()
    {
        // --- Movement ---
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
    }

    void CheckIfGrounded()
    {
        if (groundCheckPoint == null) return; // Guard clause

        // Perform a circle cast to check for ground
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset y velocity before jumping
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        // If using a Jump Trigger in Animator:
        // anim.SetTrigger("Jump");
    }

    void UpdateAnimations()
    {
        anim.SetBool("IsWalking",( Mathf.Abs(horizontalInput) > 0.01f || Mathf.Abs(horizontalInput) < 0f) && isGrounded); // Only walk if grounded
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("yVelocity",  Mathf.Abs(rb.velocity.y));
        anim.SetBool("isJumping",isJumping);

        // If not using VerticalSpeed for JumpUp, and using a trigger, you don't need to set it here
        // The trigger would handle the transition to JumpUp.
        // However, VerticalSpeed is still useful for the Fall animation.
    }

    void FlipSprite()
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            spriteRenderer.flipX = true;
        }
    }

    // Optional: Visualize the ground check radius in the editor
    void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
    }
}