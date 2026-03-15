using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 12f;
    public int damage = 1;

    void Start()
    {
        // Destroy arrow after 2 seconds to prevent memory leaks
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to hit a Regular Enemy
        if (other.TryGetComponent(out EnemyAI enemy))
        {
            enemy.TakeDamage(damage);
            Debug.Log("Arrow hit regular enemy!");
            Destroy(gameObject);
            return; // Exit so we don't check for other types
        }

        // Try to hit Hornet
        if (other.TryGetComponent(out Hornet hornet))
        {
            hornet.TakeDamage(damage);
            Debug.Log("Arrow hit a hornet!");
            Destroy(gameObject);
            return;
        }

        // Hit a Wall
        if (other.CompareTag("Wall"))
        {
            Debug.Log("Arrow hit a wall.");
            Destroy(gameObject);
        }
    }
}