using UnityEngine;

public class HeartCompanionFollower : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public Rigidbody2D targetRigidbody;
    public PlayerHealth playerHealth;
    public SpriteRenderer playerSpriteRenderer;
    public SpriteRenderer heartSpriteRenderer;

    [Header("Follow")]
    [Min(0)]
    public int heartIndex;
    public Vector2 followOffset = new Vector2(1.1f, 0.9f);
    public float smoothTime = 0.18f;
    public bool stayBehindTarget = true;
    public float movementThreshold = 0.05f;

    [Header("Floating")]
    public float bobAmplitude = 0.15f;
    public float bobFrequency = 2.2f;
    public float swayAmplitude = 0.08f;
    public float swayFrequency = 1.35f;

    [Header("Jump Lag")]
    public float jumpLagThreshold = 0.1f;
    public float verticalLagFactor = 0.08f;
    public float maxVerticalLag = 0.45f;
    public float airborneSmoothMultiplier = 1.6f;

    [Header("State")]
    public bool hideWhenPlayerDies = true;

    private Vector3 followVelocity;
    private bool isFacingRight = true;
    private float zOffset;
    private bool hasCachedZOffset;

    private void Awake()
    {
        ResolveReferences();

        if (target != null)
        {
            CacheZOffset();
            transform.position = GetDesiredPosition();
        }

        SyncVisibilityWithHealth();
    }

    private void OnEnable()
    {
        ResolveReferences();

        if (playerHealth != null)
        {
            playerHealth.HealthChanged += OnHealthChanged;
            playerHealth.Died += OnPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (playerHealth != null)
        {
            playerHealth.HealthChanged -= OnHealthChanged;
            playerHealth.Died -= OnPlayerDied;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            GetDesiredPosition(),
            ref followVelocity,
            GetCurrentSmoothTime());
    }

    private void ResolveReferences()
    {
        if (target == null && transform.parent != null)
        {
            target = transform.parent;
        }

        if (playerHealth == null && target != null)
        {
            playerHealth = target.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        if (target == null && playerHealth != null)
        {
            target = playerHealth.transform;
        }

        if (targetRigidbody == null && target != null)
        {
            targetRigidbody = target.GetComponent<Rigidbody2D>();
        }

        if (playerSpriteRenderer == null && target != null)
        {
            playerSpriteRenderer = target.GetComponentInChildren<SpriteRenderer>();
        }

        if (heartSpriteRenderer == null)
        {
            heartSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        CacheZOffset();
    }

    private Vector3 GetDesiredPosition()
    {
        UpdateFacingDirection();

        Vector2 directionalOffset = GetDirectionalOffset();
        float verticalLag = GetVerticalLagOffset();
        float bob = Mathf.Sin(Time.time * bobFrequency * Mathf.PI * 2f) * bobAmplitude;
        float sway = Mathf.Cos(Time.time * swayFrequency * Mathf.PI * 2f) * swayAmplitude;

        return target.position + new Vector3(directionalOffset.x + sway, directionalOffset.y + bob + verticalLag, zOffset);
    }

    private void UpdateFacingDirection()
    {
        if (targetRigidbody != null && Mathf.Abs(targetRigidbody.linearVelocity.x) > movementThreshold)
        {
            isFacingRight = targetRigidbody.linearVelocity.x > 0f;
            return;
        }

        if (playerSpriteRenderer != null)
        {
            isFacingRight = !playerSpriteRenderer.flipX;
        }
    }

    private Vector2 GetDirectionalOffset()
    {
        float offsetX = Mathf.Abs(followOffset.x);

        if (!stayBehindTarget)
        {
            return new Vector2(offsetX, followOffset.y);
        }

        return isFacingRight
            ? new Vector2(-offsetX, followOffset.y)
            : new Vector2(offsetX, followOffset.y);
    }

    private void OnHealthChanged(int currentHealth, int maxHealth)
    {
        SyncVisibilityWithHealth();
    }

    private void OnPlayerDied()
    {
        if (hideWhenPlayerDies)
        {
            gameObject.SetActive(false);
        }
    }

    private void SyncVisibilityWithHealth()
    {
        if (heartSpriteRenderer == null || playerHealth == null)
        {
            return;
        }

        heartSpriteRenderer.enabled = !playerHealth.IsDead && playerHealth.CurrentHealth > heartIndex;
    }

    private void CacheZOffset()
    {
        if (hasCachedZOffset || target == null)
        {
            return;
        }

        zOffset = transform.position.z - target.position.z;
        hasCachedZOffset = true;
    }

    private float GetVerticalLagOffset()
    {
        if (targetRigidbody == null)
        {
            return 0f;
        }

        float verticalVelocity = targetRigidbody.linearVelocity.y;
        if (Mathf.Abs(verticalVelocity) <= jumpLagThreshold)
        {
            return 0f;
        }

        return Mathf.Clamp(-verticalVelocity * verticalLagFactor, -maxVerticalLag, maxVerticalLag);
    }

    private float GetCurrentSmoothTime()
    {
        if (targetRigidbody == null)
        {
            return smoothTime;
        }

        if (Mathf.Abs(targetRigidbody.linearVelocity.y) <= jumpLagThreshold)
        {
            return smoothTime;
        }

        return smoothTime * airborneSmoothMultiplier;
    }
}
