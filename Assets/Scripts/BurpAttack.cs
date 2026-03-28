using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class BurpAttack : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    public Transform playerTransform;
    public float burpForce = 30f;
    public float lastDirection;
    public SpriteRenderer spriteRenderer;
    public AudioSource audioSource;
    public bool LastDirectionBool;
    public AudioClip mySound;
    private void OnEnable()
    {
        PlayerInputManager.Instance.actions.Player.Burp.performed += DoBurp;
    }
    private void OnDisable()
    {
        PlayerInputManager.Instance.actions.Player.Burp.performed -= DoBurp;
    }

    private void Start()
    {
        audioSource.clip = mySound;
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
