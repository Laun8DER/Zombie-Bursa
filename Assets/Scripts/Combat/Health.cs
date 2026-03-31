using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public Animator animator;
    private bool isDead = false;

    [Header("Death Effects")]
    public GameObject deathEffect;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Метод для получения урона. Вызывается из скриптов атаки.
    /// </summary>
    /// <param name="damage">Количество урона.</param>
    public void TakeDamage(int damage)
    {
        if (isDead) return;
        currentHealth -= damage;

        //Debug.Log($"{gameObject.name} получил {damage} урона. Текущее ХП: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log($"{gameObject.name} погиб!");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        animator.SetTrigger("IsDead");
    }
    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
