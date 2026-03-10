using UnityEngine;

// Attach this to the Sword child GameObject on the Player prefab.
// The sword GameObject should have:
//   - a SpriteRenderer (a simple white rectangle sprite is fine)
//   - a BoxCollider2D set to IS TRIGGER = true

public class Sword : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Hit an enemy
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
