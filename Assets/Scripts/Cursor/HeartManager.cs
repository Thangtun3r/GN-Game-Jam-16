using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartManager : MonoBehaviour
{
    [Header("Pre-Placed Heart Objects")]
    [Tooltip("Assign your heart GameObjects in the order you want them to be disabled.")]
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();
    
    [Header("Heart Animation Settings")]
    [SerializeField] private float shakeAmount = 0.1f;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float blinkRate = 0.5f; // Time between blinks

    private int currentHearts;
    private HashSet<int> blinkingHearts = new HashSet<int>();
    private Dictionary<int, Vector3> originalPositions = new Dictionary<int, Vector3>();
    private Dictionary<int, Coroutine> blinkCoroutines = new Dictionary<int, Coroutine>();

    private void Start()
    {
        // Set current hearts count to the number of assigned heart objects.
        currentHearts = hearts.Count;
        
        // Store original positions of hearts
        for (int i = 0; i < hearts.Count; i++)
        {
            originalPositions[i] = hearts[i].transform.localPosition;
        }
    }

    private void OnEnable()
    {
        // Subscribe to the damage event.
        CursorBeingDamage.OnDamageTaken += RemoveHeart;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks.
        CursorBeingDamage.OnDamageTaken -= RemoveHeart;
    }

    // Modified to handle two-stage heart removal
    public void RemoveHeart()
    {
        if (currentHearts <= 0)
        {
            Debug.Log("No hearts left! Player might be dead.");
            // Optionally, trigger game over logic here.
            return;
        }

        int heartIndex = currentHearts - 1;

        // If heart is already blinking, fully remove it
        if (blinkingHearts.Contains(heartIndex))
        {
            blinkingHearts.Remove(heartIndex);
            StopBlinkingAndShaking(heartIndex);
            hearts[heartIndex].SetActive(false);
            currentHearts--;
        }
        // Otherwise, make it blink
        else
        {
            blinkingHearts.Add(heartIndex);
            StartCoroutine(ShakeHeart(heartIndex));
            blinkCoroutines[heartIndex] = StartCoroutine(BlinkHeart(heartIndex));
        }
    }

    private IEnumerator ShakeHeart(int heartIndex)
    {
        GameObject heart = hearts[heartIndex];
        Vector3 originalPos = originalPositions[heartIndex];
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeAmount;
            heart.transform.localPosition = originalPos + new Vector3(randomOffset.x, randomOffset.y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore original position
        heart.transform.localPosition = originalPos;
    }

    private IEnumerator BlinkHeart(int heartIndex)
    {
        GameObject heart = hearts[heartIndex];
        
        // Keep blinking until the heart is fully removed
        while (blinkingHearts.Contains(heartIndex))
        {
            heart.SetActive(!heart.activeSelf);
            yield return new WaitForSeconds(blinkRate);
        }
    }
    
    private void StopBlinkingAndShaking(int heartIndex)
    {
        // Ensure heart is visible before full deactivation
        hearts[heartIndex].SetActive(true);
        
        // Stop blinking coroutine
        if (blinkCoroutines.ContainsKey(heartIndex))
        {
            StopCoroutine(blinkCoroutines[heartIndex]);
            blinkCoroutines.Remove(heartIndex);
        }
        
        // Reset position
        hearts[heartIndex].transform.localPosition = originalPositions[heartIndex];
    }
}