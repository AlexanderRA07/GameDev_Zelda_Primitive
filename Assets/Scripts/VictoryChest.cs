using UnityEngine;

public class VictoryChest : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite openedSprite;    // The "Open" chest lid
    public Sprite triforceSprite;  // The actual Triforce item

    [Header("Settings")]
    public float itemFloatHeight = 1.5f; // How high the Triforce floats up

    private bool isOpened = false;
    private SpriteRenderer chestSR;

    void Awake()
    {
        chestSR = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isOpened && collision.gameObject.CompareTag("Player"))
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        // 1. Change the chest to the "Open" look
        if (chestSR && openedSprite) chestSR.sprite = openedSprite;

        // 2. Create the Triforce item visually
        SpawnTriforce();

        Debug.Log("Victory Chest Opened!");

        // 3. Trigger the Win Screen
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TriggerWin();
        }
    }

    void SpawnTriforce()
    {
        // Create a new empty GameObject to hold the Triforce
        GameObject triforce = new GameObject("TriforceReward");
        triforce.transform.position = transform.position; // Start at chest
        triforce.transform.SetParent(this.transform);

        // Add a SpriteRenderer and set it to your Triforce sprite
        SpriteRenderer triforceSR = triforce.AddComponent<SpriteRenderer>();
        triforceSR.sprite = triforceSprite;
        triforceSR.sortingOrder = chestSR.sortingOrder + 1; // Make sure it's in front

        // Move it upward slightly so it "pops" out
        triforce.transform.localPosition = new Vector3(0, itemFloatHeight, 0);
    }
}