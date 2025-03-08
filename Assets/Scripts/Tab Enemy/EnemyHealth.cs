using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;
    private TabDeathParticle deathParticle;

    private void Start()
    {
        currentHealth = maxHealth;
        deathParticle = GetComponent<TabDeathParticle>();
        
        // Check if component exists
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
        // Spawn particle effect
        Vector3 deathPosition = transform.position;
        
        if (deathParticle != null)
        {
            deathParticle.SpawnDeathParticle();
        }
        
        // Return enemy to the pool
        TabPool pool = FindObjectOfType<TabPool>();
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
    }
}