using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PanelDialogueOptions : MonoBehaviour
{
    public List<ButtonDialogueOption> buttonList = new();
    [SerializeField] private byte buttonsCount;
    [SerializeField] private GameObject buttonDialogueOptionPrefab;
    [SerializeField] private Transform ContentTransform;
    public void SetButtons(List<DialogueOptionData> dod, Action<DialogueOptionData> onClickHandler)
    {
        buttonsCount = (byte)dod.Count;
        for (int i = buttonList.Count; i < buttonsCount; i++)
        {
            ButtonDialogueOption nb = Instantiate(buttonDialogueOptionPrefab, ContentTransform).GetComponent<ButtonDialogueOption>();
            nb.onClick = onClickHandler;
            buttonList.Add(nb);
        }
        for (int i = 0; i < buttonsCount; i++)
        {
            ButtonDialogueOption BDO = buttonList[i];
            BDO.gameObject.SetActive(true);
            BDO.dialogueOptionData = dod[i];
            BDO.setText(dod[i].buttonText);
            BDO.setRead(false);
        }
        for (int i = buttonsCount; i < buttonList.Count; i++)
        {
            buttonList[i].gameObject.SetActive(false);
        }
    }
    public bool SelectFirst()
    {
        bool ret = buttonList.Count > 0;
        if (ret)
        {
            buttonList[0].Button.Select();
        }
        return ret;
    }
    public void SetDialogueOptionRead(DialogueOptionData DOD)
    {
        bool match(ButtonDialogueOption b)
        {
            return b.dialogueOptionData.identifier.Equals(DOD.identifier);
        }
        int ind = buttonList.FindIndex(match);
        if (ind >= 0)
        {
            buttonList[ind].setRead(true);
        }
    }
    public void UpdateDialogueOptionVisibilityBasedOncheckPointControl(List<PlotCheckpoint> listPlotCheckpointControl)
    {
        foreach (ButtonDialogueOption but in buttonList)
        {
            foreach (PlotCheckpoint plotCheckpointOfCollection in but.dialogueOptionData.requirements)
            {
                int indexOfMatchedCheckpointFromControl = listPlotCheckpointControl.BinarySearch(plotCheckpointOfCollection);
                if (indexOfMatchedCheckpointFromControl < 0)
                    continue;
                if (listPlotCheckpointControl[indexOfMatchedCheckpointFromControl].checkpointField != plotCheckpointOfCollection.checkpointField)
                {
                    but.gameObject.SetActive(false);
                    break;
                }
                else
                {
                    but.gameObject.SetActive(true);
                }
            }
        }
        /*bool test;
            List<PlotCheckpoint> listPlotCheckpointControl = DataFlowSO.get.listPlotCheckpoints;
            for (; lastUsedOptionIndex < listOfDialogueOptions.Count; lastUsedOptionIndex++)
            {
                test = true;
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
                if (test)
                {
                    return true;
                }
            }
            return false;*/
    }

}
