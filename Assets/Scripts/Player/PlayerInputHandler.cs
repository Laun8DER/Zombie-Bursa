using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float moveSpeed = 12f;
    public Rigidbody2D rb;
    public Animator animator;
    public Vector2 moveInput;
    public PlayerInput playerInput;
    public Transform playerTransform;
    private Vector2 movement;
    public SpriteRenderer spriteRenderer;
    private float jumpForce = 8f;
    private bool isGrounded;
    float horizontalMovement;
    public Vector2 groundOffset = new Vector2(0, -1f);
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    public AudioSource audioSource; 
    public AudioClip footstepClip;



    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

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

    //MOVE
    void FixedUpdate()
    {
        if (animator.GetBool("IsBurping") == false)
        {
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }


        animator.SetFloat("magnitude", Mathf.Abs(rb.linearVelocity.x));

        if (horizontalMovement > 0.01f)
            spriteRenderer.flipX = false;
        else if (horizontalMovement < -0.01f)
            spriteRenderer.flipX = true;

        CheckGround();

        if (isGrounded && animator.GetBool("IsJumping"))
        {
            animator.SetBool("IsJumping", false);
            Debug.Log(isGrounded);
        }
        if (isGrounded && Mathf.Abs(horizontalMovement) > 0.01f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause(); // або Stop()
        }
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    //JUMP

    public void OnJump(InputAction.CallbackContext context)
    {
        CheckGround();
        if (context.performed && isGrounded)
        {
            animator.SetBool("IsJumping", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    private void CheckGround()
    {
        Vector2 checkPos = (Vector2)transform.position + groundOffset;

        // Находим все коллайдеры в радиусе
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkPos, checkRadius, groundLayer);

        isGrounded = false;
        foreach (var collider in colliders)
        {
            // Если коллайдер не принадлежит игроку — мы на земле
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
