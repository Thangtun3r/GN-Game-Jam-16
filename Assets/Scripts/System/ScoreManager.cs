using UnityEngine;
using TMPro;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    // Gameplay values.
    public int score = 0;
    public float displayedScore = 0f; // This is what appears on the screen (smooth transition)
    public float scoreAnimationSpeed = 5f; // Adjust to control how fast the numbers catch up

    public float multiplier = 1f;
    public float multiplierProgress = 0f;
    public float progressToNextMultiplier = 100f;
    public int baseScore = 100;
    
    // Timer for kill interval.
    private float killIntervalTimer = 0f;
    public float killIntervalThreshold = 3f;
    
    public bool immediateResetOnDamage = true;

    // UI references.
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI multiplierText;

    // Pop, Shake & Scaling Settings
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
    }
    
    private void Update()
    {
        // Smoothly animate displayed score towards actual score
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
        score += Mathf.RoundToInt(baseScore * multiplier);
        killIntervalTimer = 0f;
        multiplierProgress += baseScore;
        
        if (multiplierProgress >= progressToNextMultiplier)
        {
            multiplier += 0.5f;
            multiplierProgress = 0f;
            progressToNextMultiplier *= 1.2f;

            UpdateTextScale();
            ShiftMultiplierColor();
            StartCoroutine(AnimateMultiplierPop());
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
        progressToNextMultiplier = 100f;
        killIntervalTimer = 0f;
        multiplierText.color = Color.white;
    }
    
    private void UpdateUI()
    {
        if (scoreText != null)
        {
            int roundedScore = Mathf.RoundToInt(displayedScore);
            scoreText.text = roundedScore.ToString(); // Smoothly increasing number
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
        multiplierText.color = newColor;
    }
}
