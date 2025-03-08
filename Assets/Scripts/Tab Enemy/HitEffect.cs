using UnityEngine;
using System.Collections;

public class HitEffect : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private Transform visualsContainer; // Parent of all visual elements

    private Vector3 originalPosition;
    private bool isShaking = false;

    private void Awake()
    {
        // If no container is assigned, use this GameObject
        if (visualsContainer == null)
        {
            visualsContainer = transform;
        }
        
        // Store the original position of the container
        originalPosition = visualsContainer.localPosition;
        
        if (visualsContainer == null)
        {
            Debug.LogWarning("HitEffect has no visual container to shake on " + gameObject.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with damage dealer
        if (collision.gameObject.GetComponent<CursorDamageDealer>() != null)
        {

            // Start the shake effect
            if (!isShaking)
            {
                StartCoroutine(ShakeEffect());
            }
        }
    }

    private IEnumerator ShakeEffect()
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            // Apply random offset to the entire container
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
}