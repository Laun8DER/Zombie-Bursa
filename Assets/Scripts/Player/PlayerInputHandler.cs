using UnityEditorInternal;
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
    private float jumpForce = 7f;
    private bool isGrounded;
    float horizontalMovement;
    public Transform groundCheck;
    public float checkDistance = 0.1f;
    public LayerMask groundLayer;



    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
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
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
    //JUMP

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            animator.SetBool("IsJumping", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }
    private void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, checkDistance, groundLayer);
        if (hit.collider != null) { isGrounded = true; }
        else { isGrounded = false; }
    }
}
