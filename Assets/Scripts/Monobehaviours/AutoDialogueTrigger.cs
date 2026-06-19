using UnityEngine;

public class AutoDialogueTrigger : MonoBehaviour
{
    public DataFlowController dataController;

    [Header("Skrypt z danymi dialogu (z tego samego obiektu)")]
    public ObjectWithDialogueInteraction dialogueData;

    private void OnEnable()
    {
        if (dataController != null && dialogueData != null && dialogueData.listOfDialogueOptions.Count > 0)
        {
            // Odpalamy automatycznie pierwsz¹ opcjê z listy!
            dataController.StartDialogueSequence(dialogueData.listOfDialogueOptions[0]);
        }
    }
}
