using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SwitchSceneOnVideoEnd : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private string nextSceneName = "NextScene";

    private void Start()
    {
        // Make sure the VideoPlayer is assigned in the Inspector.
        if (videoPlayer == null)
        {
            Debug.LogError("No VideoPlayer assigned to SwitchSceneOnVideoEnd script.");
            return;
        }
        
        // Subscribe to the loopPointReached event of the VideoPlayer.
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    private void OnDestroy()
    {
        // Unsubscribe to avoid potential memory leaks.
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    // This method is called when the VideoPlayer hits the end of the video.
    private void OnVideoEnd(VideoPlayer vp)
    {
        ScoreDataPersistence.Instance.SaveScoreData(); 
        // Load the next scene when the video finishes.
        SceneManager.LoadScene(nextSceneName);
    }
}