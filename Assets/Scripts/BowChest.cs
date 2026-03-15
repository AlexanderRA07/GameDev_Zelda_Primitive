using UnityEngine;

public class BowChest : MonoBehaviour
{

    public AudioSource audioSource; // Drag SoundManager here
    public AudioClip chestOpenSound;

    [Header("Visuals")]
    public Sprite openedSprite;
    public GameObject itemVisual; // The bow icon that floats up

    private bool isOpened = false;
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        // Start hidden until the room is clear (using your GameManager logic)
        if (sr) sr.enabled = false;
        if (col) col.enabled = false;
        if (itemVisual) itemVisual.SetActive(false);
    }

    void Update()
    {
        // Reveal logic using your existing GameManager instance
        if (sr != null && !sr.enabled && GameManager.Instance != null && GameManager.Instance.roomCleared)
        {
            sr.enabled = true;
            col.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the solid player object bumped into the chest
        if (!isOpened && collision.gameObject.CompareTag("Player"))
        {
            OpenChest(collision.gameObject);
        }
    }

    void OpenChest(GameObject player)
    {
        if (audioSource && chestOpenSound)
        {
            audioSource.PlayOneShot(chestOpenSound);
        }

        isOpened = true;
        if (sr) sr.sprite = openedSprite;

        PlayerController pc = player.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.hasBow = true;
            UIManager.Instance.ShowNotification("You got a Bow! Press F to shoot.");
        }

        if (itemVisual) itemVisual.SetActive(true);
    }
}