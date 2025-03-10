using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoSurvivalTimer : MonoBehaviour
{
    // Event that triggers when video reaches target duration
    public static event Action OnVideoTargetReached;

    [Header("Video Settings")]
    [Tooltip("Reference to the VideoPlayer component")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Tooltip("Maximum duration to reach for victory (in seconds)")]
    [SerializeField] private float targetDuration = 60f;

    [Tooltip("If true, uses the full video length as the target duration")]
    [SerializeField] private bool useFullVideoLength = true;

    [Header("UI References")]
    [Tooltip("Optional UI Text to display the timer")]
    [SerializeField] private Text timerText;

    [Tooltip("Optional UI Image for a progress bar")]
    [SerializeField] private Image progressBar;

    [Header("Display Settings")]
    [Tooltip("Format for the timer display (e.g. mm:ss)")]
    [SerializeField] private string timeFormat = "mm:ss";

    private bool hasTriggeredVictoryEvent = false;
    private bool isInitialized = false;

    private void Awake()
    {
        if (videoPlayer == null)
        {
            videoPlayer = GetComponent<VideoPlayer>();
            if (videoPlayer == null)
            {
                Debug.LogError("VideoSurvivalTimer requires a VideoPlayer component!");
                enabled = false;
                return;
            }
        }
    }

    private void Start()
    {
        hasTriggeredVictoryEvent = false;
        
        // Wait for video to be prepared before initializing
        videoPlayer.prepareCompleted += OnVideoPrepared;
        
        if (videoPlayer.isPrepared)
        {
            OnVideoPrepared(videoPlayer);
        }
        else
        {
            videoPlayer.Prepare();
        }
    }

    private void OnVideoPrepared(VideoPlayer source)
    {
        // If set to use full video length, adjust the target duration
        if (useFullVideoLength)
        {
            targetDuration = (float)videoPlayer.length;
        }
        
        isInitialized = true;
        UpdateUI();
    }

    private void Update()
    {
        if (!isInitialized || !videoPlayer.isPlaying || hasTriggeredVictoryEvent)
            return;

        float currentTime = (float)videoPlayer.time;
            
        // Check if target duration is reached
        if (currentTime >= targetDuration && !hasTriggeredVictoryEvent)
        {
            hasTriggeredVictoryEvent = true;
            OnVideoTargetReached?.Invoke();
            Debug.Log("Video target duration reached! Victory!");
        }

        // Update UI elements
        UpdateUI();
    }

    public void SetTargetDuration(float newDuration)
    {
        targetDuration = Mathf.Max(0.1f, newDuration);
        useFullVideoLength = false;
        UpdateUI();
    }

    public float GetCurrentTime()
    {
        return videoPlayer != null ? (float)videoPlayer.time : 0f;
    }

    public float GetTargetDuration()
    {
        return targetDuration;
    }

    private void UpdateUI()
    {
        if (!isInitialized)
            return;
            
        float currentTime = (float)videoPlayer.time;
            
        // Update timer text if available
        if (timerText != null)
        {
            timerText.text = $"{FormatTime(currentTime)} / {FormatTime(targetDuration)}";
        }

        // Update progress bar if available
        if (progressBar != null)
        {
            progressBar.fillAmount = Mathf.Clamp01(currentTime / targetDuration);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60f);

        if (timeFormat == "mm:ss")
        {
            return string.Format("{0:00}:{1:00}", minutes, seconds);
        }
        else
        {
            return string.Format("{0}:{1:00}", minutes, seconds);
        }
    }
}