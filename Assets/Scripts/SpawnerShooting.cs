using UnityEngine;

public class SpawnerShooter : MonoBehaviour
{
    public enum ShootDirection { FourWay, XPattern }
    public ShootDirection shootingMode = ShootDirection.FourWay;

    private ObjectPool bulletPool;
    public float bulletSpeed = 5f; // Adjustable bullet speed

    private void Start()
    {
        bulletPool = FindObjectOfType<ObjectPool>();

        if (bulletPool == null)
        {
            Debug.LogError("No ObjectPool found in the scene! Make sure one exists.");
            return;
        }

        Shoot();
    }

    private void Shoot()
    {
        switch (shootingMode)
        {
            case ShootDirection.FourWay:
                ShootFourWay();
                break;
            case ShootDirection.XPattern:
                ShootXPattern();
                break;
        }
    }

    private void ShootFourWay()
    {
        Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        foreach (Vector2 dir in directions)
        {
            SpawnBullet(dir);
        }
    }

    private void ShootXPattern()
    {
        Vector2[] directions = {
            new Vector2(1, 1).normalized,
            new Vector2(-1, 1).normalized,
            new Vector2(1, -1).normalized,
            new Vector2(-1, -1).normalized
        };

        foreach (Vector2 dir in directions)
        {
            SpawnBullet(dir);
        }
    }

    private void SpawnBullet(Vector2 direction)
    {
        if (bulletPool == null) return;

        GameObject bullet = bulletPool.GetObject(transform.position, Quaternion.identity);
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed, bulletPool); // Set direction & speed
        }
        else
        {
            Debug.LogWarning("Bullet prefab is missing the Bullet script!");
        }
    }
}