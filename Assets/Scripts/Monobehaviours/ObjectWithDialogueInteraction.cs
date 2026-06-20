using System.Collections.Generic;
using UnityEngine;

public class ObjectWithDialogueInteraction : MonoBehaviour
{
    [Header("Czy po przeczytaniu wyœwietliæ opcje wyboru?")]
    public bool hasChoicesMenu;

    public List<DialogueOptionData> listOfDialogueOptions;
    private byte lastUsedOptionIndex;
    public bool read
    {
        get
        {
            return validateIndex() ? listOfDialogueOptions[lastUsedOptionIndex].read:false;
        }
        set
        {
            listOfDialogueOptions[lastUsedOptionIndex].read = value;
        }
    }
    public DialogueOptionData getDOD
    {
        get
        {
            return validateIndex()?listOfDialogueOptions[lastUsedOptionIndex]:null;
        }
    }
    private bool validateIndex()
    {
        if (listOfDialogueOptions.Count > lastUsedOptionIndex)
        {
            bool test;
            List<PlotCheckpoint> listPlotCheckpointControl = DataFlowSO.get.listPlotCheckpoints;
            for (; lastUsedOptionIndex < listOfDialogueOptions.Count; lastUsedOptionIndex++)
            {
                test = true;
                foreach (PlotCheckpoint plotCheckpointOfCollection in listOfDialogueOptions[lastUsedOptionIndex].requirements)
                {
                    int indexOfMatchedCheckpointFromControl = listPlotCheckpointControl.BinarySearch(plotCheckpointOfCollection);
                    if (indexOfMatchedCheckpointFromControl < 0)
                        continue;
                    if(plotCheckpointOfCollection.isAdditive)
                    {
                        if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField < plotCheckpointOfCollection.checkpointField)
                        {
                            test = false;
                            break;
                        }
                    }
                    else
                    {
                        if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField != plotCheckpointOfCollection.checkpointField)
                        {
                            test = false;
                            break;
                        }
                    }
                }
                if (test)
                {
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}
