using UnityEngine;
using System;

public abstract class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    private TabDeathParticle deathParticle;

    // Make this a normal event, but add a public static method to raise it.
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

    protected virtual void Die()
    {
        // If we have a valid singleton instance, spawn the particle
        if (TabDeathParticlePool.Instance != null)
        {
            TabDeathParticlePool.Instance.GetParticleEffect(transform.position);
        }

        // Trigger the OnEnemyDeath event
        OnEnemyDeath?.Invoke();

        // Return this object to its pool (if needed)
        TabPool pool = FindObjectOfType<TabPool>();
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
    }


    // ➊ Add a static method that raises the event
    public static void RaiseOnEnemyDeath()
    {
        OnEnemyDeath?.Invoke();
    }
}