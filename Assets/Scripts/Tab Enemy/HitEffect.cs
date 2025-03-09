using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private Transform visualsContainer; // Parent of all visual elements
    
    [Header("Sprite Swap Settings")]
    [SerializeField] private Sprite hitSprite; // White sprite to indicate hit
    [SerializeField] private float spriteSwapDuration = 0.2f; // Duration to show the hit sprite

    private Vector3 originalPosition;
    private bool isShaking = false;

    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private void Awake()
    {
        // If no container is assigned, use this GameObject
        if (visualsContainer == null)
        {
            visualsContainer = transform;
        }
        
        // Store the original position of the container
        originalPosition = visualsContainer.localPosition;
        
        // Get the SpriteRenderer from the container and store the original sprite
        spriteRenderer = visualsContainer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalSprite = spriteRenderer.sprite;
        }
    }

    private void OnDisable()
    {
        // Stop all coroutines to immediately stop any ongoing effects
        StopAllCoroutines();
        
        // Reset sprite to its original state if it was swapped
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = originalSprite;
        }
        
        // Reset the container's position to its original position
        if (visualsContainer != null)
        {
            visualsContainer.localPosition = originalPosition;
        }
        
        isShaking = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with a damage dealer
        if (collision.gameObject.GetComponent<CursorDamageDealer>() != null)
        {
            // Start the shake effect if not already shaking
            if (!isShaking)
            {
                StartCoroutine(ShakeEffect());
            }

            // Start the sprite swap effect if both the sprite renderer and hit sprite are available
            if (spriteRenderer != null && hitSprite != null)
            {
                StartCoroutine(SwapSpriteCoroutine(spriteSwapDuration));
            }
        }
    }

    private IEnumerator ShakeEffect()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Apply random offset to the container
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            visualsContainer.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Return container to original position
        visualsContainer.localPosition = originalPosition;
        isShaking = false;
    }

    private IEnumerator SwapSpriteCoroutine(float duration)
    {
        // Swap to the hit (white) sprite
        spriteRenderer.sprite = hitSprite;
        yield return new WaitForSeconds(duration);
        // Revert back to the original sprite
        spriteRenderer.sprite = originalSprite;
    }
}
