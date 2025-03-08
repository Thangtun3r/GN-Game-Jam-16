using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class CursorStateSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject colliderCursor;
    [SerializeField] private DragableCursor dragableCursor;

    [Header("Cursor Sprites")]
    [SerializeField] private Sprite colliderModeSprite;
    [SerializeField] private Sprite dragModeSprite;

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem colliderModeParticles;
    [SerializeField] private ParticleSystem dragModeParticles;

    [Header("Transition Settings")]
    [SerializeField] private float fadeTransitionDuration = 0.3f;

    [Header("Cooldown Settings")]
    [SerializeField] private float dragModeDuration = 5f; // How long dragable mode lasts
    [SerializeField] private float dragModeCooldown = 10f; // Cooldown after using dragable mode
    [SerializeField] private TextMeshProUGUI cooldownText; // Text display
    [SerializeField] private Image cooldownRadial;         // Circular fill image
 
    // References to specific components
    private SpriteRenderer colliderCursorSprite;
    private Collider2D colliderCursorCollider;

    private bool isColliderMode = true;
    private Coroutine fadeCoroutine;

    // Cooldown tracking
    private float remainingDragDuration;
    private float remainingCooldown;
    private bool isInCooldown = false;

    private void Start()
    {
        // Get component references
        if (colliderCursor != null)
        {
            colliderCursorSprite = colliderCursor.GetComponent<SpriteRenderer>();
            colliderCursorCollider = colliderCursor.GetComponent<Collider2D>();
        }

        // Set initial state
        SetCursorState(isColliderMode);

        // Initialize UI elements
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
        if (cooldownRadial != null)
        {
            cooldownRadial.fillAmount = 0;
            cooldownRadial.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        Cursor.visible = false;

        // Toggle cursor state on Tab key press if not in cooldown
        if (Input.GetKeyDown(KeyCode.Tab) && !isInCooldown)
        {
            ToggleCursorState();
        }

        // If in dragable mode, count down
        if (!isColliderMode)
        {
            remainingDragDuration -= Time.deltaTime;

            // Update UI - fill from 0 to 1 as time passes in drag mode
            if (cooldownRadial != null)
            {
                cooldownRadial.gameObject.SetActive(true);
                cooldownRadial.fillAmount = 1 - (remainingDragDuration / dragModeDuration);
            }

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(remainingDragDuration).ToString();
            }

            // Switch back when duration ends
            if (remainingDragDuration <= 0)
            {
                // Force release any dragged object before leaving drag mode
                if (dragableCursor != null)
                {
                    dragableCursor.ForceReleaseIfDragging();
                }

                // Switch to collider mode
                isColliderMode = true;
                SetCursorState(true);

                // Start cooldown
                isInCooldown = true;
                remainingCooldown = dragModeCooldown;
            }
        }
        // Handle cooldown after dragable mode
        else if (isInCooldown)
        {
            remainingCooldown -= Time.deltaTime;

            // Update UI - depleting from 1 to 0 during cooldown
            if (cooldownRadial != null)
            {
                cooldownRadial.gameObject.SetActive(true);
                cooldownRadial.fillAmount = remainingCooldown / dragModeCooldown;
            }

            if (cooldownText != null)
            {
                cooldownText.gameObject.SetActive(true);
                cooldownText.text = Mathf.CeilToInt(remainingCooldown).ToString();
            }

            // End cooldown when time is up
            if (remainingCooldown <= 0)
            {
                isInCooldown = false;

                // Hide UI elements
                if (cooldownText != null)
                    cooldownText.gameObject.SetActive(false);
                if (cooldownRadial != null)
                {
                    cooldownRadial.gameObject.SetActive(false);
                    cooldownRadial.fillAmount = 0f; // Reset to empty
                }
            }
        }
    }

    public void ToggleCursorState()
    {
        // Only allow switching to dragable mode if not in cooldown
        if (isColliderMode && !isInCooldown)
        {
            isColliderMode = false;
            remainingDragDuration = dragModeDuration;

            // Initialize the radial to empty (0%) when entering dragable mode
            if (cooldownRadial != null)
            {
                cooldownRadial.fillAmount = 0f;
            }

            SetCursorState(false);
        }
        // Switching back to collider mode
        else if (!isColliderMode)
        {
            // Force release any dragged object before leaving drag mode
            if (dragableCursor != null)
            {
                dragableCursor.ForceReleaseIfDragging();
            }

            isColliderMode = true;
            SetCursorState(true);

            // Start cooldown
            isInCooldown = true;
            remainingCooldown = dragModeCooldown;

            // Set the initial cooldown fill amount to full
            if (cooldownRadial != null)
            {
                cooldownRadial.fillAmount = 1f;
            }
        }
    }

    private void SetCursorState(bool useCollider)
    {
        if (colliderCursor != null)
        {
            // Start fade transition between sprites
            if (colliderCursorSprite != null)
            {
                // Stop any ongoing fade
                if (fadeCoroutine != null)
                    StopCoroutine(fadeCoroutine);

                // Start new fade transition
                fadeCoroutine = StartCoroutine(FadeBetweenSprites(
                    useCollider ? colliderModeSprite : dragModeSprite));
            }

            // Enable/disable the collider based on mode
            if (colliderCursorCollider != null)
                colliderCursorCollider.enabled = useCollider;
        }

        if (dragableCursor != null)
        {
            dragableCursor.enabled = !useCollider;
        }

        // Play appropriate particle effect
        if (useCollider)
        {
            if (colliderModeParticles != null)
                colliderModeParticles.Play();
        }
        else
        {
            if (dragModeParticles != null)
                dragModeParticles.Play();
        }
    }

    private IEnumerator FadeBetweenSprites(Sprite targetSprite)
    {
        // First fade out the current sprite
        Color startColor = colliderCursorSprite.color;
        float elapsedTime = 0f;

        // Fade out
        while (elapsedTime < fadeTransitionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / (fadeTransitionDuration / 2);
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, normalizedTime);
            colliderCursorSprite.color = newColor;
            yield return null;
        }

        // Change sprite when fully transparent
        colliderCursorSprite.sprite = targetSprite;

        // Fade in the new sprite
        elapsedTime = 0f;
        while (elapsedTime < fadeTransitionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / (fadeTransitionDuration / 2);
            Color newColor = startColor;
            newColor.a = Mathf.Lerp(0f, 1f, normalizedTime);
            colliderCursorSprite.color = newColor;
            yield return null;
        }

        // Ensure we end with full opacity
        Color finalColor = startColor;
        finalColor.a = 1f;
        colliderCursorSprite.color = finalColor;

        fadeCoroutine = null;
    }

    // Optional external methods to enable specific mode from other scripts:
    public void EnableColliderMode()
    {
        if (!isInCooldown || !isColliderMode)
        {
            // Force release
            if (dragableCursor != null)
            {
                dragableCursor.ForceReleaseIfDragging();
            }

            isColliderMode = true;
            SetCursorState(true);

            if (!isColliderMode)
            {
                isInCooldown = true;
                remainingCooldown = dragModeCooldown;
                if (cooldownRadial != null)
                    cooldownRadial.fillAmount = 1f;
            }
        }
    }

    public void EnableDragMode()
    {
        if (!isInCooldown)
        {
            isColliderMode = false;
            remainingDragDuration = dragModeDuration;

            if (cooldownRadial != null)
            {
                cooldownRadial.fillAmount = 0f;
            }

            SetCursorState(false);
        }
    }
}
