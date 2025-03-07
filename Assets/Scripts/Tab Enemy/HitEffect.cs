using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HitEffect : MonoBehaviour
{
    private float knockbackSpeed = 10f;      // The fixed speed of the knockback
    private float knockbackDuration = 0.1f;  // How long the knockback effect lasts

    private Rigidbody2D rb;
    private BaseTab baseTab;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        baseTab = GetComponent<BaseTab>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if collided with damage dealer
        if (collision.gameObject.GetComponent<CursorDamageDealer>() != null)
        {
            Debug.Log("Knockback from cursor");
            if (collision.contacts.Length > 0)
            {
                // Get direction away from the collision point
                Vector2 contactPoint = collision.contacts[0].point;
                Vector2 direction = ((Vector2)transform.position - contactPoint).normalized;

                // Cancel any existing velocity first
                rb.velocity = Vector2.zero;
                
                // Apply a fixed velocity regardless of collision force
                rb.velocity = direction * knockbackSpeed;
                
                // Ensure no other forces are affecting the object during knockback
                rb.angularVelocity = 0f;

                // Tell BaseTab to pause normal movement for the duration
                if (baseTab != null)
                {
                    baseTab.StartKnockback(knockbackDuration);
                }
            }
        }
    }
}