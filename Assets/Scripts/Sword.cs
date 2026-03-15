using UnityEngine;

public class Sword : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check for Regular Enemies
        if (other.TryGetComponent(out EnemyAI enemy))
        {
            enemy.TakeDamage(damage);
            return; // Exit so we don't check other types
        }

        // 3. Check for Hornet
        if (other.TryGetComponent(out Hornet hornet))
        {
            hornet.TakeDamage(damage);
            return;
        }
    }
}