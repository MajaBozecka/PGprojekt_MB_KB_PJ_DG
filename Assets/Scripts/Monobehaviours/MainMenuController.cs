using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Ustawienia Scen")]
    public string startingSceneName = "Intro";
    public string firstChapterSceneName = "Chapter1";
    [Header("ZnajdŸki")]
    public CollectiblesDatabaseSO collectiblesDatabase;

    // NOWA ZMIENNA: Referencja do panelu z galeri¹
    public GameObject collectiblesPanel;

    public void ContinueGame()
    {
        Debug.Log("Klikniêto: Kontynuuj. £adowanie sceny: " + firstChapterSceneName);
        SceneManager.LoadScene(firstChapterSceneName);
    }

    public void NewGame()
    {
        Debug.Log("Klikniêto: Nowa Gra! £adowanie sceny: " + startingSceneName);

        if (collectiblesDatabase != null)
        {
            collectiblesDatabase.ResetProgress();
        }

        SceneManager.LoadScene(startingSceneName);
    }

    public void OpenSavesMenu()
    {
        Debug.Log("Klikniêto: Zapisy (Panel zapisów w budowie)");
    }

    // ZAKTUALIZOWANA METODA
    public void OpenCollectiblesMenu()
    {
        Debug.Log("Otwieranie galerii znajdziek!");
        if (collectiblesPanel != null)
        {
            collectiblesPanel.SetActive(true); // W³¹czamy panel
        }
    }

    // NOWA METODA: Do zamykania galerii
    public void CloseCollectiblesMenu()
    {
        if (collectiblesPanel != null)
        {
            collectiblesPanel.SetActive(false); // Wy³¹czamy panel
        }
    }

    public void QuitGame()
    {
        Debug.Log("Klikniêto: WyjdŸ! Zamykanie aplikacji...");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}