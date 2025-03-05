using System;
using UnityEngine;

public class CloseAbleAd : MonoBehaviour
{
    private void OnMouseDown()
    {
        // Disable the GameObject when clicked
        gameObject.SetActive(false);
        Debug.Log(gameObject.name + " was closed!");
    }
}