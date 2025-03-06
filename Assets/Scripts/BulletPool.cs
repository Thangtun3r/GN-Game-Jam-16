using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance;

    [Header("Bullet Pool Settings")]
    [Tooltip("The bullet prefab that should have the Bullet script attached.")]
    public GameObject bulletPrefab;
    [Tooltip("Initial number of bullets to instantiate.")]
    public int poolSize = 20;

    private List<GameObject> pooledBullets;

    void Awake()
    {
        // Setup the singleton instance.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Initialize the pool list and instantiate the initial bullets.
        pooledBullets = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pooledBullets.Add(bullet);
        }
    }

    // This function returns an inactive bullet from the pool.
    // If all bullets are active, a new bullet is instantiated.
    public GameObject GetPooledObject(GameObject prefab)
    {
        // Check that the requested prefab matches the pool's bullet prefab.
        if (prefab != bulletPrefab)
        {
            Debug.LogWarning("Requested prefab does not match the bulletPrefab in the pool!");
            return null;
        }

        // Find an inactive bullet in the pool.
        foreach (GameObject bullet in pooledBullets)
        {
            if (!bullet.activeInHierarchy)
            {
                return bullet;
            }
        }

        // If none are available, instantiate a new bullet, add it to the pool, and return it.
        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.SetActive(false);
        pooledBullets.Add(newBullet);
        return newBullet;
    }

    // Return a bullet to the pool by deactivating it.
    public void ReturnToPool(GameObject bullet)
    {
        bullet.SetActive(false);
    }
}
