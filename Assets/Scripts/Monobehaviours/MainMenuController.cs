using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Ustawienia Scen")]
    [Tooltip("Wpisz dok³adn¹ nazwê sceny intro")]
    public string startingSceneName = "Intro";

    public void ContinueGame()
    {
        Debug.Log("Klikniêto: Kontynuuj (System zapisów w budowie)");
        // W przysz³oœci dodasz tu logikê wczytywania najnowszego pliku zapisu
    }

    public void NewGame()
    {
        Debug.Log("Klikniêto: Nowa Gra! £adowanie sceny: " + startingSceneName);
        SceneManager.LoadScene(startingSceneName);
    }

    public void OpenSavesMenu()
    {
        Debug.Log("Klikniêto: Zapisy (Panel zapisów w budowie)");
        // W przysz³oœci uaktywnisz tu np. panel UI z list¹ slotów (saveSlotsPanel.SetActive(true))
    }

    public void OpenCollectiblesMenu()
    {
        Debug.Log("Klikniêto: ZnajdŸki (Galeria znajdziek w budowie)");
        // Podobnie, w przysz³oœci w³¹czysz tu odpowiedni panel UI
    }

    public void QuitGame()
    {
        Debug.Log("Klikniêto: WyjdŸ! Zamykanie aplikacji...");

        // Zamyka fizycznie zbudowan¹ grê (.exe, .apk itp.)
        Application.Quit();

        // Opcjonalnie: Poni¿szy kod zatrzymuje grê równie¿ w edytorze Unity
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}