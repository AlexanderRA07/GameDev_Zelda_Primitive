using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Bow & Arrow")]
    public bool hasBow = false;        // Set by the chest
    public GameObject arrowPrefab;     // Your arrow projectle
    public float arrowCooldown = 0.5f;
    float arrowTimer = 0f;

    [Header("Movement")]
    public float moveSpeed = 4f;

    [Header("Sword")]
    public GameObject swordPrefab;      // a child GameObject with a trigger collider
    public float attackDuration = 0.3f; // how long the sword hitbox stays active
    public float attackCooldown = 0.4f;

    [Header("Invincibility")]
    public float iframeDuration = 1.0f; // seconds of invincibility after a hit

    // ── Private state ─────────────────────────────────────────────
    Rigidbody2D rb;
    Vector2 moveInput;
    Vector2 facingDir = Vector2.down;   // start facing down like NES Link

    bool isAttacking   = false;
    float attackTimer  = 0f;
    float cooldownTimer = 0f;

    bool isInvincible  = false;
    float iframeTimer  = 0f;

    SpriteRenderer sr;

    // Sword offsets per direction (up/down/left/right)
    // Adjust these values to match your sprite size
    readonly Vector2[] dirOffsets = {
        new Vector2( 0,    0.6f),  // up
        new Vector2( 0,   -0.6f),  // down
        new Vector2(-0.6f, 0),     // left
        new Vector2( 0.6f, 0),     // right
    };

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        HandleInput();
        HandleAttackTimers();
        HandleIframes();

        if (arrowTimer > 0) arrowTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        // Don't move while attacking (NES Zelda locks movement during sword swing)
        if (isAttacking)
            rb.linearVelocity = Vector2.zero;
        else
            rb.linearVelocity = moveInput * moveSpeed;
    }

    // ── Input ──────────────────────────────────────────────────────
    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // NES Zelda: 4-directional only — prioritize horizontal
        if (h != 0)
        {
            moveInput = new Vector2(h, 0);
            facingDir = new Vector2(h, 0);
        }
        else if (v != 0)
        {
            moveInput = new Vector2(0, v);
            facingDir = new Vector2(0, v);
        }
        else
        {
            moveInput = Vector2.zero;
        }

        // Bow Attack
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
            && !isAttacking && cooldownTimer <= 0f)
        {
            StartAttack();
        }

        if (hasBow && Input.GetKeyDown(KeyCode.F) && arrowTimer <= 0f && !isAttacking)
        {
            ShootArrow();
        }
    }

    // ── Attack ─────────────────────────────────────────────────────
    void StartAttack()
    {
        isAttacking  = true;
        attackTimer  = attackDuration;
        cooldownTimer = attackCooldown;

        if (swordPrefab != null)
        {
            // Place sword hitbox in facing direction
            Vector3 offset = (Vector3)GetFacingOffset();
            swordPrefab.transform.localPosition = offset;

            // Rotate sword sprite to face the right way
            float angle = Mathf.Atan2(facingDir.y, facingDir.x) * Mathf.Rad2Deg;
            swordPrefab.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            swordPrefab.SetActive(true);
        }
    }

    void HandleAttackTimers()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                if (swordPrefab != null) swordPrefab.SetActive(false);
            }
        }
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;
    }

    Vector2 GetFacingOffset()
    {
        if (facingDir == Vector2.up)    return dirOffsets[0];
        if (facingDir == Vector2.down)  return dirOffsets[1];
        if (facingDir == Vector2.left)  return dirOffsets[2];
        return dirOffsets[3]; // right
    }

    // ── Damage / Iframes ───────────────────────────────────────────
    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        GameManager.Instance.TakeDamage(amount);

        isInvincible = true;
        iframeTimer  = iframeDuration;

        // Knock player back slightly
        Vector2 knockback = -facingDir * 2f;
        rb.AddForce(knockback, ForceMode2D.Impulse);
    }

    void HandleIframes()
    {
        if (!isInvincible) return;

        iframeTimer -= Time.deltaTime;

        // Blink effect: toggle sprite visibility rapidly
        sr.enabled = (Mathf.FloorToInt(iframeTimer * 10f) % 2 == 0);

        if (iframeTimer <= 0f)
        {
            isInvincible = false;
            sr.enabled   = true;
        }
    }

    void ShootArrow()
    {
        arrowTimer = arrowCooldown;
        if (arrowPrefab == null) return;

        // 1. Force facingDir to be a "pure" direction (Up, Down, Left, or Right)
        Vector2 cleanDir = Vector2.zero;
        float spriteRotation = 0f;

        if (Mathf.Abs(facingDir.x) > Mathf.Abs(facingDir.y))
        {
            // Shooting Horizontally
            cleanDir = new Vector2(Mathf.Sign(facingDir.x), 0);
            spriteRotation = (cleanDir.x > 0) ? -90f : 90f; // Right is -90, Left is 90
        }
        else
        {
            // Shooting Vertically
            cleanDir = new Vector2(0, Mathf.Sign(facingDir.y));
            spriteRotation = (cleanDir.y > 0) ? 0f : 180f; // Up is 0, Down is 180
        }

        // 2. Spawn the arrow with the fixed rotation
        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, spriteRotation));

        // 3. Give it velocity directly so it doesn't drift
        Rigidbody2D rb = arrow.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = cleanDir * 12f; // 12 is the arrow speed
        }
    }
}
