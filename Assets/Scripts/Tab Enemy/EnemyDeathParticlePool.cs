using UnityEngine;
using System.Collections.Generic;

public class TabDeathParticlePool : MonoBehaviour
{
    public GameObject particlePrefab;
    public int poolSize = 10;
    
    private Queue<GameObject> particlePool = new Queue<GameObject>();

    private void Start()
    {
        // Initialize the pool
        for (int i = 0; i < poolSize; i++)
        {
            GameObject particle = Instantiate(particlePrefab);
            particle.SetActive(false);
            particlePool.Enqueue(particle);
        }
    }

    public GameObject GetParticleEffect(Vector3 position)
    {
        GameObject particle;
        
        if (particlePool.Count > 0)
        {
            particle = particlePool.Dequeue();
        }
        else
        {
            // Expand pool if needed
            particle = Instantiate(particlePrefab);
        }

        particle.transform.position = position;
        particle.SetActive(true);

        // Disable after effect duration
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            float duration = ps.main.duration;
            StartCoroutine(ReturnToPool(particle, duration));
        }

        return particle;
    }

    private System.Collections.IEnumerator ReturnToPool(GameObject particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        particle.SetActive(false);
        particlePool.Enqueue(particle);
    }
}