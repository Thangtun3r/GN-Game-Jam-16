using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Video : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // Reference to the VideoPlayer component
    public Image progressBar;        // UI Fill Image for progress

    void Update()
    {
        if (videoPlayer != null && videoPlayer.length > 0)
        {
            // Calculate video progress (0 to 1)
            float progress = (float)(videoPlayer.time / videoPlayer.length);

            // Update the UI Fill Image
            if (progressBar != null)
            {
                progressBar.fillAmount = progress;
            }
        }
    }
}