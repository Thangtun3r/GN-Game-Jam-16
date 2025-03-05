using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] spawnPrefabs; // Prefabs to spawn
    public Vector2 spawnArea = new Vector2(10f, 5f); // Spawn area width and height

    [Header("Spawn Timing")]
    public float minSpawnTime = 1f; // Minimum spawn delay
    public float maxSpawnTime = 3f; // Maximum spawn delay

    [Header("Object Pool Settings")]
    public int poolSizePerPrefab = 10; // Number of objects per prefab type
    public float objectLifetime = 5f; // Time before objects are returned to the pool

    private Dictionary<GameObject, Queue<GameObject>> objectPools; // Dictionary for pooling

    private void Start()
    {
        // Initialize Object Pools
        objectPools = new Dictionary<GameObject, Queue<GameObject>>();
        foreach (GameObject prefab in spawnPrefabs)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < poolSizePerPrefab; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            objectPools[prefab] = pool;
        }

        // Start spawning objects
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
        if (spawnPrefabs.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to spawn.");
            return;
        }

        // Choose a random prefab
        GameObject selectedPrefab = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

        // Get an object from the pool
        GameObject obj = GetObjectFromPool(selectedPrefab);

        // Set a random spawn position
        Vector2 spawnPosition = new Vector2(
            transform.position.x + Random.Range(-spawnArea.x / 2, spawnArea.x / 2),
            transform.position.y + Random.Range(-spawnArea.y / 2, spawnArea.y / 2)
        );

        // Activate and place the object
        obj.transform.position = spawnPosition;
        obj.SetActive(true);

        // Return object to pool after a set time
        StartCoroutine(ReturnToPool(obj, selectedPrefab));
    }

    private GameObject GetObjectFromPool(GameObject prefab)
    {
        if (objectPools[prefab].Count > 0)
        {
            return objectPools[prefab].Dequeue();
        }
        else
        {
            // Expand pool if needed
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            return newObj;
        }
    }

    private IEnumerator ReturnToPool(GameObject obj, GameObject prefab)
    {
        yield return new WaitForSeconds(objectLifetime);
        obj.SetActive(false);
        objectPools[prefab].Enqueue(obj);
    }

    // Debugging: Draw the spawn area in Scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnArea);
    }
}
