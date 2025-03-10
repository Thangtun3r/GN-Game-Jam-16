using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenceManager : MonoBehaviour
{
    [Header("Scene Names")]
    [Tooltip("Name of the scene to load when the player loses")]
    [SerializeField] private string losingSceneName = "GameOver";
    
    [Tooltip("Name of the scene to load when the player wins")]
    [SerializeField] private string winningSceneName = "Victory";
    
    [Header("Transition Settings")]
    [Tooltip("Delay before loading the losing scene")]
    [SerializeField] private float sceneTransitionDelay = 1.5f;
    
    [Tooltip("Optional transition effect GameObject")]
    [SerializeField] private GameObject transitionEffect;
    
    private void OnEnable()
    {
        // Subscribe to game over events
        ProgressBarController.OnProgressBarFilled += HandleGameOver;
        HeartManager.OnAllHeartsLost += HandleGameOver;
        
        // Subscribe to victory event
        VideoSurvivalTimer.OnVideoTargetReached += HandleVictory;
    }
    
    
    private void OnDisable()
    {
        // Unsubscribe from game over events
        ProgressBarController.OnProgressBarFilled -= HandleGameOver;
        HeartManager.OnAllHeartsLost -= HandleGameOver;
        
        // Unsubscribe from victory event
        VideoSurvivalTimer.OnVideoTargetReached -= HandleVictory;
    }
    
    // Called when either game over condition is met
    private void HandleGameOver()
    {
        // Prevent double-triggering
        ProgressBarController.OnProgressBarFilled -= HandleGameOver;
        HeartManager.OnAllHeartsLost -= HandleGameOver;
        
        Debug.Log("Game Over triggered! Loading losing screen...");
        StartCoroutine(LoadScene(losingSceneName));
    }
    
    // Called when victory condition is met
    private void HandleVictory()
    {
        // Prevent double-triggering
        
        
        Debug.Log("Victory triggered! Loading winning screen...");
        ScoreDataPersistence.Instance.SaveScoreData();
        StartCoroutine(LoadScene(winningSceneName));
    }
    
    private IEnumerator LoadScene(string sceneName)
    {
        // Optional: Activate transition effect if assigned
        if (transitionEffect != null)
        {
            transitionEffect.SetActive(true);
        }
        
        // Optional: Slow down time for dramatic effect
        Time.timeScale = 0.5f;
        
        // Wait for the specified delay
        yield return new WaitForSecondsRealtime(sceneTransitionDelay);
        
        // Reset timescale before changing scene
        Time.timeScale = 1f;
        
        // Load the target scene
        SceneManager.LoadScene(sceneName);
    }
}