using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionLifetime = 2f;
    private ExplosiveTabExplosionPool pool;
    
    private void OnEnable()
    {
        // Start the countdown when enabled
        StartCoroutine(ExplosionCountdown());
    }
    
    // Set the reference to the pool when spawned
    public void SetPool(ExplosiveTabExplosionPool explosiveTabsPool)
    {
        pool = explosiveTabsPool;
    }
    
    private IEnumerator ExplosionCountdown()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(explosionLifetime);
        
        // Return to pool when countdown ends
        ReturnToPool();
    }
    
    private void ReturnToPool()
    {
        if (pool != null)
        {
            pool.ReturnToPool(this.gameObject);
        }
        else
        {
            // Fallback if pool reference is missing
            gameObject.SetActive(false);
            Debug.LogWarning("Explosion tried to return to pool, but pool reference was missing");
        }
    }
}