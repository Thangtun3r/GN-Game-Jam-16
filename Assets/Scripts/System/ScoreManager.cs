using UnityEngine;
using TMPro;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("Audio")]
    [SerializeField] private string scoreIncreaseSoundPath = "event:/ScoreIncrease";
    [SerializeField] private string multiplierIncreaseSoundPath = "event:/MultiplierIncrease";
    [Range(0, 1)]
    [SerializeField] private float volume = 1f;

    private EventInstance scoreIncreaseInstance;
    private EventInstance multiplierIncreaseInstance;

    // Gameplay values
    public int score = 0;
    public float displayedScore = 0f;
    public float scoreAnimationSpeed = 5f;

    public float multiplier = 1f;
    public float multiplierProgress = 0f;

    [Header("Multiplier Settings")]
    public float initialProgressToNextMultiplier = 100f;
    public float multiplierThresholdGrowthFactor = 1.2f;
    private float progressToNextMultiplier = 0f;

    public int baseScore = 100;
    private float killIntervalTimer = 0f;
    public float killIntervalThreshold = 3f;

    public bool immediateResetOnDamage = true;

    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    [Header("Multiplier Scaling, Pop & Shake")]
    public float baseTextScale = 1.0f;
    public float maxTextScale = 2.5f;
    public float popScaleMultiplier = 1.2f;
    public float popSpeed = 0.15f;

    public float baseShakeMagnitude = 0.1f;
    public float maxShakeMagnitude = 5f;
    public float shakeFrequency = 0.05f;

    // Color shifting settings
    private float currentHue = 0f;
    public float hueShiftStep = 0.15f;
    public float saturation = 1f;
    public float value = 1f;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
        
        // Initialize progress to next multiplier from the inspector value.
        progressToNextMultiplier = initialProgressToNextMultiplier;
    }

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDeath += HandleEnemyDeath;
        CursorBeingDamage.OnDamageTaken += HandleCursorDamage;
        shakeCoroutine = StartCoroutine(ContinuousShakeEffect());
    }

    private void OnDisable()
    {
        EnemyHealth.OnEnemyDeath -= HandleEnemyDeath;
        CursorBeingDamage.OnDamageTaken -= HandleCursorDamage;
        if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
        StopAndReleaseSound(ref scoreIncreaseInstance);
        StopAndReleaseSound(ref multiplierIncreaseInstance);
    }

    private void StopAndReleaseSound(ref EventInstance instance)
    {
        if (instance.isValid())
        {
            instance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            instance.release();
        }
    }

    private void Update()
    {
        // Smoothly animate displayed score towards the actual score.
        displayedScore = Mathf.Lerp(displayedScore, score, scoreAnimationSpeed * Time.deltaTime);
        UpdateUI();

        killIntervalTimer += Time.deltaTime;
        if (killIntervalTimer > killIntervalThreshold)
        {
            if (multiplierProgress > 0f)
            {
                multiplierProgress -= baseScore * Time.deltaTime;
                multiplierProgress = Mathf.Max(0f, multiplierProgress);
            }
            else
            {
                ResetMultiplier();
            }
        }
    }

    private void HandleEnemyDeath()
    {
        // Increase score based on current multiplier.
        score += Mathf.RoundToInt(baseScore * multiplier);
        PlaySound(ref scoreIncreaseInstance, scoreIncreaseSoundPath);
        killIntervalTimer = 0f;
        multiplierProgress += baseScore;

        if (multiplierProgress >= progressToNextMultiplier)
        {
            multiplier += 0.5f;
            multiplierProgress = 0f;
            progressToNextMultiplier *= multiplierThresholdGrowthFactor;
            PlaySound(ref multiplierIncreaseInstance, multiplierIncreaseSoundPath);
            UpdateTextScale();
            ShiftMultiplierColor();
            StartCoroutine(AnimateMultiplierPop());
        }
    }

    private void PlaySound(ref EventInstance instance, string eventPath)
    {
        StopAndReleaseSound(ref instance);
        try
        {
            instance = RuntimeManager.CreateInstance(eventPath);
            instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
            instance.setVolume(volume);
            instance.start();
        }
        catch (EventNotFoundException)
        {
            Debug.LogWarning($"FMOD event not found: {eventPath}");
        }
    }

    private void HandleCursorDamage()
    {
        if (immediateResetOnDamage)
        {
            ResetMultiplier();
        }
        else
        {
            multiplier = Mathf.Max(1f, multiplier - 0.5f);
            multiplierProgress = 0f;
        }
    }

    private void ResetMultiplier()
    {
        multiplier = 1f;
        multiplierProgress = 0f;
        progressToNextMultiplier = initialProgressToNextMultiplier;
        killIntervalTimer = 0f;
        if (multiplierText != null)
            multiplierText.color = Color.white;
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            int roundedScore = Mathf.RoundToInt(displayedScore);
            scoreText.text = roundedScore.ToString();
        }

        if (multiplierText != null)
        {
            if (multiplier >= 1.5f)
            {
                multiplierText.text = "x" + multiplier.ToString("0.0");
                multiplierText.gameObject.SetActive(true);
            }
            else
            {
                multiplierText.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateTextScale()
    {
        if (multiplierText == null) return;
        float dynamicScale = Mathf.Lerp(baseTextScale, maxTextScale, (multiplier - 1f) / 10f);
        multiplierText.transform.localScale = Vector3.one * dynamicScale;
    }

    private IEnumerator ContinuousShakeEffect()
    {
        Transform txtTransform = multiplierText.transform;
        Vector3 originalPosition = txtTransform.localPosition;
        while (true)
        {
            float dynamicShakeMagnitude = Mathf.Lerp(baseShakeMagnitude, maxShakeMagnitude, (multiplier - 1f) / 10f);
            dynamicShakeMagnitude = Mathf.Max(0f, dynamicShakeMagnitude);
            Vector3 shakeOffset = Random.insideUnitSphere * dynamicShakeMagnitude;
            shakeOffset.z = 0f;
            txtTransform.localPosition = originalPosition + shakeOffset;
            yield return new WaitForSeconds(shakeFrequency);
        }
    }

    private IEnumerator AnimateMultiplierPop()
    {
        if (multiplierText == null) yield break;

        Transform txtTransform = multiplierText.transform;
        Vector3 originalScale = txtTransform.localScale;
        Vector3 targetScale = originalScale * popScaleMultiplier;
        float elapsed = 0f;

        while (elapsed < popSpeed)
        {
            txtTransform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / popSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        txtTransform.localScale = targetScale;

        elapsed = 0f;
        while (elapsed < popSpeed)
        {
            txtTransform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / popSpeed);
            elapsed += Time.deltaTime;
            yield return null;
        }
        txtTransform.localScale = originalScale;
    }

    private void ShiftMultiplierColor()
    {
        currentHue += hueShiftStep;
        if (currentHue > 1f) currentHue -= 1f;
        Color newColor = Color.HSVToRGB(currentHue, saturation, value);
        if (multiplierText != null)
            multiplierText.color = newColor;
    }

    // Reset method to clear score and multiplier data.
    public void ResetScore()
    {
        score = 0;
        displayedScore = 0f;
        multiplier = 1f;
        multiplierProgress = 0f;
        progressToNextMultiplier = initialProgressToNextMultiplier;
        killIntervalTimer = 0f;
        
        if (multiplierText != null)
        {
            multiplierText.color = Color.white;
            multiplierText.text = "";
        }
        
        if (scoreText != null)
        {
            scoreText.text = "0";
        }
    }
}
