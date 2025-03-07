using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    public float followSpeed = 50f; // Speed of following the mouse
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Prevents fast movement from clipping
        rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smooth movement
    }

    private void FixedUpdate()
    {
        // Get the mouse position in world coordinates
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Move using physics to prevent clipping
        rb.MovePosition(Vector2.Lerp(rb.position, mousePosition, followSpeed * Time.fixedDeltaTime));

    }
}