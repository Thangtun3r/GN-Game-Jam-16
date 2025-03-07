using UnityEngine;

public class CursorStateSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject colliderCursor;
    [SerializeField] private DragableCursor dragableCursor;
    
    private bool isColliderMode = true;

    private void Start()
    {
        // Set initial state
        SetCursorState(isColliderMode);
    }

    private void Update()
    {
        // Toggle cursor state on Space key press
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleCursorState();
        }
    }

    public void ToggleCursorState()
    {
        isColliderMode = !isColliderMode;
        SetCursorState(isColliderMode);
    }

    private void SetCursorState(bool useCollider)
    {
        if (colliderCursor != null)
        {
            colliderCursor.SetActive(useCollider);
        }
        
        if (dragableCursor != null)
        {
            dragableCursor.enabled = !useCollider;
        }
    }
    
    // Public methods for external state control
    public void EnableColliderMode()
    {
        SetCursorState(true);
        isColliderMode = true;
    }
    
    public void EnableDragMode()
    {
        SetCursorState(false);
        isColliderMode = false;
    }
}