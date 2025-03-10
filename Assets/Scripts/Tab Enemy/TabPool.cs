using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoolEntry
{
    public GameObject prefab;  // Drag & drop your prefab here
    public int poolSize = 10;  // How many instances to pre-instantiate
    public int weight = 1;     // Spawning chance weight (like a gacha system)
}

public class TabPool : MonoBehaviour
{
    public List<PoolEntry> poolEntries;

    // A dictionary to store a queue of inactive objects for each prefab
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    // Mapping from instantiated objects back to their source prefab
    private Dictionary<GameObject, GameObject> instanceToPrefab = new Dictionary<GameObject, GameObject>();

    // List used for weighted selection
    private List<PoolEntry> weightedPoolList = new List<PoolEntry>();

    private void Awake()
    {
        // Pre-instantiate objects for each prefab entry
        foreach (PoolEntry entry in poolEntries)
        {
            Queue<GameObject> objectQueue = new Queue<GameObject>();
            for (int i = 0; i < entry.poolSize; i++)
            {
                GameObject obj = Instantiate(entry.prefab, transform);
                obj.SetActive(false);
                instanceToPrefab[obj] = entry.prefab;
                objectQueue.Enqueue(obj);
            }
            poolDictionary.Add(entry.prefab, objectQueue);
            weightedPoolList.Add(entry);
        }
    }

    /// <summary>
    /// Returns an object from the pool based on weighted chance.
    /// </summary>
    public GameObject GetObjectFromPool()
    {
        // Calculate total weight
        int totalWeight = 0;
        foreach (PoolEntry entry in weightedPoolList)
        {
            totalWeight += entry.weight;
        }

        // Randomly select a weight value
        int randomWeight = Random.Range(1, totalWeight + 1);
        int cumulativeWeight = 0;
        PoolEntry selectedEntry = null;

        foreach (PoolEntry entry in weightedPoolList)
        {
            cumulativeWeight += entry.weight;
            if (randomWeight <= cumulativeWeight)
            {
                selectedEntry = entry;
                break;
            }
        }

        if (selectedEntry != null)
        {
            Queue<GameObject> queue = poolDictionary[selectedEntry.prefab];
            GameObject obj;
            if (queue.Count > 0)
            {
                obj = queue.Dequeue();
                obj.SetActive(true);
            }
            else
            {
                // Pool is empty for this prefab, so instantiate a new one
                obj = Instantiate(selectedEntry.prefab, transform);
                instanceToPrefab[obj] = selectedEntry.prefab;
            }
            return obj;
        }
        return null;
    }

    /// <summary>
    /// Returns an object back to its corresponding pool.
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (instanceToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            if (poolDictionary.ContainsKey(prefab))
            {
                obj.SetActive(false);
                poolDictionary[prefab].Enqueue(obj);
            }
        }
        else
        {
            Debug.LogWarning("Returned object does not belong to any pool.");
            obj.SetActive(false);
        }
    }
}
