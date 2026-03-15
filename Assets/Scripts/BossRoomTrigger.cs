using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    public Hornet bossScript;

    public AudioSource musicSource;
    public AudioClip bossMusic;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (musicSource && bossMusic)
            {
                musicSource.clip = bossMusic;
                musicSource.Play(); // Restarts the source with the new track
            }

            Debug.Log("Player entered the arena! Waking up Hornet.");

            if (bossScript != null)
            {
                bossScript.enabled = true;
            }
            gameObject.SetActive(false); // Destroy trigger so it only fires once
        }
    }
}