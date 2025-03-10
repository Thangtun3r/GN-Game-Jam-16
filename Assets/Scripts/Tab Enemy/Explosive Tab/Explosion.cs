using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionLifetime = 2f;
    [SerializeField] private float colliderActiveTime = 0.2f;
    [SerializeField] private Collider2D explosionCollider;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private EventReference explosionSound; // FMOD event reference
    private EventInstance explosionInstance;
    private ExplosiveTabExplosionPool pool;

    private void Awake()
    {
        // If collider isn't assigned in the inspector, try to get it
        if (explosionCollider == null)
        {
            explosionCollider = GetComponent<Collider2D>();
        }

        // If particle system isn't assigned in the inspector, try to get it
        if (explosionEffect == null)
        {
            explosionEffect = GetComponent<ParticleSystem>();
        }
    }

    private void OnEnable()
    {
        // Ensure collider is enabled when object is activated
        if (explosionCollider != null)
        {
            explosionCollider.enabled = true;
        }

        // Play particle system when enabled
        if (explosionEffect != null)
        {
            explosionEffect.Play();
        }
        
        // Play explosion sound
        PlayExplosionSound();

        // Start the countdown when enabled
        StartCoroutine(ExplosionCountdown());

        // Start the collider disabling countdown
        StartCoroutine(DisableColliderCountdown());
    }
    
    private void OnDisable()
    {
        // Stop explosion sound when disabled
        StopExplosionSound();
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

    private IEnumerator DisableColliderCountdown()
    {
        // Wait for the specified collider active time
        yield return new WaitForSeconds(colliderActiveTime);

        // Disable the collider after the active time
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
        }
    }

    private void ReturnToPool()
    {
        StopExplosionSound();
        
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
    
    private void PlayExplosionSound()
    {
        if (!explosionSound.IsNull)
        {
            explosionInstance = RuntimeManager.CreateInstance(explosionSound);
            explosionInstance.start();
        }
    }
    
    private void StopExplosionSound()
    {
        if (explosionInstance.isValid())
        {
            explosionInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            explosionInstance.release();
        }
    }
}