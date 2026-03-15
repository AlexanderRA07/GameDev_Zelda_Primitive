using UnityEngine;

// Attach to the Shield child GameObject on the Player.
// The shield moves to always face the direction Link is facing,
// blocking projectiles that come from the front.

public class Shield : MonoBehaviour
{
    // Reference to the PlayerController to read facing direction
    PlayerController player;

    // Offsets matching the sword directions (up/down/left/right)
    readonly Vector2[] dirOffsets = {
        new Vector2( 0,    0.5f),  // up
        new Vector2( 0,   -0.5f),  // down
        new Vector2(-0.5f, 0),     // left
        new Vector2( 0.5f, 0),     // right
    };

    void Start()
    {
        // Get the PlayerController from the parent object
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (player == null) return;

        // Move shield to face the direction Link is facing
        transform.localPosition = GetFacingOffset();

        // Rotate shield sprite to match facing direction
        float angle = Mathf.Atan2(
            transform.localPosition.y,
            transform.localPosition.x
        ) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    Vector2 GetFacingOffset()
    {
        // Read the facing direction from PlayerController
        Vector2 facing = player.facingDir;

        if (facing == Vector2.up)    return dirOffsets[0];
        if (facing == Vector2.down)  return dirOffsets[1];
        if (facing == Vector2.left)  return dirOffsets[2];
        return dirOffsets[3]; // right
    }

    // Block projectiles that enter the shield trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj != null)
        {
            // Destroy the projectile — it has been blocked
            Destroy(other.gameObject);
        }
    }
}