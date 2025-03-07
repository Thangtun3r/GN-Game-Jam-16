using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TabSpawner : MonoBehaviour
{
    [Tooltip("Reference to the ObjectPool that supplies spawned objects.")]
    public TabPool pool;

    [Tooltip("Interval (in seconds) between spawns.")]
    public float spawnInterval = 1f;

    [Tooltip("When false, spawning is paused.")]
    public bool spawnable = true;

    // Static UnityEvent that all spawners subscribe to in order to update their spawnable state.
    public static UnityEvent<bool> OnSpawnableChanged = new UnityEvent<bool>();

    private void OnEnable()
    {
        OnSpawnableChanged.AddListener(HandleSpawnableChanged);
        StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        OnSpawnableChanged.RemoveListener(HandleSpawnableChanged);
    }

    private void HandleSpawnableChanged(bool state)
    {
        spawnable = state;
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            if (spawnable && pool != null)
            {
                GameObject obj = pool.GetObjectFromPool();
                if (obj != null)
                {
                    // Set the spawn position (using the spawner's position).
                    obj.transform.position = transform.position;

                    // Optionally reset or initialize the spawned object's state here.
                }
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}