using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneLoader : MonoBehaviour
{
    [Tooltip("Name of the next scene to load")]
    [SerializeField] private string nextSceneName = "YourNextScene";
    
    [Tooltip("Use build index instead of scene name")]
    [SerializeField] private bool useSceneIndex = false;
    
    [Tooltip("Build index of the next scene")]
    [SerializeField] private int nextSceneIndex = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            LoadNextScene();
        }
    }
    
    public void LoadNextScene()
    {
        ScoreDataPersistence.ResetGameData();
        // Save score data before switching scenes if available
        if (ScoreDataPersistence.Instance != null)
        {
            ScoreDataPersistence.Instance.SaveScoreData();
        }
        
        // Load the next scene
        if (useSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}