using UnityEngine;

public class TriggerDebugger2D : MonoBehaviour
{
    // Called once when another object with a 2D collider enters the trigger of this object.
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} - OnTriggerEnter2D with {other.gameObject.name}");
    }

    // Called every frame the other object remains inside the trigger area.
    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} - OnTriggerStay2D with {other.gameObject.name}");
    }

    // Called once when the other object leaves the trigger area.
    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name} - OnTriggerExit2D with {other.gameObject.name}");
    }
}