using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseTab : MonoBehaviour, IDragable
{
    [Header("Targeting")]
    public string targetTag = "Target"; // Set this in the inspector or override in derived classes

    [Header("Movement")]
    public float speed = 5f; // Movement speed

    [Header("Drag Visuals")]
    [SerializeField] private GameObject visualContainer; // Assign this in the Inspector
    [SerializeField] private float wiggleAmount = 5f; // Degrees of rotation for wiggle
    [SerializeField] private float wiggleSpeed = 10f; // Speed of the wiggle effect
    [SerializeField] private float scaleMultiplier = 1.1f; // Scale increase when dragging
    [SerializeField] private float scaleSpeed = 0.2f; // Speed of scaling effect

    // New public bool to indicate if the tab is "thrown"
    public bool isThrowing = false;

    private bool isKnockedBack = false;
    private float knockbackTimer = 0f;

    private Transform target;
    private Rigidbody2D rb;
    private float originalGravityScale;
    private Collider2D attachedObjectCollider;
    private SpriteRenderer visualSpriteRenderer;
    private Coroutine wiggleCoroutine;
    private Vector3 originalScale;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0; 
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; 

        FindTarget();

        // Find the attached object's collider (parent or child)
        attachedObjectCollider = GetComponentInParent<Collider2D>();
        if (attachedObjectCollider == null)
        {
            attachedObjectCollider = GetComponentInChildren<Collider2D>();
        }

        // Get sprite renderer from the assigned visual container
        if (visualContainer != null)
        {
            visualSpriteRenderer = visualContainer.GetComponent<SpriteRenderer>();
            originalScale = visualContainer.transform.localScale; // Store the original scale
        }
        else
        {
            Debug.LogWarning("Visual Container is not assigned in BaseTab.");
        }
    }

    protected virtual void FixedUpdate()
    {
        if (isKnockedBack)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
            {
                isKnockedBack = false;
                isThrowing = false; // Knockback is over, so no longer throwing.
                rb.velocity = Vector2.zero;
            }
            return;
        }

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

        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = direction * speed;
    }

    public void StartKnockback(float duration)
    {
        isKnockedBack = true;
        isThrowing = true; // Now the object is considered "thrown"
        knockbackTimer = duration;
    }

    // IDragable Implementation
    public void OnStartDrag()
    {
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        this.enabled = false; // Disable movement script

        // Disable attached object's collider if it exists
        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = false;
        }

        // Reduce opacity to 70% in the assigned visual container
        if (visualSpriteRenderer != null)
        {
            Color newColor = visualSpriteRenderer.color;
            newColor.a = 0.7f;
            visualSpriteRenderer.color = newColor;
        }

        // Start Wiggle Effect
        if (visualContainer != null)
        {
            wiggleCoroutine = StartCoroutine(WiggleEffect());
            StartCoroutine(ScaleEffect(visualContainer.transform, originalScale * scaleMultiplier));
        }
    }

    public void OnDrag(Vector2 position)
    {
        // Position is handled by the cursor script
    }

    public void OnEndDrag(Vector2 velocity)
    {
        rb.gravityScale = originalGravityScale;
        this.enabled = true;
        StartKnockback(0.5f);

        // Re-enable attached object's collider
        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = true;
        }

        // Restore opacity to 100% in the assigned visual container
        if (visualSpriteRenderer != null)
        {
            Color newColor = visualSpriteRenderer.color;
            newColor.a = 1.0f;
            visualSpriteRenderer.color = newColor;
        }

        // Stop Wiggle Effect and Reset Scale
        if (wiggleCoroutine != null)
        {
            StopCoroutine(wiggleCoroutine);
            visualContainer.transform.rotation = Quaternion.identity;
        }

        // Reset scale back to normal
        StartCoroutine(ScaleEffect(visualContainer.transform, originalScale));
    }

    public Rigidbody2D GetRigidbody()
    {
        return rb;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    // Coroutine for Wiggle Animation
    private IEnumerator WiggleEffect()
    {
        float timeElapsed = 0f;
        while (true)
        {
            float angle = Mathf.Sin(timeElapsed * wiggleSpeed) * wiggleAmount;
            visualContainer.transform.rotation = Quaternion.Euler(0, 0, angle);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Coroutine for Smooth Scaling
    private IEnumerator ScaleEffect(Transform target, Vector3 targetScale)
    {
        float elapsedTime = 0f;
        Vector3 startScale = target.localScale;

        while (elapsedTime < scaleSpeed)
        {
            target.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / scaleSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.localScale = targetScale;
    }
}
