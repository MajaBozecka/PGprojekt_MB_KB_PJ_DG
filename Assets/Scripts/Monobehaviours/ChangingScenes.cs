using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangingScenes : MonoBehaviour
{
    public DataFlowSO data;
    public GameObject dialogueCanvas;

    [Header("Re¿yseria Przejœcia")]
    public SpriteRenderer endingSpriteRenderer;
    public float waitBeforeImage = 3f;
    public float fadeDuration = 1.5f;
    public float waitBeforeScene = 2f;
    public string nextSceneName = "Chapter1";

    [Header("Co ma wywo³aæ przejœcie?")]
    public string endingDialogueIdentifier = "Intro_finished";

    [Header("Efekty DŸwiêkowe Przejœcia")]
    public AudioSource transitionAudioSource;
    public AudioClip ambulanceSound;

    private void OnEnable()
    {
        DataFlowController.OnSequenceEnded += CheckForSceneEnd;
    }

    private void OnDisable()
    {
        DataFlowController.OnSequenceEnded -= CheckForSceneEnd;
    }

    private void CheckForSceneEnd(string finishedDialogueId)
    {
        Debug.Log($"<color=orange>[MANAGER PRZEJŒCIA]</color> Us³ysza³em sygna³! Zakoñczony dialog to: '{finishedDialogueId}'. Ja czekam na: '{endingDialogueIdentifier}'");

        if (finishedDialogueId == endingDialogueIdentifier)
        {
            Debug.Log("<color=green>[MANAGER PRZEJŒCIA]</color> Zgodnoœæ potwierdzona! Odpalam obrazek i zmianê sceny!");
            StartCoroutine(TransitionToNewSceneRoutine());
        }
        else
        {
            Debug.Log("<color=red>[MANAGER PRZEJŒCIA]</color> To nie jest dialog koñcz¹cy scenê. Ignorujê go.");
        }
    }

    private IEnumerator TransitionToNewSceneRoutine()
    {
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
        }

        yield return new WaitForSeconds(waitBeforeImage);

        if (transitionAudioSource != null && ambulanceSound != null)
        {
            transitionAudioSource.PlayOneShot(ambulanceSound);
        }

        yield return new WaitForSeconds(waitBeforeImage);
        yield return new WaitForSeconds(waitBeforeImage);

        if (endingSpriteRenderer != null)
        {
            Color spriteColor = endingSpriteRenderer.color;
            spriteColor.a = 0f;
            endingSpriteRenderer.color = spriteColor;
            endingSpriteRenderer.gameObject.SetActive(true);

            float elapsedTime = 0f;
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                spriteColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                endingSpriteRenderer.color = spriteColor;
                yield return null;
            }
        }

        yield return new WaitForSeconds(waitBeforeScene);
        SceneManager.LoadScene(nextSceneName);
    }
}
