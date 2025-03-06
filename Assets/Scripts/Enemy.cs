using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float health = 100f;
    public GameObject deathPrefab; // Optional death effect prefab
    private DamageFlash damageFlash;

    // Reference to the pool that spawned this enemy.
    [HideInInspector]
    public TabPool originatingPool;
    [HideInInspector]
    public bool isPooled = false;

    private void Start()
    {
        damageFlash = GetComponent<DamageFlash>();
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        Debug.Log(gameObject.name + " took " + damageAmount + " damage. Remaining health: " + health);

        if (damageFlash != null)
        {
            damageFlash.FlashWhite();
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has been destroyed!");

        // Spawn death effect if assigned
        if (deathPrefab != null)
        {
            Instantiate(deathPrefab, transform.position, Quaternion.identity);
        }

        // Return to pool if the enemy was spawned from one; otherwise, destroy.
        if (isPooled && originatingPool != null)
        {
            // Optionally reset enemy state here (e.g., health) before returning.
            originatingPool.ReturnToPool(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}