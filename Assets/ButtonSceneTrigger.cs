using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneManager : MonoBehaviour
{
    // This method takes a scene name as a parameter.
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            ScoreDataPersistence.ResetGameData();
            SceneManager.LoadScene(sceneName);
            
        }
        else
        {
            Debug.LogError("No scene name provided to load.");
        }
    }
}