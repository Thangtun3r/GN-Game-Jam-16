using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        using UnityEngine.Events;
        
        public class ExplosiveTabPool : MonoBehaviour
        {
            public static ExplosiveTabPool Instance { get; private set; }
        
            [Header("Pool Settings")]
            [SerializeField] private GameObject explosiveTabPrefab;
            [SerializeField] private int initialPoolSize = 10;
            [SerializeField] private bool expandPoolIfNeeded = true;
        
            [Header("Spawn Settings")]
            [SerializeField] private BoxCollider2D spawnArea;
        
            [Header("Percentage-Based Spawning")]
            [Range(0, 100)]
            [SerializeField] private float spawnChancePercentage = 25f; // Chance to spawn per check
            [SerializeField] private float minCheckInterval = 1f; // Minimum time between spawn checks
            [SerializeField] private float maxCheckInterval = 3f; // Maximum time between spawn checks
            [SerializeField] private bool autoSpawn = false;
            [SerializeField] private bool respectTabSpawnerManager = true;
        
            private Queue<GameObject> tabPool;
            private Coroutine autoSpawnRoutine;
            private bool canSpawn = true;
        
            private void Awake()
            {
                // Singleton pattern
                if (Instance == null)
                {
                    Instance = this;
                }
                else
                {
                    Destroy(gameObject);
                    return;
                }
        
                // Initialize pool
                tabPool = new Queue<GameObject>();
                InitializePool();
            }
        
            private void Start()
            {
                if (autoSpawn)
                {
                    StartAutoSpawn();
                }
        
                // Subscribe to tab spawner manager if needed
                if (respectTabSpawnerManager)
                {
                    TabSpawner.OnSpawnableChanged.AddListener(OnSpawnableChanged);
                }
            }
        
            private void OnDestroy()
            {
                if (respectTabSpawnerManager)
                {
                    TabSpawner.OnSpawnableChanged.RemoveListener(OnSpawnableChanged);
                }
            }
        
            private void OnSpawnableChanged(bool isSpawnable)
            {
                canSpawn = isSpawnable;
            }
        
            private void InitializePool()
            {
                for (int i = 0; i < initialPoolSize; i++)
                {
                    CreateNewPoolItem();
                }
            }
        
            private GameObject CreateNewPoolItem()
            {
                GameObject newTab = Instantiate(explosiveTabPrefab, transform);
                ExplosiveTab tabComponent = newTab.GetComponent<ExplosiveTab>();
        
                if (tabComponent == null)
                {
                    Debug.LogError("ExplosiveTab component missing from prefab!");
                }
        
                newTab.SetActive(false);
                tabPool.Enqueue(newTab);
                return newTab;
            }
        
            public void StartAutoSpawn()
            {
                if (autoSpawnRoutine == null)
                {
                    autoSpawnRoutine = StartCoroutine(PercentageBasedSpawnRoutine());
                }
            }
        
            public void StopAutoSpawn()
            {
                if (autoSpawnRoutine != null)
                {
                    StopCoroutine(autoSpawnRoutine);
                    autoSpawnRoutine = null;
                }
            }
        
            private IEnumerator PercentageBasedSpawnRoutine()
            {
                while (true)
                {
                    // Calculate random interval for unpredictable spawning
                    float currentInterval = Random.Range(minCheckInterval, maxCheckInterval);
                    
                    // Wait before attempting to spawn
                    yield return new WaitForSeconds(currentInterval);
                    
                    // Only spawn if allowed by TabSpawnerManager (if respecting it)
                    // AND if the random chance check passes
                    if ((!respectTabSpawnerManager || canSpawn) && 
                        Random.Range(0f, 100f) < spawnChancePercentage)
                    {
                        SpawnTab();
                    }
                }
            }
        
            public GameObject SpawnTab()
            {
                if (respectTabSpawnerManager && !canSpawn)
                {
                    return null; // Don't spawn if the TabSpawnerManager disallows it
                }
                
                Vector2 spawnPosition = GetRandomSpawnPosition();
        
                if (spawnPosition == Vector2.zero)
                {
                    Debug.LogWarning("Could not determine spawn position. Is the spawn area assigned?");
                    return null;
                }
        
                GameObject tab = GetTabFromPool();
                if (tab != null)
                {
                    tab.transform.position = spawnPosition;
                    tab.SetActive(true);
                    return tab;
                }
        
                return null;
            }
        
            public GameObject GetTabFromPool()
            {
                if (tabPool.Count == 0)
                {
                    if (expandPoolIfNeeded)
                    {
                        return CreateNewPoolItem();
                    }
                    else
                    {
                        Debug.LogWarning("Tab pool is empty and not set to expand!");
                        return null;
                    }
                }
        
                return tabPool.Dequeue();
            }
        
            public void ReturnToPool(GameObject tab)
            {
                tab.SetActive(false);
                tab.transform.SetParent(transform);
                tabPool.Enqueue(tab);
            }
        
            private Vector2 GetRandomSpawnPosition()
            {
                if (spawnArea == null)
                {
                    Debug.LogError("Spawn area not assigned!");
                    return Vector2.zero;
                }
        
                // Get the bounds of the box collider in world space
                Bounds bounds = spawnArea.bounds;
        
                // Calculate a random position within the bounds
                float randomX = Random.Range(bounds.min.x, bounds.max.x);
                float randomY = Random.Range(bounds.min.y, bounds.max.y);
        
                // Return the random position
                return new Vector2(randomX, randomY);
            }
        
            // Optional: Visualize the spawn area in the editor
            private void OnDrawGizmos()
            {
                if (spawnArea != null)
                {
                    Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.3f);
                    Bounds bounds = spawnArea.bounds;
                    Gizmos.DrawCube(bounds.center, bounds.size);
        
                    Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.7f);
                    Gizmos.DrawWireCube(bounds.center, bounds.size);
                }
            }
        }