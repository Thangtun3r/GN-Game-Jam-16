using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private Rigidbody2D rb;

    // Initialize no longer needs an object pool reference.
    public void Initialize(Vector2 direction, float speed)
    {
        moveDirection = direction;
        moveSpeed = speed;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet is missing Rigidbody2D!");
            return;
        }

        // Set the bullet's velocity.
        rb.linearVelocity = moveDirection * moveSpeed;
    }

    private void Update()
    {
        // If you want to use physics only, you can remove this manual position update.
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReturnToPool();
    }

    private void OnBecameInvisible()
    {
        ReturnToPool(); // Return bullet to the pool when it goes off-screen.
    }

    private void ReturnToPool()
    {
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
        // Return this bullet to the BulletPool.
        BulletPool.Instance.ReturnToPool(gameObject);
    }
}