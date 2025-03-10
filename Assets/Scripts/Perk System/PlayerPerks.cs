using UnityEngine;

public class PlayerPerks : MonoBehaviour
{
    [SerializeField] private PerksManager perksManager;
    
    [Header("Collision Detection")]
    [SerializeField] private Collider2D triggerCollider; // Assign specific trigger collider
    [SerializeField] private Collider2D collisionCollider; // Assign specific collision collider
    [SerializeField] private bool useSpecificColliders = false; // Toggle to use specific colliders

    private void Update()
    {
        // If right mouse button is pressed, use the current perk
        if (Input.GetMouseButtonDown(1))
        {
            perksManager.UseCurrentPerk();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Skip if we're using specific colliders and this isn't the right one
        if (useSpecificColliders && triggerCollider != null && other.GetComponentInParent<Collider2D>() != triggerCollider)
            return;

        // Check if this object is a perk
        IPickable perk = other.GetComponent<IPickable>();
        if (perk != null)
        {
            // Attempt to pick it up
            bool success = perksManager.TryPickupPerk(perk);
            // If successful, perksManager calls OnPickup() which destroys the perk GameObject
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Skip if we're using specific colliders and this isn't the right one
        if (useSpecificColliders && collisionCollider != null && 
            collision.collider.GetComponentInParent<Collider2D>() != collisionCollider)
            return;
            
        // Check if this object is a perk
        IPickable perk = collision.gameObject.GetComponent<IPickable>();
        if (perk != null)
        {
            // Attempt to pick it up
            bool success = perksManager.TryPickupPerk(perk);
            // If successful, perksManager calls OnPickup() which destroys the perk GameObject
        }
    }
}