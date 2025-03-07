using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    public int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
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
        // Instead of destroying the enemy, return it to the pool.
        TabPool pool = FindObjectOfType<TabPool>();
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
    }
}