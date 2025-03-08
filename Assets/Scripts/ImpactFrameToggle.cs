using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFrameToggle : MonoBehaviour
{
    [SerializeField] private GameObject childObject; // Reference to the child object

    private void OnEnable()
    {
        ImpactDebugger.OnToggleObject += ToggleChildObject;
    }

    private void OnDisable()
    {
        ImpactDebugger.OnToggleObject -= ToggleChildObject;
    }

    private void ToggleChildObject(bool isActive)
    {
        if (childObject != null)
        {
            childObject.SetActive(isActive);
        }
    }
}