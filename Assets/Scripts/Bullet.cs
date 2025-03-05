using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 moveDirection;
    private float moveSpeed;
    private ObjectPool pool;
    private Rigidbody2D rb;

    public void Initialize(Vector2 direction, float speed, ObjectPool objectPool)
    {
        moveDirection = direction;
        moveSpeed = speed;
        pool = objectPool;
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("Bullet is missing Rigidbody2D!");
            return;
        }

        rb.linearVelocity = moveDirection * moveSpeed; // Ensures a straight path
    }

    private void Update()
    {
        // Ensures the bullet always moves in the exact same direction
        transform.position += (Vector3)moveDirection * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ReturnToPool();
    }

    private void OnBecameInvisible()
    {
        ReturnToPool(); // Return bullet to the pool when off-screen
    }

    private void ReturnToPool()
    {
        rb.linearVelocity = Vector2.zero; // Reset velocity before returning
        gameObject.SetActive(false);
        pool.ReturnObject(gameObject);
    }
}