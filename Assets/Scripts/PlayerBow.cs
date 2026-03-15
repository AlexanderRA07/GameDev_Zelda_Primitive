using UnityEngine;

public class PlayerBow : MonoBehaviour
{
    public GameObject arrowPrefab;
    public Transform firePoint; // A child object of Player showing where arrows come out

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F)) // Press F to fire
        {
            Shoot();
        }
    }

    void Shoot()
    {
        // Spawn arrow at the firePoint position and rotation
        Instantiate(arrowPrefab, firePoint.position, firePoint.rotation);
    }
}