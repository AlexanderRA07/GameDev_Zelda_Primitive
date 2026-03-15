using UnityEngine;

public class BossManager : MonoBehaviour
{
    [Header("References")]
    public GameObject bossObject;    // Drag your Boss enemy here
    public GameObject victoryChest; // Drag your Victory Chest here

    private bool chestSpawned = false;

    void Start()
    {
        // Ensure the chest is hidden at the start of the fight
        if (victoryChest != null) victoryChest.SetActive(false);
    }

    void Update()
    {
        // Check if the boss is gone
        if (bossObject == null && !chestSpawned)
        {
            SpawnChest();
        }
    }

    void SpawnChest()
    {
        chestSpawned = true;
        if (victoryChest != null)
        {
            victoryChest.SetActive(true);
            Debug.Log("Boss defeated! Victory Chest spawned.");
        }
    }
}