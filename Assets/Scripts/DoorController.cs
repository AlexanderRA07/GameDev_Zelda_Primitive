// ─────────────────────────────────────────────────────────────────
// DoorController.cs
// Attach to each Door GameObject (4 doors: N/S/E/W).
// The door should have a SpriteRenderer and a Collider2D (non-trigger).
// When Open() is called the collider is disabled and the sprite changes color.
// ─────────────────────────────────────────────────────────────────

using UnityEngine;

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