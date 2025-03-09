using UnityEngine;
            using System.Collections.Generic;
            
            public class ExplosiveTabExplosionPool : MonoBehaviour
            {
                public static ExplosiveTabExplosionPool Instance { get; private set; }
            
                public GameObject explosivePrefab;
                public int poolSize = 10;
            
                private Queue<GameObject> poolQueue;
            
                private void Awake()
                {
                    // Simple singleton pattern
                    if (Instance == null) Instance = this;
                    else Destroy(gameObject);
            
                    poolQueue = new Queue<GameObject>();
                    for (int i = 0; i < poolSize; i++)
                    {
                        GameObject obj = Instantiate(explosivePrefab, transform);
                        // Set the pool reference when creating explosions
                        Explosion explosionComponent = obj.GetComponent<Explosion>();
                        if (explosionComponent != null)
                        {
                            explosionComponent.SetPool(this);
                        }
                        obj.SetActive(false);
                        poolQueue.Enqueue(obj);
                    }
                }
            
                public GameObject GetExplosion()
                {
                    if (poolQueue.Count > 0)
                    {
                        GameObject explosion = poolQueue.Dequeue();
                        explosion.SetActive(true);
                        return explosion;
                    }
                    else
                    {
                        GameObject explosion = Instantiate(explosivePrefab, transform);
                        // Set the pool reference for newly created explosions
                        Explosion explosionComponent = explosion.GetComponent<Explosion>();
                        if (explosionComponent != null)
                        {
                            explosionComponent.SetPool(this);
                        }
                        return explosion;
                    }
                }
            
                // Renamed method to match what Explosion.cs is calling
                public void ReturnToPool(GameObject explosion)
                {
                    explosion.SetActive(false);
                    poolQueue.Enqueue(explosion);
                }
            }