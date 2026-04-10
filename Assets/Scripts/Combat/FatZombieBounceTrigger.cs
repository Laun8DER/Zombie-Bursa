using UnityEngine;

public class FatZombieBounceTrigger : MonoBehaviour
{
    [Header("Bounce")]
    public float bounceImpulse = 8f;
    public string playerTag = "Player";
    public bool clampFallingSpeed = true;
    public float minimumFallingSpeed = 0f;

    [Header("Stomp Check")]
    public float raycastDistance = 0.75f;
    public float topStompWindow = 0.9f;

    private Collider2D zombieCollider;

    private void Awake()
    {
        zombieCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D targetRigidbody = collision.rigidbody;

        if (targetRigidbody == null || !targetRigidbody.CompareTag(playerTag))
            return;

        Collider2D playerCollider = collision.collider;
        if (playerCollider == null)
            return;

        Vector2 velocity = targetRigidbody.linearVelocity;

        // Перевірка що гравець падає
        //if (velocity.y >= -minimumFallingSpeed)
        //    return;

        Bounds playerBounds = playerCollider.bounds;
        Bounds zombieBounds = zombieCollider.bounds;

        //Перевірка що гравець зверху
        if (playerBounds.min.y < zombieBounds.max.y - topStompWindow)
            return;

        // Raycast вниз щоб переконатись що перше зіткнення — це зомбі
        Vector2 rayOrigin = new Vector2(playerBounds.center.x, playerBounds.min.y + 0.05f);
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.down, raycastDistance);

        bool hitThisZombieFirst = false;

        for (int i = 0; i < hits.Length; i++)
        {
            Collider2D hitCollider = hits[i].collider;

            if (hitCollider == null || hitCollider.attachedRigidbody == targetRigidbody)
                continue;

            if (hitCollider == zombieCollider)
            {
                hitThisZombieFirst = true;
            }

            break;
        }

        if (!hitThisZombieFirst)
            return;

        // Обнуляємо падіння (щоб не було багів)
        if (clampFallingSpeed && velocity.y < 0f)
        {
            velocity.y = 0f;
            targetRigidbody.linearVelocity = velocity;
        }

        // Сам відскок
        targetRigidbody.AddForce(Vector2.up * bounceImpulse, ForceMode2D.Impulse);

        // Анімація
        //Animator targetAnimator = targetRigidbody.GetComponent<Animator>();
        //if (targetAnimator != null)
        //{
        //    targetAnimator.SetBool("IsJumping", true);
        //}
    }
}