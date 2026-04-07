using UnityEngine;

public class FatZombieBounceTrigger : MonoBehaviour
{
    [Header("Bounce")]
    public float bounceImpulse = 8f;
    public string playerTag = "Player";
    public bool clampFallingSpeed = true;
    public float minimumFallingSpeed = 0.1f;

    [Header("Stomp Check")]
    public float raycastDistance = 0.75f;
    public float topStompWindow = 0.9f;

    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Rigidbody2D targetRigidbody = other.attachedRigidbody != null
            ? other.attachedRigidbody
            : other.GetComponentInParent<Rigidbody2D>();

        if (targetRigidbody == null || !targetRigidbody.CompareTag(playerTag))
        {
            return;
        }

        Collider2D playerCollider = other;
        if (playerCollider == null)
        {
            return;
        }

        Vector2 velocity = targetRigidbody.linearVelocity;
        if (velocity.y >= -minimumFallingSpeed)
        {
            return;
        }

        Bounds playerBounds = playerCollider.bounds;
        Bounds zombieBounds = triggerCollider.bounds;

        if (playerBounds.min.y < zombieBounds.max.y - topStompWindow)
        {
            return;
        }

        Vector2 rayOrigin = new Vector2(playerBounds.center.x, playerBounds.min.y + 0.05f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, raycastDistance);

        bool hitThisZombieFirst = false;
        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;
            if (hitCollider == null || hitCollider.attachedRigidbody == targetRigidbody)
            {
                continue;
            }

            hitThisZombieFirst = hitCollider == triggerCollider;
            break;
        }

        if (!hitThisZombieFirst)
        {
            return;
        }

        if (clampFallingSpeed && velocity.y < 0f)
        {
            velocity.y = 0f;
            targetRigidbody.linearVelocity = velocity;
        }

        targetRigidbody.AddForce(Vector2.up * bounceImpulse, ForceMode2D.Impulse);

        Animator targetAnimator = targetRigidbody.GetComponent<Animator>();
        if (targetAnimator != null)
        {
            targetAnimator.SetBool("IsJumping", true);
        }
    }
}
