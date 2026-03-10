using UnityEngine;

// ─────────────────────────────────────────────────────────────────
// DoorController.cs
// Attach to each Door GameObject (4 doors: N/S/E/W).
// The door should have a SpriteRenderer and a Collider2D (non-trigger).
// When Open() is called the collider is disabled and the sprite changes color.
// ─────────────────────────────────────────────────────────────────
public class DoorController : MonoBehaviour
{
    public Color openColor   = new Color(0.78f, 0.66f, 0.29f); // golden
    public Color closedColor = new Color(0.1f,  0.1f,  0.1f);  // dark

    SpriteRenderer sr;
    Collider2D col;

    void Awake()
    {
        sr  = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        // Start closed
        if (sr) sr.color = closedColor;
    }

    public void Open()
    {
        if (col) col.enabled = false;         // player can now walk through
        if (sr)  sr.color    = openColor;
    }
}


// ─────────────────────────────────────────────────────────────────
// ChestTrigger.cs
// Attach to the Chest GameObject.
// Has a BoxCollider2D set to IS TRIGGER = true.
// Only activates after room is cleared (GameManager.roomCleared).
// ─────────────────────────────────────────────────────────────────
public class ChestTrigger : MonoBehaviour
{
    public GameObject triforceSprite;   // optional glow / icon to show on open

    bool collected = false;

    void Start()
    {
        // Hide chest until room is cleared
        gameObject.SetActive(false);
    }

    void Update()
    {
        // Enable chest once room is cleared
        if (!gameObject.activeSelf && GameManager.Instance != null
            && GameManager.Instance.roomCleared)
        {
            gameObject.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;
        if (!GameManager.Instance.roomCleared) return;

        collected = true;
        if (triforceSprite) triforceSprite.SetActive(true);
        GameManager.Instance.TriggerWin();
    }
}
