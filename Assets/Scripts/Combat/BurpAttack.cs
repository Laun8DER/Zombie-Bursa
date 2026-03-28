using UnityEngine;
using UnityEngine.InputSystem;

public class BurpAttack : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public Transform playerTransform;
    public float burpForce = 30f;
    public float burpShakeDuration = 0.18f;
    public float burpShakeAmplitude = 0.2f;
    public float burpShakeFrequency = 30f;
    public float lastDirection;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public bool LastDirectionBool;
    public AudioClip mySound;
    public float burpDeceleration = 120f;
    public float burpStopVelocityThreshold = 0.15f;
    public float burpMaxDistance = 3.5f;
    private bool isBurpSubscribed;
    private bool isBurping;
    private float burpStartX;

    private void OnEnable()
    {
        TrySubscribeBurp();
    }

    private void OnDisable()
    {
        TryUnsubscribeBurp();
    }

    private void Start()
    {
        TrySubscribeBurp();
        audioSource.clip = mySound;
    }

    private void TrySubscribeBurp()
    {
        if (isBurpSubscribed)
        {
            return;
        }

        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager == null || inputManager.actions == null)
        {
            return;
        }

        inputManager.actions.Player.Burp.performed += DoBurp;
        isBurpSubscribed = true;
    }

    private void TryUnsubscribeBurp()
    {
        if (!isBurpSubscribed)
        {
            return;
        }

        PlayerInputManager inputManager = PlayerInputManager.Instance;
        if (inputManager != null && inputManager.actions != null)
        {
            inputManager.actions.Player.Burp.performed -= DoBurp;
        }

        isBurpSubscribed = false;
    }

    void Update()
    {
        if (!isBurping)
        {
            lastDirection = spriteRenderer.flipX ? 1f : -1f;
        }

        if (lastDirection == 1f)
        {
            LastDirectionBool = true;
        }
        else if (lastDirection == -1f)
        {
            LastDirectionBool = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isBurping)
        {
            return;
        }

        float traveledDistance = Mathf.Abs(rb.position.x - burpStartX);
        if (traveledDistance >= burpMaxDistance)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            StopBurpState();
            return;
        }

        float slowedVelocityX = Mathf.MoveTowards(rb.linearVelocity.x, 0f, burpDeceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector2(slowedVelocityX, rb.linearVelocity.y);

        if (Mathf.Abs(rb.linearVelocity.x) <= burpStopVelocityThreshold)
        {
            StopBurpState();
        }
    }

    private void DoBurp(InputAction.CallbackContext ctx)
    {
        if (isBurping)
        {
            return;
        }

        burpStartX = rb.position.x;
        rb.linearVelocity = new Vector2(lastDirection * burpForce, rb.linearVelocity.y);

        isBurping = true;
        animator.SetBool("IsBurping", true);
        ScreenShake.Instance.Shake(burpShakeDuration, burpShakeAmplitude, burpShakeFrequency);

        audioSource.Play();

        if (LastDirectionBool)
        {
            animator.SetBool("FacingLeft", true);
            animator.SetBool("FacingRight", false);
        }
        else if (!LastDirectionBool)
        {
            animator.SetBool("FacingLeft", false);
            animator.SetBool("FacingRight", true);
        }
    }

    private void StopBurpState()
    {
        isBurping = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        animator.SetBool("IsBurping", false);
        animator.SetBool("FacingLeft", false);
        animator.SetBool("FacingRight", false);
    }
}
