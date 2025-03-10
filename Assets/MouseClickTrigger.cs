using UnityEngine;
            using FMODUnity;
            using FMOD.Studio;
            
            public class MouseClickSoundTrigger : MonoBehaviour
            {
                [Header("Sound Settings")]
                [SerializeField] private string soundEventPath = "event:/MouseClickSound";
                [Range(0, 1)]
                [SerializeField] private float volume = 1f;
            
                [Header("Mouse Button Settings")]
                [SerializeField] private bool leftMouseButton = true;
                [SerializeField] private bool rightMouseButton = false;
                [SerializeField] private bool middleMouseButton = false;
            
                // Reference to the event instance
                private EventInstance soundEventInstance;
            
                private void OnDisable()
                {
                    StopAndReleaseSound();
                }
            
                private void Update()
                {
                    // Check for mouse button presses
                    if ((leftMouseButton && Input.GetMouseButtonDown(0)) ||
                        (rightMouseButton && Input.GetMouseButtonDown(1)) ||
                        (middleMouseButton && Input.GetMouseButtonDown(2)))
                    {
                        PlaySound();
                    }
                }
            
                private void PlaySound()
                {
                    StopAndReleaseSound();
            
                    try
                    {
                        soundEventInstance = RuntimeManager.CreateInstance(soundEventPath);
                        soundEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                        soundEventInstance.setVolume(volume);
                        soundEventInstance.start();
                        soundEventInstance.release();
                    }
                    catch (EventNotFoundException)
                    {
                        Debug.LogWarning($"FMOD event not found: {soundEventPath}");
                    }
                }
            
                private void StopAndReleaseSound()
                {
                    if (soundEventInstance.isValid())
                    {
                        soundEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                        soundEventInstance.release();
                    }
                }
            }