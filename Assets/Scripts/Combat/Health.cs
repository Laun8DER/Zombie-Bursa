using UnityEngine;

public class Health : MonoBehaviour
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

    [Header("Loot Drop")]
    public GameObject[] energyDrinkPrefabs;
    public float[] energyDrinkSpawnChances;

    private bool hasSpawnedEnergyDrink;

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
        SpawnEnergyDrink();
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    private void SpawnEnergyDrink()
    {
        if (hasSpawnedEnergyDrink)
        {
            return;
        }

        int selectedIndex = GetRandomEnergyDrinkIndex();
        if (selectedIndex < 0)
        {
            return;
        }

        GameObject energyDrinkPrefab = energyDrinkPrefabs[selectedIndex];
        if (energyDrinkPrefab == null)
        {
            return;
        }

        Object prefabToSpawn = energyDrinkPrefab;
        Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
        hasSpawnedEnergyDrink = true;
    }

    private int GetRandomEnergyDrinkIndex()
    {
        if (energyDrinkPrefabs == null || energyDrinkSpawnChances == null)
        {
            return -1;
        }

        int entryCount = Mathf.Min(energyDrinkPrefabs.Length, energyDrinkSpawnChances.Length);
        if (entryCount == 0)
        {
            return -1;
        }

        int successfulRollCount = 0;
        int lastSuccessfulIndex = -1;

        for (int i = 0; i < entryCount; i++)
        {
            if (energyDrinkPrefabs[i] == null)
            {
                continue;
            }

            float dropChance = Mathf.Clamp(energyDrinkSpawnChances[i], 0f, 100f);
            if (dropChance <= 0f)
            {
                continue;
            }

            if (Random.Range(0f, 100f) <= dropChance)
            {
                successfulRollCount++;
                if (Random.Range(0, successfulRollCount) == 0)
                {
                    lastSuccessfulIndex = i;
                }
            }
        }

        return lastSuccessfulIndex;
    }
}
