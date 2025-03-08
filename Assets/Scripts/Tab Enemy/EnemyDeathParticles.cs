using UnityEngine;

public class TabDeathParticle : MonoBehaviour
{
    private TabDeathParticlePool particlePool;

    private void Start()
    {
        particlePool = FindObjectOfType<TabDeathParticlePool>();
    }

    public void SpawnDeathParticle()
    {
        if (particlePool != null)
        {
            particlePool.GetParticleEffect(transform.position);
        }
    }
}