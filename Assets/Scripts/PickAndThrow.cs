using UnityEngine;

public class PickAndThrow : MonoBehaviour
{
    public float pickUpRange = 1.5f; // Max distance to pick up objects
    public float throwForce = 10f; // Strength of the throw
    public LayerMask pickableLayer; // Layer for pickable objects

    private Rigidbody2D heldObject;
    private Vector2 lastMousePosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left-click to pick up
        {
            TryPickUpObject();
        }
        else if (Input.GetMouseButtonUp(0) && heldObject) // Release to throw
        {
            ThrowObject();
        }
    }

    private void FixedUpdate()
    {
        if (heldObject)
        {
            // Move object towards mouse smoothly
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            heldObject.linearVelocity = (mousePosition - (Vector2)heldObject.position) * 10f;
            lastMousePosition = mousePosition;
        }
    }

    private void TryPickUpObject()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos, pickableLayer);

        if (hit != null && hit.CompareTag("Pickable")) // Ensure it's pickable
        {
            heldObject = hit.GetComponent<Rigidbody2D>();
            heldObject.gravityScale = 0; // Disable gravity while holding
            heldObject.linearVelocity = Vector2.zero; // Stop movement
        }
    }

    private void ThrowObject()
    {
        if (heldObject)
        {
            Vector2 throwDirection = (lastMousePosition - (Vector2)heldObject.position).normalized;
            heldObject.gravityScale = 1; // Restore gravity
            heldObject.linearVelocity = throwDirection * throwForce; // Apply throw force
            heldObject = null;
        }
    }
}