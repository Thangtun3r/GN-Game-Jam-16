using System.Collections;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Networking;

public class VideoLoaderForTutorial : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        // Update the file name here
        string fileName = "TutorialVid.mp4";
        string videoPath = GetVideoPath(fileName);

        // If running in WebGL, use UnityWebRequest to get the video path
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            StartCoroutine(LoadVideoFromWebGL(videoPath));
        }
        else
        {
            videoPlayer.url = videoPath;
            videoPlayer.Play();
        }
    }

    private string GetVideoPath(string fileName)
    {
        string path;
#if UNITY_WEBGL
        path = Application.streamingAssetsPath + "/" + fileName;
#else
        path = "file:///" + Application.streamingAssetsPath + "/" + fileName;
#endif
        return path;
    }

    IEnumerator LoadVideoFromWebGL(string url)
    {
        // Optionally check if the request is actually needed
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            videoPlayer.url = url;
            videoPlayer.Play();
        }
        else
        {
            Debug.LogError("Failed to load video: " + request.error);
        }
    }
}