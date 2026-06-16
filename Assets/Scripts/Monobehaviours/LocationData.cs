using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour
{
    [Header("Ustawienia Lokacji")]
    public string locationId;
    public Sprite backgroundSprite;

    [Header("Obiekty w tym pokoju")]
    public List<GameObject> interactableInRoom;

    public void SetRoomActive(bool isActive)
    {
        foreach (GameObject obj in interactableInRoom)
        {
            if(obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
