using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameObject videoUI; // e.g. the parent panel or RawImage for the video

    private void Start()
    {
        // Hide the video UI initially
        if (videoUI != null)
            videoUI.SetActive(false);

        // Subscribe to the prepareCompleted event
        videoPlayer.prepareCompleted += OnVideoPrepared;

        // Manually prepare the video
        videoPlayer.Prepare();
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // Now that the video is prepared, we can safely show the UI
        if (videoUI != null)
            videoUI.SetActive(true);

        // Play the video after preparation
        videoPlayer.Play();
    }
}