using UnityEngine;
using System.Collections;

public class DelayEnemyHealth : EnemyHealth, IDamageable
{
    [Tooltip("Time delay (in seconds) before the enemy is actually disabled after death is triggered.")]
    public float dieDelay = 2f;

    [Tooltip("Should this enemy spawn an explosion effect when it dies?")]
    public bool spawnExplosion = true;

    [Header("Visual Warning Effects")]
    [Tooltip("GameObject to use for explosive range caution (drag & drop in inspector)")]
    public GameObject explosiveRangeCaution;

    [Tooltip("Flash frequency in Hertz (flashes per second)")]
    public float flashFrequency = 2f;

    [Tooltip("Minimum opacity for the caution sprite during flashing")]
    public float minOpacity = 0.3f;

    [Tooltip("Maximum opacity for the caution sprite during flashing")]
    public float maxOpacity = 1f;

    [Tooltip("Minimum shake intensity (subtle shake)")]
    public float minShakeIntensity = 0.05f;

    [Tooltip("Maximum shake intensity (high shake near explosion)")]
    public float maxShakeIntensity = 0.15f;

    // A flag to ensure the death routine is only triggered once.
    private bool isDying = false;
    private bool skipDelay = false;

    [Tooltip("Assign the transform of the sprite that should be shaken on death. If not assigned, the object's transform is used.")]
    public Transform spriteToShake;

    private Vector3 originalPos;
    private SpriteRenderer cautionSpriteRenderer;

    private void Start()
    {
        // Cache the original position
        if (spriteToShake != null)
            originalPos = spriteToShake.localPosition;
        else
            originalPos = transform.localPosition;

        // Cache the caution sprite renderer if available
        if (explosiveRangeCaution != null)
        {
            cautionSpriteRenderer = explosiveRangeCaution.GetComponent<SpriteRenderer>();
            explosiveRangeCaution.SetActive(false);
        }
    }

    // Explicit interface implementation to catch ImpactDebugger calls
    void IDamageable.TakeDamage(int damage)
    {
        // Check if this call is from ImpactDebugger
        // We can use callstack depth or caller detection if needed
        skipDelay = true;
        
        // Call base TakeDamage
        base.TakeDamage(damage);
        
        // Reset flag
        skipDelay = false;
    }

    // Override Die to check if we should skip delay
    protected override void Die()
    {
        if (isDying)
            return;

        isDying = true;

        if (skipDelay)
        {
            // Skip delay for ImpactDebugger
            if (spawnExplosion)
            {
                SpawnExplosion();
            }
            base.Die();
        }
        else
        {
            // Use delay for normal damage
            StartCoroutine(DelayedDeath());
        }
    }

    private IEnumerator DelayedDeath()
    {
        // Enable the caution indicator if available
        if (explosiveRangeCaution != null)
            explosiveRangeCaution.SetActive(true);

        float elapsed = 0f;

        // Flash and shake until the delay is over
        while (elapsed < dieDelay)
        {
            // Calculate progress (0 at start, 1 when time is up)
            float progress = elapsed / dieDelay;
            float shakeIntensity = Mathf.Lerp(minShakeIntensity, maxShakeIntensity, progress);

            // Apply shake effect
            Transform target = spriteToShake != null ? spriteToShake : transform;
            Vector2 offset = Random.insideUnitCircle * shakeIntensity;
            target.localPosition = originalPos + new Vector3(offset.x, offset.y, 0);

            // Apply flashing effect to caution sprite
            if (cautionSpriteRenderer != null)
            {
                float alpha = Mathf.Lerp(minOpacity, maxOpacity,
                    (Mathf.Sin(2 * Mathf.PI * flashFrequency * elapsed) + 1f) / 2f);
                Color color = cautionSpriteRenderer.color;
                color.a = alpha;
                cautionSpriteRenderer.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset position
        Transform resetTarget = spriteToShake != null ? spriteToShake : transform;
        resetTarget.localPosition = originalPos;

        // Spawn explosion if enabled
        if (spawnExplosion)
        {
            SpawnExplosion();
        }

        // Call base EnemyHealth.Die() to handle destruction properly
        base.Die();
    }

    private void SpawnExplosion()
    {
        if (ExplosiveTabExplosionPool.Instance != null)
        {
            GameObject explosion = ExplosiveTabExplosionPool.Instance.GetExplosion();
            explosion.transform.position = transform.position;
        }
        else
        {
            Debug.LogWarning("No ExplosiveTabExplosionPool found (singleton Instance is null).");
        }
    }
}