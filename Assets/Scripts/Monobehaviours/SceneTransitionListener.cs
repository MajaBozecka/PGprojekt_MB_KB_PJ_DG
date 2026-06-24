using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionListener : MonoBehaviour
{
    [Header("Ustawienia Przejœcia")]
    [Tooltip("Dok³adne ID dialogu (z DataFlowSO), po którym ma zmieniæ siê scena")]
    public string endingDialogueId;

    [Tooltip("Dok³adna nazwa pliku sceny (np. Chapter2)")]
    public string nextSceneName;

    private void OnEnable()
    {
        DataFlowController.OnSequenceEnded += CheckForSceneChange;
    }

    private void OnDisable()
    {
        DataFlowController.OnSequenceEnded -= CheckForSceneChange;
    }

    private void CheckForSceneChange(string endedDialogueId)
    {
        // Gra wypisze dok³adnie to, co us³ysza³a, w specjalnych cudzys³owach
        Debug.Log($"<color=yellow>[Szpieg]</color> Zakoñczono dialog: '{endedDialogueId}'. Czekam na: '{endingDialogueId}'");

        if (endedDialogueId == endingDialogueId)
        {
            Debug.Log($"<color=green>Zgodnoœæ potwierdzona! £adowanie nowej sceny: {nextSceneName}</color>");
            SceneManager.LoadScene(nextSceneName);
        }
    }
}