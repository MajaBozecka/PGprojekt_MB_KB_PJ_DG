using System.Collections.Generic;
using UnityEngine;

public class LocationData : MonoBehaviour
{
    [Header("Ustawienia Lokacji")]
    public string locationId;
    public Sprite backgroundSprite;

    [Tooltip("Odznacz, jeœli gracz nie powinien móc tu wróciæ (np. cutscenki, intro)")]
    public bool allowReturnToThisRoom = true;

    [Header("Obiekty w tym pokoju")]
    public List<GameObject> interactablesInRoom;

    public void SetRoomActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}