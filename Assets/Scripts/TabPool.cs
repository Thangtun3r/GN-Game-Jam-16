using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabPool : MonoBehaviour
{
    [Header("Prefab Settings")]
    public GameObject prefab;         // The prefab to pool
    public int poolSize = 10;         // Number of objects to instantiate in the pool
    public float objectLifetime = 5f; // (Optional) Lifetime before auto return

    private Queue<GameObject> pool;   // Internal queue for pooling

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError("No prefab assigned to TabPool on " + gameObject.name);
            return;
        }

        pool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Retrieves an object from the pool.
    /// </summary>
    public GameObject GetObjectFromPool()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            // Expand pool if needed
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            return newObj;
        }
    }

    /// <summary>
    /// Returns an object to the pool.
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    /// <summary>
    /// Spawns an object from the pool at the given position.
    /// Sets the pool reference on the enemy component if available.
    /// </summary>
    public void Spawn(Vector3 position)
    {
        GameObject obj = GetObjectFromPool();
        obj.transform.position = position;
        obj.SetActive(true);

        // If the spawned object has an Enemy script, set its originating pool reference.
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.originatingPool = this;
            enemy.isPooled = true;
        }

        // Optionally, if you want objects to auto-return after a fixed lifetime, uncomment:
        // StartCoroutine(ReturnAfterDelay(obj));
    }

    // Optional auto-return coroutine (if you want timed returns instead of letting the enemy decide)
    private IEnumerator ReturnAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(objectLifetime);
        ReturnToPool(obj);
    }
}
