using UnityEngine;

public class Interact_Item_Physics : MonoBehaviour
{
    public Rigidbody2D rigidBodyBall;

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
