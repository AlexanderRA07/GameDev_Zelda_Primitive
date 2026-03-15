using UnityEngine;
using TMPro; // Needed for Text Mesh Pro

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI keyCountText;
    public TextMeshProUGUI notificationText;

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void UpdateKeyUI(int amount)
    {
        keyCountText.text = "Keys: " + amount;
    }

    public void ShowNotification(string message)
    {
        notificationText.text = message;
        // Hide the text after 3 seconds
        Invoke("ClearNotification", 3f);
    }

    void ClearNotification()
    {
        notificationText.text = "";
    }
}