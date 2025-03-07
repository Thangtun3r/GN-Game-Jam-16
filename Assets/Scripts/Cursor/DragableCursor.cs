using UnityEngine;

public class DragableCursor : MonoBehaviour
{
    [Header("Draggable Settings")]
    [SerializeField] private string draggableTag = "Tab";
    [SerializeField] private float throwForceMultiplier = 10f;
    [SerializeField] private float throwSpeed = 1f; // Adjustable throw speed factor
    [SerializeField] private LayerMask dragMask;

    [Header("Throw Force Limits")]
    [SerializeField] private float minThrowForce = 1f;  // Minimum impulse force to apply
    [SerializeField] private float maxThrowForce = 20f; // Maximum impulse force to apply

    private Rigidbody2D selectedRb = null;
    private BaseTab baseTab = null;
    private Vector2 dragOffset = Vector2.zero;
    private Vector2 lastMouseWorldPos;
    private Camera mainCamera;
    private float originalGravityScale;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            TryGrabObject(mouseWorldPos);
        }

        if (Input.GetMouseButton(0) && selectedRb != null)
        {
            DragObject(mouseWorldPos);
        }

        if (Input.GetMouseButtonUp(0) && selectedRb != null)
        {
            ReleaseObject(mouseWorldPos);
        }
    }

    private void TryGrabObject(Vector2 mouseWorldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0.1f, dragMask);

        if (hit.collider != null && hit.collider.CompareTag(draggableTag))
        {
            selectedRb = hit.collider.GetComponent<Rigidbody2D>();

            if (selectedRb != null)
            {
                // Store the original gravity scale and disable gravity during drag
                originalGravityScale = selectedRb.gravityScale;
                selectedRb.gravityScale = 0f;
                selectedRb.velocity = Vector2.zero;
                selectedRb.angularVelocity = 0f;

                // Calculate offset from mouse to object center
                dragOffset = (Vector2)selectedRb.transform.position - mouseWorldPos;
                lastMouseWorldPos = mouseWorldPos;

                // Disable BaseTab movement while dragging
                baseTab = hit.collider.GetComponent<BaseTab>();
                if (baseTab != null)
                {
                    baseTab.enabled = false;
                }
            }
        }
    }

    private void DragObject(Vector2 mouseWorldPos)
    {
        // Update object position
        selectedRb.MovePosition(mouseWorldPos + dragOffset);
        lastMouseWorldPos = mouseWorldPos;
    }

    private void ReleaseObject(Vector2 mouseWorldPos)
    {
        // Restore the original gravity scale before throwing
        selectedRb.gravityScale = originalGravityScale;

        // Calculate the impulse based on direct velocity
        Vector2 throwVelocity = (mouseWorldPos - lastMouseWorldPos) / Time.deltaTime;
        Vector2 impulse = throwVelocity * throwForceMultiplier * throwSpeed;

        // Clamp the impulse magnitude between the minimum and maximum force limits
        float clampedMagnitude = Mathf.Clamp(impulse.magnitude, minThrowForce, maxThrowForce);
        if (impulse != Vector2.zero)
        {
            impulse = impulse.normalized * clampedMagnitude;
        }

        // Apply the clamped impulse force
        selectedRb.AddForce(impulse, ForceMode2D.Impulse);

        // Re-enable BaseTab and trigger knockback to disable movement temporarily
        if (baseTab != null)
        {
            baseTab.enabled = true;
            baseTab.StartKnockback(0.5f); // Disables BaseTab movement for 0.5 seconds
        }

        // Clear references
        selectedRb = null;
        baseTab = null;
    }
}