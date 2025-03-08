using UnityEngine;
using System;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    private TabDeathParticle deathParticle;

    // Static event to notify when an enemy dies.
    public static event Action OnEnemyDeath;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;
        deathParticle = GetComponent<TabDeathParticle>();
        
        if (deathParticle == null)
        {
            Debug.LogWarning("TabDeathParticle component missing on " + gameObject.name);
            deathParticle = gameObject.AddComponent<TabDeathParticle>();
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Vector3 deathPosition = transform.position;
        if (deathParticle != null)
        {
            deathParticle.SpawnDeathParticle();
        }
        
        // Notify subscribers that this enemy has died.
        OnEnemyDeath?.Invoke();

        TabPool pool = FindObjectOfType<TabPool>();
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
    }
}