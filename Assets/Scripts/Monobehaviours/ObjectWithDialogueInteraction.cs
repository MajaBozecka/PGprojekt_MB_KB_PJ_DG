using System.Collections.Generic;
using UnityEngine;

public class ObjectWithDialogueInteraction : MonoBehaviour
{
    public string dialogueSequenceId
    {
        get
        {
            if (validateIndex())
            {
                return listOfDialogueOptions[lastUsedOptionIndex].identifier;
            }
            else
                return "";
        }
    }

    public bool read
    {
        get
        {
            return listOfDialogueOptions[lastUsedOptionIndex].read;
        }
        set
        {
            listOfDialogueOptions[lastUsedOptionIndex].read = value;
        }
    }
    public List<DialogueOptionData> listOfDialogueOptions;
    private byte lastUsedOptionIndex;
    private bool validateIndex()
    {
        if(listOfDialogueOptions.Count > lastUsedOptionIndex)
        {
            bool test = true;
            List<PlotCheckpoint> listPlotCheckpointControl = DataFlowSO.get.listPlotCheckpoints;
            foreach (PlotCheckpoint plotCheckpointOfCollection in listOfDialogueOptions[lastUsedOptionIndex].requirements)
            {
                int indexOfMatchedCheckpointFromControl = listPlotCheckpointControl.BinarySearch(plotCheckpointOfCollection);
                if (indexOfMatchedCheckpointFromControl < 0)
                    continue;
                if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField != plotCheckpointOfCollection.checkpointField)
                {
                    test = false;
                    break;
                }
            }
            if (!test)
            {
                for (; lastUsedOptionIndex < listOfDialogueOptions.Count; lastUsedOptionIndex++)
                {
                    test = true;
                    foreach (PlotCheckpoint item in listOfDialogueOptions[lastUsedOptionIndex].requirements)
                    {
                        int temp_index = listPlotCheckpointControl.BinarySearch(item);
                        if (listPlotCheckpointControl[temp_index].checkpointField != item.checkpointField)
                        {
                            test = false;
                            break;
                        }
                    }
                    if (test)
                    {
                        return true;
                    }
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
