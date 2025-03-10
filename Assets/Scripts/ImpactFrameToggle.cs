using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactFrameToggle : MonoBehaviour
{
    [SerializeField] private GameObject childObject; // Reference to the child object

    private void OnEnable()
    {
        EffectManager.OnToggleObject += ToggleChildObject;
    }

    private void OnDisable()
    {
        EffectManager.OnToggleObject -= ToggleChildObject;
    }

    private void ToggleChildObject(bool isActive)
    {
        if (childObject != null)
        {
            childObject.SetActive(isActive);
        }
    }
}