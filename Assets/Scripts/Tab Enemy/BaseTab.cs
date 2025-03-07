using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseTab : MonoBehaviour
{
    [Header("Targeting")]
    public string targetTag = "Target"; // Set this in the inspector or override in derived classes

    [Header("Movement")]
    public float speed = 5f; // Movement speed

    // Knockback control
    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Transform target;
    private Rigidbody2D rb;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; // Ensure no unwanted gravity effects
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // Prevent unintended rotation

        FindTarget();
    }

    protected virtual void FixedUpdate()
    {
        // If we're in a knockback phase, count down and skip normal movement.
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                // Optionally reset velocity to avoid drifting
                rb.velocity = Vector2.zero;
            }
            return;
        }

        // Normal movement toward the target if not knocked back
        if (target != null)
        {
            MoveTowardsTarget();
        }
    }

    private void FindTarget()
    {
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogWarning($"No GameObject found with tag '{targetTag}'.");
        }
    }

    private void MoveTowardsTarget()
    {
        if (target == null) return;

        // Use velocity instead of MovePosition
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = direction * speed;
    }

    /// <summary>
    /// Call this method from your knockback script to disable normal movement
    /// for a short duration while the knockback force takes effect.
    /// </summary>
    /// <param name="duration">How long to skip normal movement.</param>
    
    public void StartKnockback(float duration)
    {
        isKnockedBack = true;
        knockbackTimer = duration;
    }
}
