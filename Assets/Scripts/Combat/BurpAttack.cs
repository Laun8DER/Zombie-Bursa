using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

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
    private bool isBurpSubscribed;

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
        if (!animator.GetBool("IsBurping"))
        {
            lastDirection = spriteRenderer.flipX ? 1f : -1f;
        }
        if(lastDirection == 1f)
        {
            LastDirectionBool = true;
        }
        else if (lastDirection == -1f)
        {
            LastDirectionBool = false;
        }
    }
    private void DoBurp(InputAction.CallbackContext ctx)
    {
        rb.linearVelocity = new Vector2(lastDirection * burpForce, rb.linearVelocity.y);

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

            StartCoroutine(StopBurp());
    }
    private IEnumerator StopBurp()
    {
        yield return new WaitForSeconds(0.30f);

        animator.SetBool("IsBurping", false);
        animator.SetBool("FacingLeft", false);
        animator.SetBool("FacingRight", false);

    }
}
