using System;
using System.Collections;
using UnityEngine;
using FMODUnity;
using Random = UnityEngine.Random;

public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance;

    [Header("Camera Shake Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeIntensity = 0.5f;

    [Header("Slow Motion Settings")]
    [SerializeField] private float slowMotionDuration = 0.6f;
    [SerializeField] private float slowMotionTimeScale = 0.1f;
    [SerializeField] private GameObject objectToToggle;
    
    [Header("Audio Settings")]
    [SerializeField] private string slowMotionSoundEvent = "event:/SlowMotionEffect";

    private Vector3 originalCameraPosition;

    private Coroutine shakeCoroutine;
    private Coroutine slowMotionCoroutine;
    public static event Action<bool> OnToggleObject;

    private void Awake()
    {
        // Typical singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera != null) originalCameraPosition = mainCamera.transform.position;
    }

    // PUBLIC entry point for shaking
    public void TriggerScreenShake()
    {
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        shakeCoroutine = StartCoroutine(ScreenShakeRoutine());
    }

    // PUBLIC entry point for slow motion + toggle
    public void TriggerSlowMotion()
    {
        StartCoroutine(SlowMotionRoutine());
    }

    private IEnumerator ScreenShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;
            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Restore camera
        mainCamera.transform.position = originalCameraPosition;
    }

    private IEnumerator SlowMotionRoutine()
    {
        // Play sound effect at the start of slow motion
        FMOD.Studio.EventInstance slowMoSound = RuntimeManager.CreateInstance(slowMotionSoundEvent);
        slowMoSound.start();
        
        OnToggleObject?.Invoke(true);
        Time.timeScale = slowMotionTimeScale;
        yield return new WaitForSecondsRealtime(slowMotionDuration);
        Time.timeScale = 1.0f;
        OnToggleObject?.Invoke(false);
        
        // Release the sound instance when done
        slowMoSound.release();
    }

    private void OnDisable()
    {
        // Clean up if the manager is disabled
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        if (slowMotionCoroutine != null) StopCoroutine(slowMotionCoroutine);

        if (mainCamera != null)
            mainCamera.transform.position = originalCameraPosition;
        Time.timeScale = 1f;
    }
}