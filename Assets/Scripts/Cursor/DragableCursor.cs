using UnityEngine;

public class DragableCursor : MonoBehaviour
{
    [Header("Draggable Settings")]
    [SerializeField] private string draggableTag = "Tab";
    [SerializeField] private float throwForceMultiplier = 10f;
    [SerializeField] private float throwSpeed = 1f;
    [SerializeField] private LayerMask dragMask;

    [Header("Throw Force Limits")]
    [SerializeField] private float minThrowForce = 1f;
    [SerializeField] private float maxThrowForce = 20f;

    private IDragable selectedDragable = null;
    private Rigidbody2D selectedRb = null;
    private Vector2 dragOffset = Vector2.zero;
    private Vector2 lastMouseWorldPos;
    private Camera mainCamera;

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

        if (Input.GetMouseButton(0) && selectedDragable != null)
        {
            DragObject(mouseWorldPos);
        }

        if (Input.GetMouseButtonUp(0) && selectedDragable != null)
        {
            ReleaseObject(mouseWorldPos);
        }
    }

    private void TryGrabObject(Vector2 mouseWorldPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, 0.1f, dragMask);

        if (hit.collider != null && hit.collider.CompareTag(draggableTag))
        {
            selectedDragable = hit.collider.GetComponent<IDragable>();

            if (selectedDragable != null)
            {
                selectedRb = selectedDragable.GetRigidbody();
                
                // Calculate offset from mouse to object center
                dragOffset = (Vector2)selectedDragable.GetTransform().position - mouseWorldPos;
                lastMouseWorldPos = mouseWorldPos;
                
                // Notify the dragable object that it's being dragged
                selectedDragable.OnStartDrag();
            }
        }
    }

    private void DragObject(Vector2 mouseWorldPos)
    {
        // Update object position
        selectedRb.MovePosition(mouseWorldPos + dragOffset);
        selectedDragable.OnDrag(mouseWorldPos + dragOffset);
        lastMouseWorldPos = mouseWorldPos;
    }

    private void ReleaseObject(Vector2 mouseWorldPos)
    {
        // Calculate the impulse based on direct velocity
        Vector2 throwVelocity = (mouseWorldPos - lastMouseWorldPos) / Time.deltaTime;
        Vector2 impulse = throwVelocity * throwForceMultiplier * throwSpeed;

        // Clamp the impulse magnitude
        float clampedMagnitude = Mathf.Clamp(impulse.magnitude, minThrowForce, maxThrowForce);
        if (impulse != Vector2.zero)
        {
            impulse = impulse.normalized * clampedMagnitude;
        }

        // Apply the clamped impulse force
        selectedRb.AddForce(impulse, ForceMode2D.Impulse);

        // Notify the dragable that dragging has ended
        selectedDragable.OnEndDrag(impulse);

        // Clear references
        selectedDragable = null;
        selectedRb = null;
    }

    /// <summary>
    /// Forcefully release the object if we are in the middle of a drag,
    /// typically called when drag mode is disabled by Tab or timeout.
    /// Pass zero (Vector2.zero) if you want the object to simply drop.
    /// </summary>
    public void ForceReleaseIfDragging()
    {
        if (selectedDragable != null)
        {
            // Optionally compute a velocity or just pass zero.
            // For example, to "drop" it with no throw:
            ReleaseObject(lastMouseWorldPos); 
        }
    }
}
