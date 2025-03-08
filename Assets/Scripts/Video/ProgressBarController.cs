using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [Tooltip("Tag of the objects that will trigger the progress bar fill.")]
    public string targetTag = "Target";

    [Tooltip("UI Image that represents the progress bar (set its Image Type to Filled).")]
    public Image progressBar;

    [Tooltip("Fill rate per second when a valid object is inside the collider.")]
    public float fillRate = 0.2f;

    [Tooltip("Cooldown rate per second when no valid object is inside the collider.")]
    public float cooldownRate = 0.1f;

    [Header("Shake Settings")]
    [Tooltip("GameObject to shake when the progress bar is filling.")]
    public GameObject shakeObject;
    [Tooltip("The magnitude of the shake effect (maximum offset in local space).")]
    public float shakeMagnitude = 0.1f;
    [Tooltip("Frequency of shake oscillation in Hz (how many shake updates per second).")]
    public float shakeFrequency = 10f;
    [Tooltip("Speed at which the object returns to its original position when not shaking.")]
    public float shakeReturnSpeed = 5f;

    // Flag indicating whether a valid object is detected
    private bool isObjectDetected = false;

    // Store the original local position of the shake object
    private Vector3 defaultPosition;

    private void Start()
    {
        if (shakeObject != null)
        {
            defaultPosition = shakeObject.transform.localPosition;
        }
    }

    // Called while an object remains within the trigger collider
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isObjectDetected = true;
        }
    }

    // Called when an object exits the trigger collider
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            isObjectDetected = false;
        }
    }

    private void Update()
    {
        // Update the progress bar's fill amount
        if (isObjectDetected)
        {
            progressBar.fillAmount = Mathf.Clamp01(progressBar.fillAmount + fillRate * Time.deltaTime);
        }
        else
        {
            progressBar.fillAmount = Mathf.Clamp01(progressBar.fillAmount - cooldownRate * Time.deltaTime);
        }

        // Shake effect: only applied if shakeObject is assigned
        if (shakeObject != null)
        {
            if (isObjectDetected)
            {
                // Use Perlin noise to generate a smooth, adjustable shake offset
                float offsetX = (Mathf.PerlinNoise(Time.time * shakeFrequency, 0f) - 0.5f) * 2f * shakeMagnitude;
                float offsetY = (Mathf.PerlinNoise(0f, Time.time * shakeFrequency) - 0.5f) * 2f * shakeMagnitude;
                shakeObject.transform.localPosition = defaultPosition + new Vector3(offsetX, offsetY, 0f);
            }
            else
            {
                // Smoothly return the object's position back to its original state when not shaking
                shakeObject.transform.localPosition = Vector3.Lerp(shakeObject.transform.localPosition, defaultPosition, Time.deltaTime * shakeReturnSpeed);
            }
        }
    }
}
