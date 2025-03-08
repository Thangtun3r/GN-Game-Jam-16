using UnityEngine;
using System;

public class CursorBeingDamage : MonoBehaviour, ICursorDamageable
{
    // Tag used by damaging objects.
    [SerializeField] private string damageDealerTag = "DamageDealer";

    // Static event that notifies subscribers when damage is taken.
    public static event Action OnDamageTaken;

    // Called when another collider makes contact with this collider.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(damageDealerTag))
        {
            Debug.Log("Damage taken!");
            TakeDamage();
        }
    }
    
    // Called when another trigger collider overlaps with this collider.
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag(damageDealerTag))
        {
            Debug.Log("Trigger damage taken!");
            TakeDamage();
            TakeDamage();
        }
    }

    // Implements the ICursorDamageable interface.
    public void TakeDamage()
    {
        // Fire the event so that subscribers (like HeartManager) can handle the damage.
        OnDamageTaken?.Invoke();
    }
}