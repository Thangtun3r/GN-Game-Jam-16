using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class BaseTab : MonoBehaviour, IDragable
{
    [Header("Targeting")]
    public string targetTag = "Target"; // Set this in the inspector or override in derived classes
    public float stoppingDistance = 0.5f; // Distance at which the object stops moving when near target

    [Header("Movement")]
    public float speed = 5f; // Movement speed

    [Header("Drag Visuals")]
    [SerializeField] private GameObject visualContainer; // Assign this in the Inspector
    [SerializeField] private float wiggleAmount = 5f; // Degrees of rotation for wiggle
    [SerializeField] private float wiggleSpeed = 10f; // Speed of the wiggle effect
    [SerializeField] private float scaleMultiplier = 1.1f; // Scale increase when dragging
    [SerializeField] private float scaleSpeed = 0.2f; // Speed of scaling effect

    public bool isThrowing = false;
    private bool isKnockedBack = false;
    private bool isPausedByCollision = false;
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

        attachedObjectCollider = GetComponentInParent<Collider2D>();
        if (attachedObjectCollider == null)
        {
            attachedObjectCollider = GetComponentInChildren<Collider2D>();
        }

        if (visualContainer != null)
        {
            visualSpriteRenderer = visualContainer.GetComponent<SpriteRenderer>();
            originalScale = visualContainer.transform.localScale;
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
                isThrowing = false;
                rb.velocity = Vector2.zero;
            }
            return;
        }

        if (target != null && !isPausedByCollision)
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

        // Stop moving when close to the target
        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        if (distanceToTarget <= stoppingDistance)
        {
            rb.velocity = Vector2.zero; // Stop moving when close to the target
            return;
        }

        // Otherwise, continue moving
        Vector2 direction = ((Vector2)target.position - rb.position).normalized;
        rb.velocity = direction * speed;
    }

    public void StartKnockback(float duration)
    {
        isKnockedBack = true;
        isThrowing = true;
        knockbackTimer = duration;
    }

    // IDragable Implementation
    public void OnStartDrag()
    {
        originalGravityScale = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        this.enabled = false;

        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = false;
        }

        if (visualSpriteRenderer != null)
        {
            Color newColor = visualSpriteRenderer.color;
            newColor.a = 0.7f;
            visualSpriteRenderer.color = newColor;
        }

        if (visualContainer != null)
        {
            wiggleCoroutine = StartCoroutine(WiggleEffect());
            StartCoroutine(ScaleEffect(visualContainer.transform, originalScale * scaleMultiplier));
        }
    }

    public void OnDrag(Vector2 position) { }

    public void OnEndDrag(Vector2 velocity)
    {
        rb.gravityScale = originalGravityScale;
        this.enabled = true;
        StartKnockback(0.5f);

        if (attachedObjectCollider != null)
        {
            attachedObjectCollider.enabled = true;
        }

        if (visualSpriteRenderer != null)
        {
            Color newColor = visualSpriteRenderer.color;
            newColor.a = 1.0f;
            visualSpriteRenderer.color = newColor;
        }

        if (wiggleCoroutine != null)
        {
            StopCoroutine(wiggleCoroutine);
            visualContainer.transform.rotation = Quaternion.identity;
        }

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

    // Detects collision and pauses movement if the object is hitting the **facing direction**
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (target == null) return;

        // Don't pause if already close to the target
        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        if (distanceToTarget <= stoppingDistance) return;

        Vector2 moveDirection = ((Vector2)target.position - rb.position).normalized;
        foreach (ContactPoint2D contact in collision.contacts)
        {
            Vector2 normal = contact.normal; // Collision normal (the side that was hit)
            float dotProduct = Vector2.Dot(normal, moveDirection);

            if (dotProduct < -0.5f) // Only pause if hitting the side **facing** movement direction
            {
                isPausedByCollision = true;
                rb.velocity = Vector2.zero;
                return;
            }
        }
    }

    // Resumes movement when no longer colliding
    private void OnCollisionExit2D(Collision2D collision)
    {
        isPausedByCollision = false;
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
