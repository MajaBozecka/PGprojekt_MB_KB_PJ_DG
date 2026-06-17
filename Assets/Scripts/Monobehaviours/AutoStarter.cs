using UnityEngine;
using System.Collections;

public class AutoStarter : MonoBehaviour
{
    [Tooltip("Przeci¹gnij tutaj obiekt DataFlowController ze sceny")]
    public DataFlowController dataFlow;

    [Tooltip("Skonfiguruj tutaj dane swojego intro (np. wpisz ID: Intro)")]
    public DialogueOptionData introSequence;

    void Start()
    {
        StartCoroutine(PlayIntroSequence());
    }

    IEnumerator PlayIntroSequence()
    {
        yield return new WaitForSeconds(0.5f);

        if (dataFlow != null && introSequence != null)
        {
            dataFlow.StartDialogueSequence(introSequence);
        }
        else
        {
            Debug.LogError("Brak podpiêtego DataFlowController lub pusty dialog w AutoStarter!");
        }
    }
}