using UnityEngine;

public class ZombieChase : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Collider2D bodyCollider;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Health health;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float detectionRadius = 8f;
    public float retargetInterval = 0.15f;

    [Header("Attack")]
    public float attackRadius = 1.5f;
    public float attackCooldown = 1f;
    public float attackAnimationDuration = 0.35f;
    public int attackDamage = 1;

    [Header("Debug")]
    public bool enableDebugLogs = true;

    [Header("Visual")]
    public bool invertFacing = false;

    [Header("Targeting")]
    public string playerTag = "Player";
    public LayerMask targetLayers = ~0;

    private Transform currentTarget;
    private Collider2D currentTargetCollider;
    private PlayerHealth currentTargetHealth;
    private float retargetTimer;
    private float attackTimer;
    private float attackAnimationTimer;
    private bool wasInAttackRange;
    private string currentAnimationState;

    private const string IdleStateName = "Zombie_Idle";
    private const string WalkStateName = "Zombie_Walk";
    private const string BiteStateName = "Zombie_Bite";

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }

    private void FixedUpdate()
    {
        if (health != null && health.IsDead)
        {
            currentAnimationState = null;
            StopHorizontalMovement();
            return;
        }

        if (currentTarget == null)
        {
            retargetTimer -= Time.fixedDeltaTime;
            if (retargetTimer <= 0f)
            {
                RefreshTarget();
                retargetTimer = retargetInterval;
            }
        }

        if (currentTarget == null)
        {
            wasInAttackRange = false;
            PlayAnimationState(IdleStateName);
            StopHorizontalMovement();
            return;
        }

        if (!currentTarget.gameObject.activeInHierarchy)
        {
            LogDebug("Цель стала неактивной.");
            ClearTarget();
            PlayAnimationState(IdleStateName);
            StopHorizontalMovement();
            return;
        }

        attackTimer -= Time.fixedDeltaTime;
        attackAnimationTimer -= Time.fixedDeltaTime;

        float distanceToTarget = GetDistanceToTarget();
        float horizontalDistance = currentTarget.position.x - transform.position.x;

        UpdateFacing(horizontalDistance);

        if (attackAnimationTimer > 0f)
        {
            StopHorizontalMovement();
            PlayAnimationState(BiteStateName);
            return;
        }

        if (distanceToTarget <= attackRadius)
        {
            if (!wasInAttackRange)
            {
                LogDebug($"Вошел в радиус атаки цели {currentTarget.name}.");
                wasInAttackRange = true;
            }

            StopHorizontalMovement();
            PlayAnimationState(IdleStateName);
            TryDealDamage();
            return;
        }

        if (wasInAttackRange)
        {
            LogDebug($"Цель {currentTarget.name} вышла из радиуса атаки.");
            wasInAttackRange = false;
        }

        float direction = Mathf.Sign(horizontalDistance);
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
        PlayAnimationState(WalkStateName);
    }

    private void RefreshTarget()
    {
        if (currentTarget != null)
        {
            if (currentTarget.gameObject.activeInHierarchy)
            {
                return;
            }

            ClearTarget();
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, targetLayers);
        float closestDistance = float.MaxValue;

        foreach (Collider2D hit in hits)
        {
            if (hit == null || !hit.CompareTag(playerTag))
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = hit.transform;
                currentTargetCollider = hit;
                currentTargetHealth = hit.GetComponentInParent<PlayerHealth>();
                LogDebug($"Обнаружил цель {currentTarget.name} на дистанции {distance:F2}.");
            }
        }
    }

    private void TryDealDamage()
    {
        if (attackTimer > 0f)
        {
            return;
        }

        if (currentTargetHealth == null)
        {
            currentTargetHealth = currentTarget != null ? currentTarget.GetComponentInParent<PlayerHealth>() : null;
        }

        if (currentTargetCollider == null)
        {
            currentTargetCollider = currentTarget != null ? currentTarget.GetComponent<Collider2D>() : null;
        }

        if (currentTargetHealth == null || currentTargetHealth.IsDead)
        {
            LogDebug("Не нашел PlayerHealth на цели или цель уже мертва.");
            return;
        }

        currentTargetHealth.TakeDamage(attackDamage);
        attackAnimationTimer = attackAnimationDuration;
        ForcePlayAnimationState(BiteStateName);
        LogDebug($"Укусил {currentTarget.name} и нанес {attackDamage} урона. ХП игрока: {currentTargetHealth.CurrentHealth}/{currentTargetHealth.MaxHealth}.");
        attackTimer = attackCooldown;
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            LogDebug($"Потерял цель {currentTarget.name}.");
        }

        currentTarget = null;
        currentTargetCollider = null;
        currentTargetHealth = null;
        wasInAttackRange = false;
    }

    private float GetDistanceToTarget()
    {
        if (currentTarget == null)
        {
            return float.MaxValue;
        }

        if (bodyCollider != null && currentTargetCollider != null)
        {
            ColliderDistance2D distanceInfo = bodyCollider.Distance(currentTargetCollider);
            return distanceInfo.distance;
        }

        return Vector2.Distance(transform.position, currentTarget.position);
    }

    private void StopHorizontalMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void UpdateFacing(float horizontalDistance)
    {
        if (spriteRenderer == null || Mathf.Abs(horizontalDistance) <= 0.01f)
        {
            return;
        }

        bool shouldFlip = horizontalDistance > 0f;
        if (invertFacing)
        {
            shouldFlip = !shouldFlip;
        }

        spriteRenderer.flipX = shouldFlip;
    }

    private void PlayAnimationState(string stateName)
    {
        if (animator == null || currentAnimationState == stateName)
        {
            return;
        }

        animator.Play(stateName, 0, 0f);
        currentAnimationState = stateName;
    }

    private void ForcePlayAnimationState(string stateName)
    {
        if (animator == null)
        {
            return;
        }

        animator.Play(stateName, 0, 0f);
        currentAnimationState = stateName;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }

    private void LogDebug(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        Debug.Log($"[ZombieChase] {gameObject.name}: {message}");
    }
}
