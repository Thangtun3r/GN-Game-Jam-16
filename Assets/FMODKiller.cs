using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class FMODSceneManager : MonoBehaviour
{
    private static FMODSceneManager instance;

    private void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loading events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StopAllFMODEvents();
        Debug.Log("All FMOD events stopped due to scene transition");
    }

    // This can also be called manually when needed
    public void StopAllFMODEvents()
    {
        // Get all event instances
        Bus masterBus = RuntimeManager.GetBus("bus:/");
        masterBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
    
    // Static method to access this functionality from anywhere
    public static void StopAllEvents()
    {
        if (instance != null)
        {
            instance.StopAllFMODEvents();
        }
    }
}