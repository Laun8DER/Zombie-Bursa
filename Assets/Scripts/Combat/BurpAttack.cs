using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    public int burpDamage = 1;
    public float attackRange = 2.0f;
    public float rayOffsetY = 1.0f;
    public LayerMask enemyLayer;

    [Header("Stamina")]
    public Image staminaBarImage;
    public float maxStamina = 100f;
    public float currentStamina = 100f;
    public float burpStaminaCost = 10f;
    public float staminaRecoveryRate = 5f;

    private bool isBurpSubscribed;
    private bool isBurping;
    private float burpStartX;
    private System.Collections.Generic.List<GameObject> hitEnemies = new System.Collections.Generic.List<GameObject>();

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
        RefreshStaminaBar();
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
        // Пассивное восстановление стамины
        if (currentStamina < maxStamina)
        {
            currentStamina += staminaRecoveryRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
        }

        RefreshStaminaBar();

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

        // Raycast logic: Invert direction for the ray to match visual facing
        Vector2 direction = new Vector2(lastDirection * -1f, 0);
        Vector3 rayOrigin = transform.position + new Vector3(0, rayOffsetY, 0);

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, attackRange, enemyLayer);

        // Debug line visible in Scene view
        Debug.DrawRay(rayOrigin, direction * attackRange, Color.red);

        if (hit.collider != null)
        {
            Debug.Log($"Ray hit: {hit.collider.name}");
            if (hit.collider.CompareTag("enemy"))
            {
                if (!hitEnemies.Contains(hit.collider.gameObject) && TryDamageEnemy(hit.collider))
                {
                    hitEnemies.Add(hit.collider.gameObject);
                }
            }
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
        if (isBurping || currentStamina < burpStaminaCost)
        {
            return;
        }

        currentStamina -= burpStaminaCost;
        hitEnemies.Clear();
        burpStartX = rb.position.x;
        rb.linearVelocity = new Vector2(lastDirection * burpForce, rb.linearVelocity.y);

        isBurping = true;
        animator.SetBool("IsBurping", true);
        ScreenShake.Instance.Shake(burpShakeDuration, burpShakeAmplitude, burpShakeFrequency);
        // vibration gamepad
        Gamepad.current?.SetMotorSpeeds(1f, 1f);

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
        Gamepad.current?.SetMotorSpeeds(0f, 0f);
        isBurping = false;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        animator.SetBool("IsBurping", false);
        animator.SetBool("FacingLeft", false);
        animator.SetBool("FacingRight", false);
    }

    public void RestoreStamina(float amount)
    {
        if (amount <= 0f)
        {
            return;
        }

        currentStamina = Mathf.Clamp(currentStamina + amount, 0f, maxStamina);
        RefreshStaminaBar();
    }

    private bool TryDamageEnemy(Collider2D enemyCollider)
    {
        if (enemyCollider == null) return false;

        if (enemyCollider.TryGetComponent(out Health health))
        {
            health.TakeDamage(burpDamage);
            return true;
        }

        if (enemyCollider.TryGetComponent(out Cutscene_Zombie_Health cutscene))
        {
            cutscene.TakeDamage(burpDamage);
            return true;
        }

        return false;
    }

    private void RefreshStaminaBar()
    {
        if (staminaBarImage == null || maxStamina <= 0f)
        {
            return;
        }

        staminaBarImage.fillAmount = currentStamina / maxStamina;
    }
}
