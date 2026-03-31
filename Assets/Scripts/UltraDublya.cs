using UnityEngine;

public class UltraDublya : MonoBehaviour
{
    public GameObject tuffPrefab; 

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(tuffPrefab, transform.position, Quaternion.identity);
        }
    }
}
}
