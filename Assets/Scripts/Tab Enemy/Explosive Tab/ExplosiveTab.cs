using UnityEngine;
using System.Collections;

public class ExplosiveTab : BaseTab
{
    [Header("Explosive Settings")]
    [Tooltip("Delay (in seconds) before self-destruction occurs")]
    public float selfDestructDelay = 5f;

    [Tooltip("Warning threshold (0-1) at which the caution sprite is enabled (e.g., 0.5 = when 50% of time is left)")]
    public float cautionThreshold = 0.5f;

    [Tooltip("Minimum shake intensity (subtle shake)")]
    public float minShakeIntensity = 0.1f;

    [Tooltip("Maximum shake intensity (high shake near explosion)")]
    public float maxShakeIntensity = 1f;

    [Header("Flashing Effect Settings")]
    [Tooltip("Flash frequency in Hertz (flashes per second)")]
    public float flashFrequency = 2f;
    
    [Tooltip("Minimum opacity for the caution sprite during flashing")]
    public float minOpacity = 0.3f;
    
    [Tooltip("Maximum opacity for the caution sprite during flashing")]
    public float maxOpacity = 1f;

    [Tooltip("GameObject to use for explosive range caution (drag & drop in inspector)")]
    public GameObject explosiveRangeCaution;

    // Tracks whether we've already exploded (so we don't spawn the explosion twice).
    private bool _hasExploded = false;

    // Reference to the running self-destruct coroutine.
    private Coroutine _selfDestructCoroutine;

    // Timer pause flag and remaining time.
    private bool isTimerPaused = false;
    private float remainingTime;

    // Cache original positions and components.
    private Vector3 originalVisualPosition;
    private SpriteRenderer cautionSpriteRenderer;
    
    private void OnEnable()
    {
        // Prevent movement by setting speed to 0.
        speed = 0;

        _hasExploded = false;
        remainingTime = selfDestructDelay;

        // Ensure caution sprite is initially off and cache its sprite renderer.
        if (explosiveRangeCaution != null)
        {
            explosiveRangeCaution.SetActive(false);
            cautionSpriteRenderer = explosiveRangeCaution.GetComponent<SpriteRenderer>();
        }

        // Cache the original local position of the visual container.
        if (visualContainer != null)
            originalVisualPosition = visualContainer.transform.localPosition;

        _selfDestructCoroutine = StartCoroutine(SelfDestructTimer());
    }

    private void OnDisable()
    {
        if (_selfDestructCoroutine != null)
        {
            StopCoroutine(_selfDestructCoroutine);
            _selfDestructCoroutine = null;
        }
    }

    // Called by the drag methods to pause/resume the timer.
    public void PauseTimer()
    {
        isTimerPaused = true;
    }

    public void ResumeTimer()
    {
        isTimerPaused = false;
    }

    private IEnumerator SelfDestructTimer()
    {
        float elapsedTime = 0f;

        while (remainingTime > 0)
        {
            if (!isTimerPaused)
            {
                remainingTime -= Time.deltaTime;
                elapsedTime += Time.deltaTime;
            }

            // Calculate progress (0 at start, 1 when time is up) and interpolate shake intensity.
            float progress = 1 - (remainingTime / selfDestructDelay);
            float shakeIntensity = Mathf.Lerp(minShakeIntensity, maxShakeIntensity, progress);

            // Apply shaking effect to the visual container.
            if (visualContainer != null)
            {
                Vector2 offset = Random.insideUnitCircle * shakeIntensity;
                visualContainer.transform.localPosition = originalVisualPosition + new Vector3(offset.x, offset.y, 0);
            }

            // Toggle and animate the caution sprite based on the remaining time threshold.
            if (explosiveRangeCaution != null)
            {
                if ((remainingTime / selfDestructDelay) < cautionThreshold)
                {
                    if (!explosiveRangeCaution.activeSelf)
                        explosiveRangeCaution.SetActive(true);

                    // Calculate alpha using a sine wave based on frequency.
                    float alpha = Mathf.Lerp(minOpacity, maxOpacity, (Mathf.Sin(2 * Mathf.PI * flashFrequency * elapsedTime) + 1f) / 2f);
                    if (cautionSpriteRenderer != null)
                    {
                        Color col = cautionSpriteRenderer.color;
                        col.a = alpha;
                        cautionSpriteRenderer.color = col;
                    }
                }
                else
                {
                    // Before threshold, ensure the caution sprite is disabled.
                    if (explosiveRangeCaution.activeSelf)
                        explosiveRangeCaution.SetActive(false);
                }
            }

            yield return null;
        }

        // Reset visual container position.
        if (visualContainer != null)
            visualContainer.transform.localPosition = originalVisualPosition;

        SelfDestruct();
    }

    /// <summary>
    /// Called externally to kill the tab.
    /// If awardScore=true, the player gets score; if false, no score is awarded.
    /// </summary>
    public void Die(bool awardScore)
    {
        if (_hasExploded) return;  

        if (_selfDestructCoroutine != null)
        {
            StopCoroutine(_selfDestructCoroutine);
            _selfDestructCoroutine = null;
        }

        if (awardScore)
        {
            DieWithScore();
        }
        else
        {
            SelfDestruct();
        }
    }

    private void DieWithScore()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        EnemyHealth.RaiseOnEnemyDeath();
        SpawnExplosion();
        ReturnToPool();
    }

    private void SelfDestruct()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        SpawnExplosion();
        ReturnToPool();
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

    private void ReturnToPool()
    {
        TabPool pool = FindObjectOfType<TabPool>();
        if (pool != null)
        {
            pool.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
