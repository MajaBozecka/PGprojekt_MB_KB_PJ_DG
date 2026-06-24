using NUnit.Framework.Internal;
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
            List<PlotCheckpoint> listPlotCheckpointControl = DataFlowSO.get.listPlotCheckpoints;
            for (lastUsedOptionIndex = 0; lastUsedOptionIndex < listOfDialogueOptions.Count; lastUsedOptionIndex++)
            {
                if(listOfDialogueOptions[lastUsedOptionIndex].requirements.Contains(DataFlowSO.get.pc_CheckpointForDialogueDepth))
                {
                    if (iterateThroughObjectListOfReq(listPlotCheckpointControl))
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
        bool iterateThroughObjectListOfReq(List<PlotCheckpoint> listPlotCheckpointControl)
        {
            foreach (PlotCheckpoint plotCheckpointOfCollection in listOfDialogueOptions[lastUsedOptionIndex].requirements)
            {
                int indexOfMatchedCheckpointFromControl = listPlotCheckpointControl.BinarySearch(plotCheckpointOfCollection);
                if (indexOfMatchedCheckpointFromControl < 0)
                    continue;
                if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].isAdditive)
                {
                    if (plotCheckpointOfCollection.isAdditive)
                    {
                        if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField < plotCheckpointOfCollection.checkpointField)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField >= plotCheckpointOfCollection.checkpointField)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField != plotCheckpointOfCollection.checkpointField)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
