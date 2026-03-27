using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    public Vector2 moveInput;
    public PlayerInput playerInput;
    public Transform playerTransform;
    private Vector2 movement;
    public SpriteRenderer spriteRenderer;

    float horizontalMovement;



    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
    }
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
    }
    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }
}
