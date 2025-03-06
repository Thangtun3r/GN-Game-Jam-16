using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    public float flashDuration = 0.1f; // Duration of white flash

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
            return;
        }

        originalColor = spriteRenderer.color; // Store original color
    }

    public void FlashWhite()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white; // Change to white
            Invoke(nameof(ResetColor), flashDuration); // Reset after delay
        }
    }

    private void ResetColor()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor; // Restore original color
        }
    }
}