using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio")]
    public AudioSource musicSource;
    public AudioClip deathMusic;
    public AudioClip winMusic;

    [Header("Health")]
    public int maxHearts = 3;           // each heart = 2 HP
    public int currentHP;               // starts at 6 (3 full hearts)

    [Header("UI References")]
    public TextMeshProUGUI heartDisplay;           // drag a UI Text here in Inspector
    public TextMeshProUGUI statusText;             // "Defeat all enemies!" etc.
    public GameObject winPanel;         // panel shown on win
    public GameObject deadPanel;        // panel shown on death

    [Header("Room State")]
    public int enemiesRemaining;
    public bool roomCleared = false;

    void Awake()
    {
        // Singleton so any script can call GameManager.Instance
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentHP = maxHearts * 2;
        UpdateHeartUI();
        if (winPanel)  winPanel.SetActive(false);
        if (deadPanel) deadPanel.SetActive(false);
    }

    // ── Called by EnemyAI when an enemy dies ──────────────────────
    public void EnemyKilled()
    {
        enemiesRemaining--;
        if (enemiesRemaining <= 0 && !roomCleared)
        {
            roomCleared = true;
            OpenDoors();
            if (statusText) statusText.text = "The doors are open! Find the chest!";
        }
    }

    // ── Called by PlayerController on hit ─────────────────────────
    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        currentHP = Mathf.Max(currentHP, 0);
        UpdateHeartUI();

        if (currentHP <= 0) TriggerDeath();
    }

    // ── Called by ChestTrigger ─────────────────────────────────────
    public void TriggerWin()
    {
        if (musicSource != null && winMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = winMusic;
            musicSource.loop = true; // Let the victory music loop on the win screen
            musicSource.Play();
        }
        Time.timeScale = 0f;            // freeze the game
        if (winPanel) winPanel.SetActive(true);
    }

    public void TriggerDeath()
    {
        if (musicSource != null && deathMusic != null)
        {
            musicSource.Stop();
            musicSource.clip = deathMusic;
            musicSource.loop = false; // Usually, death music plays once
            musicSource.Play();
        }
        Time.timeScale = 0f;
        if (deadPanel) deadPanel.SetActive(true);
    }

    // ── Restart (wire this to a UI Button's OnClick) ───────────────
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ── Open all Door objects in the scene ────────────────────────
    void OpenDoors()
    {
        foreach (var door in FindObjectsByType<DoorController>(FindObjectsSortMode.None))
            door.Open();
    }

    // ── Heart display: ♥♥♡ style ──────────────────────────────────
    void UpdateHeartUI()
    {
        if (heartDisplay == null) return;
        int full  = currentHP / 2;
        int half  = currentHP % 2;
        int empty = maxHearts - full - half;
        heartDisplay.text =
            new string('♥', full) +
            (half == 1 ? "♡" : "") +
            new string('♡', empty);
    }
}
