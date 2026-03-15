using UnityEngine;

// Attach to each enemy prefab.
// Set enemyType in the Inspector to switch between Octorok and Keese behavior.

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public enum EnemyType { Octorok, Keese }

    [Header("Type")]
    public EnemyType enemyType = EnemyType.Octorok;

    [Header("Stats")]
    public int maxHP = 2;
    public float moveSpeed = 1.5f;
    public int contactDamage = 1;

    [Header("Octorok Shooting")]
    public GameObject projectilePrefab;
    public float shootInterval = 2.5f;

    // ── Private ───────────────────────────────────────────────────
    Rigidbody2D rb;
    SpriteRenderer sr;
    Transform player;

    int currentHP;
    bool isDead = false;

    // Patrol / AI timers
    float changeDirTimer;
    float shootTimer;
    Vector2 moveDir;

    // Flash on hit
    float flashTimer = 0f;
    Color baseColor;

    void Awake()
    {
        rb  = GetComponent<Rigidbody2D>();
        sr  = GetComponent<SpriteRenderer>();
        rb.gravityScale  = 0;
        rb.freezeRotation = true;
    }

    void Start()
    {
        currentHP  = maxHP;
        baseColor  = sr.color;
        player     = GameObject.FindGameObjectWithTag("Player")?.transform;

        // Randomize start timers so enemies aren't synchronized
        changeDirTimer = Random.Range(0.5f, 2f);
        shootTimer     = Random.Range(1f, shootInterval);

        PickNewDirection();
    }

    void Update()
    {
        if (isDead) return;

        HandleFlash();

        switch (enemyType)
        {
            case EnemyType.Octorok: UpdateOctorok(); break;
            case EnemyType.Keese:   UpdateKeese();   break;
        }
    }

    void FixedUpdate()
    {
        if (!isDead)
            rb.linearVelocity = moveDir * moveSpeed;
    }

    // ── Octorok: grid-patrol, occasionally shoot ──────────────────
    void UpdateOctorok()
    {
        // Change direction every so often
        changeDirTimer -= Time.deltaTime;
        if (changeDirTimer <= 0f)
        {
            changeDirTimer = Random.Range(1f, 2.5f);
            // 30% chance to walk toward player, otherwise pick random cardinal dir
            if (player != null && Random.value < 0.3f)
            {
                Vector2 toPlayer = (player.position - transform.position);
                moveDir = SnapToCardinal(toPlayer.normalized);
            }
            else
            {
                PickNewDirection();
            }
        }

        // Shoot at player
        if (player != null)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
            {
                shootTimer = shootInterval + Random.Range(-0.5f, 0.5f);
                Shoot();
            }
        }
    }

    // ── Keese: chase the player directly ─────────────────────────
    void UpdateKeese()
    {
        if (player == null) return;

        // Update chase direction frequently
        changeDirTimer -= Time.deltaTime;
        if (changeDirTimer <= 0f)
        {
            changeDirTimer = Random.Range(0.2f, 0.5f);
            Vector2 toPlayer = (player.position - transform.position).normalized;
            moveDir = toPlayer;
        }
    }

    // ── Helpers ────────────────────────────────────────────────────
    void PickNewDirection()
    {
        Vector2[] cardinals = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        moveDir = cardinals[Random.Range(0, cardinals.Length)];
    }

    Vector2 SnapToCardinal(Vector2 v)
    {
        if (Mathf.Abs(v.x) > Mathf.Abs(v.y))
            return new Vector2(Mathf.Sign(v.x), 0);
        return new Vector2(0, Mathf.Sign(v.y));
    }

    void Shoot()
    {
        if (projectilePrefab == null) return;
        Vector2 dir = (player.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null) p.SetDirection(dir);
    }

    // ── Wall bouncing (called by Unity physics) ───────────────────
    void OnCollisionEnter2D(Collision2D col)
    {
        // The 'col.gameObject' is what we hit
        if (col.gameObject.CompareTag("Wall"))
        {
            PickNewDirection();
        }
    }

    // ── Player contact damage ──────────────────────────────────────
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController pc = other.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(contactDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        currentHP -= amount;
        flashTimer = 0.15f;

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        GameManager.Instance.EnemyKilled();
        // Simple death: just destroy. You can spawn a puff particle here.
        Destroy(gameObject, 0.05f);
    }

    void HandleFlash()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            sr.enabled = (Mathf.FloorToInt(Time.time * 20f) % 2 == 0);
        }
        else
        {
            sr.enabled = true;
            sr.color = baseColor;
        }
    }
}
