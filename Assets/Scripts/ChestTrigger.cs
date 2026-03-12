using UnityEngine;

// ─────────────────────────────────────────────────────────────────
// ChestTrigger.cs (UPDATED)
// Attach to the Chest GameObject.
// Has a BoxCollider2D set to IS TRIGGER = true.
// Only activates visuals and collision after room is cleared.
// ─────────────────────────────────────────────────────────────────
public class ChestTrigger : MonoBehaviour
{
    public GameObject triforceSprite;   // optional glow / icon to show on open

    bool collected = false;

    // References to the components we want to hide
    SpriteRenderer sr;
    Collider2D col;

    void Awake()
    {
        // Grab the components off the chest when the game starts
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Hide the chest visuals and turn off its hitbox, 
        // but KEEP the GameObject active so Update() keeps running!
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;

        if (triforceSprite != null) triforceSprite.SetActive(false);
    }

    void Update()
    {
        // Reveal the chest once the room is cleared
        if (sr != null && !sr.enabled && GameManager.Instance != null && GameManager.Instance.roomCleared)
        {
            sr.enabled = true;   // Make it visible
            col.enabled = true;  // Make it touchable
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        if (!GameManager.Instance.roomCleared) return;

        collected = true;

        // Show the item floating above the chest
        if (triforceSprite != null) triforceSprite.SetActive(true);

        // Trigger the win screen
        GameManager.Instance.TriggerWin();
    }
}