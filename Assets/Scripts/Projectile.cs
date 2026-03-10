using UnityEngine;

// Attach to the Projectile prefab (a small sprite with a CircleCollider2D trigger)

public class Projectile : MonoBehaviour
{
    public float speed  = 4f;
    public float lifespan = 3f;     // auto-destroy after this many seconds
    public int   damage = 1;

    Vector2 dir;

    void Start()
    {
        Destroy(gameObject, lifespan);
    }

    public void SetDirection(Vector2 direction)
    {
        dir = direction.normalized;
        // Rotate sprite to face movement direction (optional)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(dir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit the player
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(damage);
            Destroy(gameObject);
        }

        // Hit a wall
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
