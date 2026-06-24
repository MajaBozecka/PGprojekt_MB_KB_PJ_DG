using UnityEngine;

public class ButtonDialogueTrigger : MonoBehaviour
{
    [Header("Zarz¹dca Dialogów")]
    public DataFlowController dataFlow;

    [Header("Pojemnik na tekst zakoñczenia")]
    public ObjectWithDialogueInteraction dialogueContainer;

    public void StartEndingSequence()
    {
        if (dataFlow != null && dialogueContainer != null)
        {
            dialogueContainer.ResetIndexManual();
            dataFlow.InteractWithObject(dialogueContainer);
        }
        else
        {
            Debug.LogWarning("B³¹d: Nie podpiêto DataFlowControllera lub pojemnika z dialogiem w Inspektorze!");
        }
    }
}
