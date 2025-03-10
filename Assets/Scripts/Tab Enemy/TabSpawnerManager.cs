using System.Collections;
using UnityEngine;

public class TabSpawnerManager : MonoBehaviour
{
    [Header("Tab Limit")]
    [Tooltip("Maximum allowed number of active objects with tag 'Tab'")]
    public int maxTabs = 10;

    [Header("Spawning Interval")]
    [Tooltip("Time (in seconds) between spawn checks.")]
    public float spawnInterval = 1f;

    [Header("Pool Reference")]
    [Tooltip("Reference to the TabPool that handles weighted prefab selection.")]
    public TabPool tabPool;

    [Header("Spawn Points")]
    [Tooltip("List of possible spawn point Transforms.")]
    public Transform[] spawnPoints;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 1) Count how many active Tabs are in the scene
            GameObject[] tabs = GameObject.FindGameObjectsWithTag("Tab");

            // 2) If we're under the limit, spawn a new one
            if (tabs.Length < maxTabs && tabPool != null && spawnPoints.Length > 0)
            {
                // Pick a random spawn point from the array
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform chosenPoint = spawnPoints[randomIndex];

                // Get an object from the pool (this uses the weighting in TabPool)
                GameObject newTab = tabPool.GetObjectFromPool();
                if (newTab != null)
                {
                    // Position (and optionally rotate) the new Tab
                    newTab.transform.position = chosenPoint.position;
                    // newTab.transform.rotation = chosenPoint.rotation; // if desired
                }
            }

            // Wait for the next spawn interval before looping again
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}