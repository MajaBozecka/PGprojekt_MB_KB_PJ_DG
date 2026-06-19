using UnityEngine;
using UnityEngine.UI;

public class GalleryManager : MonoBehaviour
{
    public CollectiblesDatabaseSO database;
    public GameObject slotPrefab;
    public Transform gridContent; 

    void OnEnable()
    {
        RefreshGallery();
    }

    public void RefreshGallery()
    {
        foreach (Transform child in gridContent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in database.allCollectibles)
        {
            GameObject newSlot = Instantiate(slotPrefab, gridContent);
            Image slotImage = newSlot.GetComponent<Image>();

            slotImage.sprite = item.itemSprite;

            if (item.isUnlocked)
            {
                slotImage.color = Color.white;
            }
            else
            {
                slotImage.color = Color.black;
            }
        }
    }
}