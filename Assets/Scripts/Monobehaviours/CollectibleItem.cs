using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Tooltip("Musi siê zgadzaæ z ID wpisanym w bazie (np. 'klapki')")]
    public string collectibleId;

    public CollectiblesDatabaseSO database;

    public void Collect()
    {
        Debug.Log("KROK 1: Odpalono Collect() na obiekcie: " + gameObject.name);

        if (database == null)
        {
            Debug.LogError("B£¥D: Baza danych (DB) nie jest podpiêta pod ten konkretny obiekt na scenie!");
            return;
        }

        Debug.Log("KROK 2: Baza podpiêta. Szukam ID: '" + collectibleId + "' w bazie...");

        CollectibleData data = database.allCollectibles.Find(x => x.id == collectibleId);

        if (data == null)
        {
            Debug.LogError("B£¥D: Nie znaleziono przedmiotu o ID '" + collectibleId + "' w pliku ScriptableObject! (SprawdŸ literówki/spacje)");
            return;
        }

        Debug.Log("KROK 3: Znaleziono przedmiot w bazie: " + data.displayName + ". Czy by³ ju¿ odblokowany? " + data.isUnlocked);

        if (!data.isUnlocked)
        {
            data.isUnlocked = true;
            Debug.Log("<color=yellow>KROK 4: Zebrano znajdŸkê!</color> Zapisano progres w bazie.");
        }

        Debug.Log("KROK 5: Usuwam obiekt ze sceny (ukrywam).");
        gameObject.SetActive(false);
    }
}