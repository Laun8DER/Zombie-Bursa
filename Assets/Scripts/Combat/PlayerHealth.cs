using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Settings")]
    [Range(1, 3)]
    public int maxHealth = 3;

    private int currentHealth;
    private bool isDead;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    public event Action<int, int> HealthChanged;
    public event Action Died;

    private void Awake()
    {
        maxHealth = Mathf.Clamp(maxHealth, 1, 3);
        currentHealth = maxHealth;
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (damage <= 0)
        {
            return;
        }

        if (currentHealth <= 0)
        {
            Debug.Log($"[PlayerHealth] {gameObject.name}: получил фатальный удар без сердец.");
            Die();
            return;
        }

        int previousHealth = currentHealth;
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        if (currentHealth != previousHealth)
        {
            HealthChanged?.Invoke(currentHealth, maxHealth);
        }

        Debug.Log($"[PlayerHealth] {gameObject.name}: получил {damage} урона. ХП: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        Died?.Invoke();
        Debug.Log($"[PlayerHealth] {gameObject.name}: погиб.");
    }
}
