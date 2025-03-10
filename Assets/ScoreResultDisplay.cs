using UnityEngine;
using TMPro;
using System.Collections;
using FMODUnity;

public class ScoreResultsDisplay : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI maxComboText;
    public TextMeshProUGUI maxMultiplierText;

    [Header("Display Settings")]
    [Tooltip("Delay between showing each result value")]
    [SerializeField] private float displayDelay = 0.5f;
    [Tooltip("Duration of the counting animation")]
    [SerializeField] private float countDuration = 1.0f;

    [Header("Audio")]
    [EventRef]
    [SerializeField] private string resultSoundEvent = ""; // FMOD event path

    private void Start()
    {
        // Hide all text fields initially
        if (scoreText) scoreText.text = "";
        if (maxComboText) maxComboText.text = "";
        if (maxMultiplierText) maxMultiplierText.text = "";

        // Start the display sequence
        StartCoroutine(DisplayResultsSequentially());
    }

    private IEnumerator DisplayResultsSequentially()
    {
        yield return new WaitForSeconds(0.5f);

        if (ScoreDataPersistence.Instance != null)
        {
            int finalScore;
            int finalMaxCombo;
            float finalMaxMultiplier;

            ScoreDataPersistence.Instance.GetGameResults(out finalScore, out finalMaxCombo, out finalMaxMultiplier);

            // Display score with counting animation
            if (scoreText)
            {
                PlayResultSound();
                yield return StartCoroutine(CountNumber(scoreText, 0, finalScore, countDuration, false));
                yield return new WaitForSeconds(displayDelay);
            }

            // Display max combo with counting animation
            if (maxComboText)
            {
                PlayResultSound();
                yield return StartCoroutine(CountNumber(maxComboText, 0, finalMaxCombo, countDuration, false));
                yield return new WaitForSeconds(displayDelay);
            }

            // Display max multiplier with counting animation
            if (maxMultiplierText)
            {
                PlayResultSound();
                yield return StartCoroutine(CountNumber(maxMultiplierText, 0, finalMaxMultiplier, countDuration, true));
            }

            Debug.Log($"Displayed results: Score={finalScore}, MaxCombo={finalMaxCombo}, MaxMultiplier={finalMaxMultiplier}");
        }
        else
        {
            Debug.LogWarning("ScoreDataPersistence instance not found!");
        }
    }

    private IEnumerator CountNumber(TextMeshProUGUI textField, float startValue, float endValue, float duration, bool isFloat)
    {
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float currentValue = Mathf.Lerp(startValue, endValue, elapsedTime / duration);

            if (isFloat)
                textField.text = currentValue.ToString("0.0");
            else
                textField.text = Mathf.RoundToInt(currentValue).ToString();

            yield return null;
        }

        // Ensure we end with the exact target value
        if (isFloat)
            textField.text = endValue.ToString("0.0");
        else
            textField.text = Mathf.RoundToInt(endValue).ToString();
    }

    private void PlayResultSound()
    {
        if (!string.IsNullOrEmpty(resultSoundEvent))
        {
            RuntimeManager.PlayOneShot(resultSoundEvent);
        }
    }
}