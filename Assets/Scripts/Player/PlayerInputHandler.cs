using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12f;
    public float jumpForce = 6f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public PlayerInput playerInput;

    [Header("Ground Check")]
    public Vector2 groundOffset = new Vector2(0, -1f);
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip footstepClip;

    private float horizontalMovement;
    private bool isGrounded;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (groundLayer == 0)
        {
            groundLayer = ~LayerMask.GetMask("Ignore Raycast");
        }

        if (audioSource != null && footstepClip != null)
        {
            audioSource.clip = footstepClip;
            audioSource.loop = true;
        }
    }

    private void FixedUpdate()
    {
        CheckGround();

        // MOVEMENT
        if (!animator.GetBool("IsBurping"))
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }

        // ANIMATOR SPEED
        animator.SetFloat("magnitude", Mathf.Abs(rb.linearVelocity.x));

        // FLIP
        if (horizontalMovement > 0.01f)
            spriteRenderer.flipX = false;
        else if (horizontalMovement < -0.01f)
            spriteRenderer.flipX = true;

        // LAND RESET
        if (isGrounded && animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsJumping", false);
        }

        // FOOTSTEPS
        if (isGrounded && Mathf.Abs(horizontalMovement) > 0.01f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    // MOVE (Input System Event)
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    // JUMP (Input System Event)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        CheckGround();

        if (isGrounded)
        {
            animator.SetBool("IsJumping", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundOffset;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            checkPos,
            checkRadius,
            groundLayer
        );

        isGrounded = false;

        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                isGrounded = true;
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 checkPos = (Vector2)transform.position + groundOffset;
        Gizmos.DrawWireSphere(checkPos, checkRadius);
    }
}