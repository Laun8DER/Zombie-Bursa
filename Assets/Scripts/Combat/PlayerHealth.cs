using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 5;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"[PlayerHealth] {gameObject.name}: получил {damage} урона. ХП: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            isDead = true;
            Debug.Log($"[PlayerHealth] {gameObject.name}: погиб.");
        }
    }
}
