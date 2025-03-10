using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScoreDataPersistence : MonoBehaviour
{
    public static ScoreDataPersistence Instance { get; private set; }

    // Data that persists between scenes
    public int currentScore;
    public int maxCombo;
    public float maxMultiplierReached;

    // Flag to indicate if data has been saved
    private bool dataSaved = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Call this before changing scenes to save the current score data.
    public void SaveScoreData()
    {
        if (ScoreManager.Instance != null)
        {
            currentScore = ScoreManager.Instance.score;
            
            // Update the highest multiplier reached if needed.
            if (ScoreManager.Instance.multiplier > maxMultiplierReached)
            {
                maxMultiplierReached = ScoreManager.Instance.multiplier;
            }
            
            dataSaved = true;
            Debug.Log($"Score data saved: Score={currentScore}, MaxMultiplier={maxMultiplierReached}");
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (dataSaved)
        {
            // Attempt to apply saved data when a new scene loads.
            StartCoroutine(ApplyDataWithRetry(5, 0.5f));
        }
    }

    private IEnumerator ApplyDataWithRetry(int attempts, float delay)
    {
        while (attempts > 0)
        {
            yield return new WaitForSeconds(delay);
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.score = currentScore;
                ScoreManager.Instance.displayedScore = currentScore;
                Debug.Log($"Score data applied: Score={currentScore}, MaxMultiplier={maxMultiplierReached}");
                yield break;
            }
            attempts--;
        }
        Debug.LogWarning("Failed to apply score data: ScoreManager not found");
    }

    // Provides the saved game results to other systems.
    public void GetGameResults(out int finalScore, out int finalMaxCombo, out float finalMaxMultiplier)
    {
        finalScore = currentScore;
        finalMaxCombo = maxCombo;
        finalMaxMultiplier = maxMultiplierReached;
    }

    // Reset method to clear persisted values.
    public void ResetScoreData()
    {
        currentScore = 0;
        maxCombo = 0;
        maxMultiplierReached = 0f; // Adjust if your initial multiplier should be different
        dataSaved = false;
    }

    // Centralized reset method that also resets the ScoreManager.
    public static void ResetGameData()
    {
        if (Instance != null)
        {
            Instance.ResetScoreData();
        }
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }
    }
}
