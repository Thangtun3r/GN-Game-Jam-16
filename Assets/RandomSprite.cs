using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    // Array to hold the list of sprites. You can set these in the Inspector.
    public Sprite[] sprites;

    // Reference to the SpriteRenderer component.
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        // Try to get the SpriteRenderer component from the current GameObject.
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("RandomSprite script requires a SpriteRenderer component on the same GameObject.");
        }
    }

    private void OnEnable()
    {
        // Check if the sprites array is not null and contains at least one sprite.
        if (sprites != null && sprites.Length > 0)
        {
            // Select a random index from the array.
            int randomIndex = Random.Range(0, sprites.Length);
            // Set the sprite to the randomly selected sprite.
            spriteRenderer.sprite = sprites[randomIndex];
        }
        else
        {
            Debug.LogWarning("No sprites assigned in the RandomSprite script.");
        }
    }
}