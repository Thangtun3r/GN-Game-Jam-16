using UnityEngine;
using FMODUnity;

public class FMODOneShotEffect : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private string soundEventPath = "event:/SoundEffect";
    [Tooltip("Volume of the sound effect (0-1)")]
    [Range(0, 1)]
    [SerializeField] private float volume = 1f;

    private bool isFirstEnable = true; // Flag to track first enable

    private void OnEnable()
    {
        // Skip triggering sound on first enable (during pool initialization)
        if (isFirstEnable)
        {
            isFirstEnable = false;
            return;
        }

        // Play one-shot sound effect
        RuntimeManager.PlayOneShot(soundEventPath, transform.position);
    }
}