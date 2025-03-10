using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity; // Add FMOD Unity namespace

public class CursorHit : MonoBehaviour
{
    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem hitParticleEffect;
    [SerializeField] private float particleLifetime = 1f;
    [SerializeField] private int poolSize = 20;
    [SerializeField] private float minCollisionForce = 3f;

    [Header("Sound Effects")]
    [SerializeField] private EventReference hitSound; // FMOD event reference

    private List<ParticleSystem> particlePool = new List<ParticleSystem>();
    private int currentIndex = 0;

    private void Awake()
    {
        // Make sure particle system exists
        if (hitParticleEffect == null)
        {
            Debug.LogWarning("Hit particle effect is not assigned to CursorHit component.");
            return;
        }

        // Make sure this object has a collider that is NOT a trigger
        Collider2D collider = GetComponent<Collider2D>();
        if (collider == null)
        {
            Debug.LogWarning("CursorHit requires a Collider2D component.");
        }
        else if (collider.isTrigger)
        {
            Debug.LogWarning("The Collider2D should NOT have isTrigger set to true for OnCollisionEnter2D.");
        }

        // Initialize particle pool
        InitializeParticlePool();
    }

    private void InitializeParticlePool()
    {
        if (hitParticleEffect == null) return;

        for (int i = 0; i < poolSize; i++)
        {
            ParticleSystem newParticle = Instantiate(hitParticleEffect);
            newParticle.gameObject.SetActive(false);
            particlePool.Add(newParticle);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collision force is strong enough
        if (collision.relativeVelocity.magnitude >= minCollisionForce)
        {
            // Get the contact point from the collision
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 contactPoint = contact.point;

            // Get collision normal and calculate rotation
            Vector2 contactNormal = contact.normal;
            float angle = Mathf.Atan2(contactNormal.y, contactNormal.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.Euler(0, 0, angle - 90);

            PlayHitEffect(contactPoint, rotation);
        }
    }

    private void PlayHitEffect(Vector2 position, Quaternion rotation)
    {
        // Play visual effect
        if (particlePool.Count > 0)
        {
            // Get next particle from pool
            ParticleSystem particleSystem = particlePool[currentIndex];

            // Position and activate the particle
            particleSystem.transform.position = position;
            particleSystem.transform.rotation = rotation;
            particleSystem.gameObject.SetActive(true);

            // Make sure any previous emissions are stopped
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.Play(true);

            // Schedule to return to pool
            StartCoroutine(ReturnToPool(particleSystem));

            // Update pool index
            currentIndex = (currentIndex + 1) % poolSize;
        }

        // Play sound effect at hit position
        if (!hitSound.IsNull)
        {
            RuntimeManager.PlayOneShot(hitSound, position);
        }
    }

    private IEnumerator ReturnToPool(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(particleLifetime);
        particleSystem.gameObject.SetActive(false);
    }
}