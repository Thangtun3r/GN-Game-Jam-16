using UnityEngine;
using FMODUnity;
using UnityEngine.Video;  // Add this namespace

public class VideoAudioToFMOD : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    private FMOD.Studio.EventInstance fmodInstance;

    void Start()
    {
        // Ensure AudioSource is playing the video audio
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);

        // Create FMOD Event Instance
        fmodInstance = RuntimeManager.CreateInstance("event:/YourFMODEvent");

        // Attach Unity AudioSource to FMOD
        RuntimeManager.AttachInstanceToGameObject(fmodInstance, audioSource.transform);
        fmodInstance.start();
    }

    void OnDestroy()
    {
        fmodInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        fmodInstance.release();
    }
}