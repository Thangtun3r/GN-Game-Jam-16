using UnityEngine;
using System.Collections;

public class TimeSlowPerk : PerkBase
{
    [Header("Time Slow Settings")]
    [Tooltip("Time scale factor during slow (e.g., 0.1 = 10% speed).")]
    [SerializeField] private float slowFactor = 0.5f;

    [Tooltip("Duration (in real-time seconds) to remain slowed.")]
    [SerializeField] private float slowDuration = 3f;

    // ------------------------------------------------------------------------
    // PICKUP LOGIC
    // ------------------------------------------------------------------------
    // Instead of destroying the perk object immediately, we "fake destroy":
    // - Disable its collider and sprite so the player can’t see or pick it up again.
    // - Keep the script alive so we can still run coroutines or logic later.
    public override void OnPickup()
    {
        base.OnPickup(); 
        Debug.Log($"[{PerkName}] Picked up! Fake-destroying (disabling visuals/collider).");

        // Disable Collider (so it no longer triggers pickup)
        Collider2D col = GetComponent<Collider2D>();
        if (col) col.enabled = false;

        // Disable SpriteRenderer (so it's invisible in the world)
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr) sr.enabled = false;
    }

    // ------------------------------------------------------------------------
    // USAGE LOGIC
    // ------------------------------------------------------------------------
    // Called by PerksManager when the player presses the use button
    protected override void OnUsePerk()
    {
        // Start the time slow coroutine
        Debug.Log($"Using TimeSlowPerk: Slowing time to {slowFactor} for {slowDuration} seconds.");
        StartCoroutine(SlowTimeRoutine());
    }

    // Coroutine that slows time, waits, then restores it
    private IEnumerator SlowTimeRoutine()
    {
        // Set time scale to the slow factor
        Time.timeScale = slowFactor;
        
        // Wait for slowDuration in real-time
        yield return new WaitForSecondsRealtime(slowDuration);

        // Restore normal time
        Time.timeScale = 1f;
        Debug.Log("Time restored to normal.");

        // Now that we're done with the effect, fully destroy this object
        Destroy(gameObject);
    }
}
