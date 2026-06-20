using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CollectibleData
{
    public string id;
    public string displayName;
    public Sprite itemSprite;
    public bool isUnlocked = false;
}

[CreateAssetMenu(fileName = "CollectiblesDatabase", menuName = "Game/Collectibles Database")]
public class CollectiblesDatabaseSO : ScriptableObject
{
    public List<CollectibleData> allCollectibles = new List<CollectibleData>();

    public void ResetProgress()
    {
        foreach (var item in allCollectibles)
        {
            item.isUnlocked = false;
        }
        Debug.Log("Progres znajdziek zosta³ wyzerowany.");
    }
}