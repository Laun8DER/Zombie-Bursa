using UnityEngine;

public class ZombieKFC_Health : MonoBehaviour
{
    [Header("Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    public Animator animator;
    private bool isDead = false;
    public bool IsDead => isDead;

    public AudioSource audioSource;
    public AudioClip explosionClip;

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
        audioSource.PlayOneShot(explosionClip);
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
