using UnityEngine;
        using FMODUnity;
        using FMOD.Studio;
        
        public class KeySoundTrigger : MonoBehaviour
        {
            [Header("Sound Settings")]
            [SerializeField] private string soundEventPath = "event:/YourSoundEvent";
            [Range(0, 1)]
            [SerializeField] private float volume = 1f;
        
            [Header("Keyboard Input Settings")]
            [SerializeField] private KeyCode triggerKey = KeyCode.Space;
            [SerializeField] private string buttonName = ""; // Optional: Use Unity's Input system button name
        
            [Header("Mouse Input Settings")]
            [SerializeField] private bool leftMouseButton = false;
            [SerializeField] private bool rightMouseButton = false;
            [SerializeField] private bool middleMouseButton = false;
        
            // Reference to the event instance
            private EventInstance soundEventInstance;
        
            private void OnDisable()
            {
                // Stop and release the sound if it's still playing when disabled
                StopAndReleaseSound();
            }
        
            private void Update()
            {
                // Check for key press
                if (Input.GetKeyDown(triggerKey))
                {
                    PlaySound();
                }
        
                // Check for button press if button name is specified
                if (!string.IsNullOrEmpty(buttonName) && Input.GetButtonDown(buttonName))
                {
                    PlaySound();
                }
        
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
                // Stop and release any existing instance
                StopAndReleaseSound();
        
                try
                {
                    // Create and play the sound event
                    soundEventInstance = RuntimeManager.CreateInstance(soundEventPath);
                    soundEventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
                    soundEventInstance.setVolume(volume);
                    soundEventInstance.start();
        
                    // Optional: Release the instance after it's done playing
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