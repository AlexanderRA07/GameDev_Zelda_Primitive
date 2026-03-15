using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int smallKeys = 0;

    public void AddKey()
    {
        smallKeys++;
        UIManager.Instance.UpdateKeyUI(smallKeys); // Update the screen
    }

    public bool UseKey()
    {
        if (smallKeys > 0)
        {
            smallKeys--;
            UIManager.Instance.UpdateKeyUI(smallKeys); // Update the screen
            return true;
        }
        return false;
    }
}