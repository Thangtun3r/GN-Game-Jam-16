using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    public TabPool[] tabPools;          // Array of TabPool components (one per prefab type)
    public Vector2 spawnArea = new Vector2(10f, 5f); // Spawn area dimensions

    [Header("Spawn Timing")]
    public float minSpawnTime = 1f;     // Minimum delay before spawning
    public float maxSpawnTime = 3f;     // Maximum delay before spawning

    private void Start()
    {
        // Start the spawning loop
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);
            SpawnRandomObject();
        }
    }

    private void SpawnRandomObject()
    {
        if (tabPools == null || tabPools.Length == 0)
        {
            Debug.LogWarning("No TabPools assigned to the spawner.");
            return;
        }

        // Pick a random TabPool from the array
        TabPool selectedPool = tabPools[Random.Range(0, tabPools.Length)];

        // Determine a random spawn position within the spawn area
        Vector2 spawnPosition = new Vector2(
            transform.position.x + Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            transform.position.y + Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
        );

        // Spawn an object from the selected TabPool
        selectedPool.Spawn(spawnPosition);
    }

    // Draw the spawn area in the Scene view for easier debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}