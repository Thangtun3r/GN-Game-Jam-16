using System.Collections;
using UnityEngine;
using FMODUnity;

public class DelayedObjectEnabler : MonoBehaviour
{
    [Header("Target Objects")]
    [Tooltip("The GameObject to enable after delay")]
    [SerializeField] private GameObject targetObject;
    
    [Tooltip("The Confetti GameObject to enable after its own delay")]
    [SerializeField] private GameObject confettiObject;

    [Header("Delay Settings")]
    [Tooltip("Time in seconds before enabling the target object")]
    [SerializeField] private float enableDelay = 2.0f;
    
    [Tooltip("Time in seconds before enabling the confetti object")]
    [SerializeField] private float confettiDelay = 3.0f;

    [Header("Audio")]
    [EventRef]
    [Tooltip("FMOD event to play when object is enabled")]
    [SerializeField] private string enableSoundEvent = "";

    private void OnEnable()
    {
        // Handle main target object
        if (targetObject != null)
        {
            targetObject.SetActive(false);
            StartCoroutine(EnableAfterDelay(targetObject, enableDelay, true));
        }
        else
        {
            Debug.LogWarning("No target object assigned to DelayedObjectEnabler!");
        }
        
        // Handle confetti object
        if (confettiObject != null)
        {
            confettiObject.SetActive(false);
            StartCoroutine(EnableAfterDelay(confettiObject, confettiDelay, false));
        }
    }

    private IEnumerator EnableAfterDelay(GameObject obj, float delay, bool playSound)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Enable the target object
        if (obj != null)
        {
            obj.SetActive(true);

            // Play the sound effect if specified
            if (playSound)
            {
                PlayEnableSound();
            }

            Debug.Log($"Enabled object: {obj.name} after {delay} seconds");
        }
    }

    private void PlayEnableSound()
    {
        if (!string.IsNullOrEmpty(enableSoundEvent))
        {
            RuntimeManager.PlayOneShot(enableSoundEvent);
        }
    }
}