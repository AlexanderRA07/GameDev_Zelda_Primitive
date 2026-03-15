using UnityEngine;
using UnityEngine.UI;

public class HeartUIController : MonoBehaviour
{
    [Header("Heart UI")]
    public Image[] heartImages;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public void updateHearts(int currentHP, int maxHearts)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            int heartHP = currentHP - (i * 2);
            if (heartHP >= 2)
                heartImages[i].sprite = fullHeart;
            else if (heartHP == 1)
                heartImages[i].sprite = halfHeart;
            else
                heartImages[i].sprite = emptyHeart;
        }
    }
}