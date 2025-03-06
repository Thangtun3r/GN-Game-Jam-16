using UnityEngine;

public class SpawnerShooter : MonoBehaviour
{
    [Header("Bullet Settings")]
    [Tooltip("The bullet prefab that should have the Bullet script attached.")]
    public GameObject bulletPrefab;
    public BulletDirections bulletDirections = BulletDirections.Up;
    public float bulletSpeed = 5f;

    private void Start()
    {


        // Fire bullets in all chosen directions.
        Shoot();
    }

    private void Shoot()
    {
        if ((bulletDirections & BulletDirections.Up) != 0)
            SpawnBullet(Vector2.up);
        if ((bulletDirections & BulletDirections.Down) != 0)
            SpawnBullet(Vector2.down);
        if ((bulletDirections & BulletDirections.Left) != 0)
            SpawnBullet(Vector2.left);
        if ((bulletDirections & BulletDirections.Right) != 0)
            SpawnBullet(Vector2.right);
        if ((bulletDirections & BulletDirections.DiagonalUpLeft) != 0)
            SpawnBullet(new Vector2(-1, 1).normalized);
        if ((bulletDirections & BulletDirections.DiagonalUpRight) != 0)
            SpawnBullet(new Vector2(1, 1).normalized);
        if ((bulletDirections & BulletDirections.DiagonalDownLeft) != 0)
            SpawnBullet(new Vector2(-1, -1).normalized);
        if ((bulletDirections & BulletDirections.DiagonalDownRight) != 0)
            SpawnBullet(new Vector2(1, -1).normalized);
    }

    private void SpawnBullet(Vector2 direction)
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned!");
            return;
        }

        // Get the bullet instance from the universal pool.
        GameObject bullet = BulletPool.Instance.GetPooledObject(bulletPrefab);
        bullet.transform.position = transform.position;
        bullet.transform.rotation = Quaternion.identity;
        bullet.SetActive(true);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.Initialize(direction, bulletSpeed);
        }
        else
        {
            Debug.LogWarning("Bullet prefab is missing the Bullet script!");
        }
    }
}
