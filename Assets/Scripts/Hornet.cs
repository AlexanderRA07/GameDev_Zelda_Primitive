using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class Hornet : MonoBehaviour
{
    public enum BossState { Moving, PreparingDash, Dashing }

    [Header("Audio")]
    public AudioSource sfxSource;   // Drag SoundManager here
    public AudioClip dashSound;     // The "Whoosh" or "Slash" sound

    [Header("Health System")]
    public int maxHP = 10;
    public int currentHP; // Made public so you can see it in the Inspector
    private bool isDead = false;

    [Header("Movement Stats")]
    public float normalSpeed = 2f;
    public float dashSpeed = 12f;
    public float dashCooldown = 3f;
    public float preparationTime = 0.8f;

    [Header("Visuals")]
    private SpriteRenderer sr;
    public Color chargeColor = Color.yellow;
    public Color hitColor = Color.red;
    private Color baseColor;
    private float flashTimer = 0f;

    // Internal Logic
    private Rigidbody2D rb;
    private Transform player;
    private BossState currentState = BossState.Moving;
    private float stateTimer;
    private Vector2 dashDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        currentHP = maxHP;
        if (sr != null) baseColor = sr.color;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        stateTimer = dashCooldown;

        Debug.Log("Boss Initialized with " + maxHP + " HP.");
    }

    void Update()
    {
        if (isDead || player == null) return;

        HandleFlashVisuals();

        stateTimer -= Time.deltaTime;

        switch (currentState)
        {
            case BossState.Moving:
                if (stateTimer <= 0) StartPreparation();
                break;
            case BossState.PreparingDash:
                if (stateTimer <= 0) StartDash();
                break;
            case BossState.Dashing:
                if (stateTimer <= 0) EndDash();
                break;
        }
    }

    void FixedUpdate()
    {
        if (isDead || player == null) return;

        if (currentState == BossState.Moving)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.linearVelocity = dir * normalSpeed;
        }
        else if (currentState == BossState.Dashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    // --- State Logic ---
    void StartPreparation()
    {
        currentState = BossState.PreparingDash;
        stateTimer = preparationTime;
        Debug.Log("Boss is PREPARING dash.");
        if (sr != null) sr.color = chargeColor;
    }

    void StartDash()
    {
        currentState = BossState.Dashing;
        stateTimer = 0.5f;
        dashDirection = (player.position - transform.position).normalized;
        Debug.Log("Boss is DASHING.");

        if (sfxSource && dashSound)
        {
            sfxSource.PlayOneShot(dashSound);
        }

        if (sr != null) sr.color = baseColor;
    }

    void EndDash()
    {
        currentState = BossState.Moving;
        stateTimer = dashCooldown;
    }

    // --- Damage Logic ---
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;
        flashTimer = 0.2f; // Increased slightly for visibility

        Debug.Log("<color=red>BOSS TOOK DAMAGE!</color> Amount: " + amount + " | Current HP: " + currentHP);

        if (currentHP <= 0) Die();
    }

    void Die()
    {
        isDead = true;
        Debug.Log("<color=green>BOSS DIED.</color> Notifying GameManager.");
        if (GameManager.Instance != null) GameManager.Instance.EnemyKilled();
        Destroy(gameObject, 0.1f);
    }

    void HandleFlashVisuals()
    {
        if (flashTimer > 0f)
        {
            flashTimer -= Time.deltaTime;
            if (sr != null) sr.color = hitColor;
        }
        else if (currentState == BossState.PreparingDash)
        {
            if (sr != null) sr.color = chargeColor;
        }
        else
        {
            if (sr != null) sr.color = baseColor;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            Debug.Log("Boss hit the Player!");
            PlayerController pc = col.gameObject.GetComponent<PlayerController>();
            if (pc != null) pc.TakeDamage(1);
        }
    }
}