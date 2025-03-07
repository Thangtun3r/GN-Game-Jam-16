using System.Collections;
using UnityEngine;

public class TabSpawnerManager : MonoBehaviour
{
    [Tooltip("Maximum allowed number of active objects with tag 'Tab'")]
    public int maxTabs = 10;
    
    [Tooltip("Interval (in seconds) to check the active count")]
    public float checkInterval = 1f;
    
    private void Start()
    {
        StartCoroutine(CheckTabsRoutine());
    }

    private IEnumerator CheckTabsRoutine()
    {
        while (true)
        {
            // Get all active objects in the scene with tag "Tab"
            GameObject[] tabs = GameObject.FindGameObjectsWithTag("Tab");

            // If count exceeds limit, stop spawning, otherwise allow it.
            if (tabs.Length > maxTabs)
            {
                TabSpawner.OnSpawnableChanged.Invoke(false);
            }
            else
            {
                TabSpawner.OnSpawnableChanged.Invoke(true);
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }
}