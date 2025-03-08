using UnityEngine;

public class CursorDamageDealer : MonoBehaviour
{
    public int damageAmount = 10; // Set this in the Inspector or dynamically in code

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has a component that implements IDamageable
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        // Check if the triggered object has a component that implements IDamageable
        IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();

        if (damageable != null)
        {
            damageable.TakeDamage(damageAmount);
        }
    }
}