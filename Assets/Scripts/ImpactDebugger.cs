using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ImpactDebugger : MonoBehaviour
{
    [Header("Debug Options")]
    [SerializeField] private bool logImpact = true; // Toggle logging to console
    [SerializeField] private bool drawImpactRay = true; // Toggle visual debugging
    [SerializeField] private Color impactColor = Color.red; // Debug line color
    [SerializeField] private float debugLineDuration = 0.5f; // Duration of the debug line
    [SerializeField] private float impactThreshold = 10.0f; // Impact power threshold
    [SerializeField] private LayerMask impactLayer; // Layer mask for specific layer

    [Header("Camera Shake Settings")]
    [SerializeField] private Camera mainCamera; // Reference to the main camera
    [SerializeField] private float shakeDuration = 0.5f; // Duration of the screen shake
    [SerializeField] private float shakeIntensity = 0.5f; // Intensity of the shake

    [Header("Slow Motion Settings")]
    [SerializeField] private float slowMotionDuration = 2.0f; // Duration of the slow motion
    [SerializeField] private float slowMotionTimeScale = 0.5f; // Time scale during slow motion
    [SerializeField] private GameObject objectToToggle; // Object to toggle during slow motion

    [Header("Damage Settings")]
    [SerializeField] private int damageAmount = 10; // Amount of damage to deal

    private Vector3 originalCameraPosition;
    private BaseTab baseTab;

    public static event Action OnImpactDetected;
    public static event Action<bool> OnToggleObject;

    private void Start()
    {
        mainCamera = Camera.main;
        baseTab = GetComponentInParent<BaseTab>();
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision object is in the specified layer
        if (((1 << collision.gameObject.layer) & impactLayer) != 0)
        {
            // Calculate the impact force using the impulse magnitude
            float impactPower = collision.relativeVelocity.magnitude * collision.rigidbody.mass;

            // Check if the impact power exceeds the threshold
            if ((impactPower > impactThreshold) && (baseTab.isThrowing == true))
            {
                OnImpactDetected?.Invoke();
                // Log impact force to console
                if (logImpact)
                {
                    Debug.Log($"Impact Detected! Object: {collision.gameObject.name}, Impact Power: {impactPower}");
                }

                // Draw impact force direction (optional visualization)
                if (drawImpactRay)
                {
                    Vector2 impactDirection = -collision.contacts[0].normal; // Opposite to collision normal
                    Debug.DrawRay(collision.contacts[0].point, impactDirection * impactPower * 0.1f, impactColor, debugLineDuration);
                }

                // Log "impact frame!" to console
                Debug.Log("Impact frame!");

                // Trigger screen shake
                if (mainCamera != null)
                {
                    StartCoroutine(ScreenShake());
                }

                // Start slow motion effect
                StartCoroutine(SlowMotionEffect());

                // Deal damage to the object if it implements IDamageable
                IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damageAmount);
                }
            }
        }
    }

    private IEnumerator ScreenShake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeIntensity;
            float y = Random.Range(-1f, 1f) * shakeIntensity;

            mainCamera.transform.position = originalCameraPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
    }

    private IEnumerator SlowMotionEffect()
    {
        // Toggle object on
        OnToggleObject?.Invoke(true);

        // Slow down time
        Time.timeScale = slowMotionTimeScale;
        yield return new WaitForSecondsRealtime(slowMotionDuration);

        // Restore time scale
        Time.timeScale = 1.0f;

        // Toggle object off
        OnToggleObject?.Invoke(false);
    }
}