using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer; 
    public List<LocationData> allLocations; 

    [SerializeField]
    private LocationData currentLocation;

    void Start()
    {

        foreach (var loc in allLocations)
        {
            loc.SetRoomActive(false);
        }

        if (currentLocation != null)
        {
            ChangeLocation(currentLocation.locationId);
        }
    }

    public void ChangeLocation(string newLocationId)
    {
        // Szukamy nowego pokoju na liœcie
        LocationData targetLocation = allLocations.Find(x => x.locationId == newLocationId);

        if (targetLocation != null)
        {
            if (currentLocation != null)
            {
                currentLocation.SetRoomActive(false);
            }

            backgroundRenderer.sprite = targetLocation.backgroundSprite;

            targetLocation.SetRoomActive(true);
            currentLocation = targetLocation;

            Debug.Log($"Przeniesiono do: {newLocationId}");
        }
        else
        {
            Debug.LogWarning($"Nie znaleziono lokacji o ID: {newLocationId}");
        }
    }
}