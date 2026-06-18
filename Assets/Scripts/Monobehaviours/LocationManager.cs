using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Dodane dla obs³ugi przycisku UI

public class LocationManager : MonoBehaviour
{
    public SpriteRenderer backgroundRenderer;
    public List<LocationData> allLocations;

    [SerializeField]
    private LocationData currentLocation;

    [Header("UI Powrotu")]
    public GameObject backButtonObject; // Przeci¹gniesz tu przycisk z Canvasa

    private bool isDialogueActive = false;

    // Stos pamiêtaj¹cy ID poprzednich pokoi
    private Stack<string> locationHistory = new Stack<string>();

    void Start()
    {
        foreach (var loc in allLocations)
        {
            loc.SetRoomActive(false);
        }

        if (currentLocation != null)
        {
            ChangeLocation(currentLocation.locationId, false, true); // true = start gry
        }
    }

    public void SetDialogueState(bool isActive)
    {
        isDialogueActive = isActive;
        UpdateBackButtonVisibility(); // Odœwie¿amy widocznoœæ przycisku po zmianie stanu
    }

    // Zmieniona funkcja, która teraz wie, czy idziemy do przodu, czy wracamy
    public void ChangeLocation(string newLocationId, bool isGoingBack = false, bool isGameStart = false)
    {
        LocationData targetLocation = allLocations.Find(x => x.locationId == newLocationId);

        if (targetLocation != null)
        {
            // Jeœli idziemy do nowego pokoju, zapamiêtujemy stary na stosie
            if (!isGoingBack && !isGameStart && currentLocation != null)
            {
                locationHistory.Push(currentLocation.locationId);
            }

            if (currentLocation != null)
            {
                currentLocation.SetRoomActive(false);
            }

            backgroundRenderer.sprite = targetLocation.backgroundSprite;
            targetLocation.SetRoomActive(true);
            currentLocation = targetLocation;

            UpdateBackButtonVisibility();
        }
    }

    // Nowa funkcja wywo³ywana przez przycisk powrotu
    public void GoBack()
    {
        if (locationHistory.Count > 0)
        {
            // Zdejmujemy ostatni pokój ze stosu pamiêci i idziemy do niego
            string previousRoomId = locationHistory.Pop();
            ChangeLocation(previousRoomId, true); // isGoingBack = true
        }
    }

    // Pokazuje przycisk tylko wtedy, gdy mamy do czego wracaæ
    private void UpdateBackButtonVisibility()
    {
        if (backButtonObject != null)
        {
            backButtonObject.SetActive(locationHistory.Count > 0 && !isDialogueActive);
        }
    }
}