using UnityEngine;

public class Ball_Physics : MonoBehaviour
{
    public Rigidbody2D rigidBodyBall;
    private float forceStrength = 0.5f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rigidBodyBall.bodyType = RigidbodyType2D.Dynamic;
            rigidBodyBall.gravityScale = 1;

            Vector2 direction = (transform.position - collision.transform.position).normalized;

        }
    }
}
