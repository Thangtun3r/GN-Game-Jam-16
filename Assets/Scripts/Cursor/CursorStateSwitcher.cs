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
    [SerializeField] private float inputBufferDelay = 0.5f; // Buffer delay to prevent accidental toggle

    [Header("Cooldown Settings")]
    [SerializeField] private float dragModeDuration = 5f; // How long dragable mode lasts
    [SerializeField] private float dragModeCooldown = 10f; // Cooldown after using dragable mode
    [SerializeField] private TextMeshProUGUI cooldownText; // Text display
    [SerializeField] private Image cooldownRadial;         // Circular fill image

    [Header("Cursor Mode Indicator")]
    [SerializeField] private SpriteRenderer indicatorSpriteRenderer; // <--- Extra sprite for showing mode

    // References to specific components
    private SpriteRenderer colliderCursorSprite;
    private Collider2D colliderCursorCollider;

    private bool isColliderMode = true;
    private Coroutine fadeCoroutine;

    // Cooldown tracking
    private float remainingDragDuration;
    private float remainingCooldown;
    private bool isInCooldown = false;
    private bool isInInputBuffer = false; // New flag for input buffer
    private float remainingInputBuffer = 0f; // Timer for input buffer

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
        // Hide default system cursor
        Cursor.visible = false;

        // Update input buffer timer
        if (isInInputBuffer)
        {
            remainingInputBuffer -= Time.deltaTime;
            if (remainingInputBuffer <= 0f)
            {
                isInInputBuffer = false;
            }
        }

        // Toggle cursor state on Tab key press if not in cooldown and not in input buffer
        if (Input.GetKeyDown(KeyCode.Tab) && !isInCooldown && !isInInputBuffer)
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
                    dragableCursor.ForceReleaseIfDragging();

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
                cooldownRadial.fillAmount = 0f;

            SetCursorState(false);
            
            // Add input buffer when switching to dragable mode
            isInInputBuffer = true;
            remainingInputBuffer = inputBufferDelay;
        }
        // Switching back to collider mode
        else if (!isColliderMode)
        {
            // Force release any dragged object before leaving drag mode
            if (dragableCursor != null)
                dragableCursor.ForceReleaseIfDragging();

            isColliderMode = true;
            SetCursorState(true);

            // Start cooldown
            isInCooldown = true;
            remainingCooldown = dragModeCooldown;

            // Set the initial cooldown fill amount to full
            if (cooldownRadial != null)
                cooldownRadial.fillAmount = 1f;
        }
    }

    private void SetCursorState(bool useCollider)
    {
        // Fade the in-world cursor sprite
        if (colliderCursor != null && colliderCursorSprite != null)
        {
            // Stop any ongoing fade
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            // Start new fade transition
            fadeCoroutine = StartCoroutine(
                FadeBetweenSprites(useCollider ? colliderModeSprite : dragModeSprite)
            );

            // Enable/disable the collider
            if (colliderCursorCollider != null)
                colliderCursorCollider.enabled = useCollider;
        }

        // Enable/disable the dragable cursor component
        if (dragableCursor != null)
            dragableCursor.enabled = !useCollider;

        // Play appropriate particle effect
        if (useCollider)
        {
            if (colliderModeParticles != null) colliderModeParticles.Play();
        }
        else
        {
            if (dragModeParticles != null) dragModeParticles.Play();
        }

        // Update the indicator's sprite
        if (indicatorSpriteRenderer != null)
        {
            indicatorSpriteRenderer.sprite = useCollider
                ? colliderModeSprite
                : dragModeSprite;
        }
    }

    private IEnumerator FadeBetweenSprites(Sprite targetSprite)
    {
        // Fade out current sprite
        Color startColor = colliderCursorSprite.color;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTransitionDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / (fadeTransitionDuration / 2);

            Color newColor = startColor;
            newColor.a = Mathf.Lerp(1f, 0f, normalizedTime);
            colliderCursorSprite.color = newColor;
            yield return null;
        }

        // Switch sprite after fully transparent
        colliderCursorSprite.sprite = targetSprite;

        // Fade in new sprite
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

        // Ensure final opacity is 1
        Color finalColor = startColor;
        finalColor.a = 1f;
        colliderCursorSprite.color = finalColor;

        fadeCoroutine = null;
    }

    // Optionally, call these from other scripts to force a particular mode:
    public void EnableColliderMode()
    {
        if ((!isInCooldown || !isColliderMode) && !isInInputBuffer)
        {
            if (dragableCursor != null)
                dragableCursor.ForceReleaseIfDragging();

            isColliderMode = true;
            SetCursorState(true);

            // If switching away from drag mode, trigger cooldown
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
        if (!isInCooldown && !isInInputBuffer)
        {
            isColliderMode = false;
            remainingDragDuration = dragModeDuration;

            if (cooldownRadial != null)
                cooldownRadial.fillAmount = 0f;

            SetCursorState(false);
            
            // Add input buffer when switching to dragable mode
            isInInputBuffer = true;
            remainingInputBuffer = inputBufferDelay;
        }
    }
}