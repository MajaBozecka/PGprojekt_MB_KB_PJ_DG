using System;
using System.Collections.Generic;
using UnityEngine;
public class CanvasController : MonoBehaviour
{
    [SerializeField]
    PanelDialogueSequenceController dialogueSequencePanel;
    [SerializeField]
    PanelDialogueOptions dialogueOptionsPanel;
    [SerializeField]
    PanelHistoryController historyPanel;
    [SerializeField]
    public GameObject explorationButton;


    [SerializeField]
    GameObject skippingIcon;
    [SerializeField]
    private EUIMode m_UIMode;
    private EUIMode lastUIMode;
    public EUIMode UIMode {
        get { return m_UIMode; } 
        set
        {
            if (value == m_UIMode) return;
            m_UIMode = value;
            dialogueOptionsPanel.gameObject.SetActive(false);
            dialogueSequencePanel.gameObject.SetActive(false);
            historyPanel.gameObject.SetActive(false);
            if (explorationButton != null) explorationButton.SetActive(false);
            switch (m_UIMode)
            {
                case EUIMode.NOTHING:
                    {
                        if (explorationButton != null) explorationButton.SetActive(true);
                        break;
                    }
                case EUIMode.BUTTONS:
                    {
                        dialogueOptionsPanel.gameObject.SetActive(true);
                        if (dialogueOptionsPanel.buttonList.Count > 0)
                            dialogueOptionsPanel.buttonList[0].Button.Select();
                        break;
                    }
                case EUIMode.DIALOGUE:
                    {
                        dialogueSequencePanel.gameObject.SetActive(true);
                        break;
                    }
                case EUIMode.HISTORY:
                    {
                        historyPanel.gameObject.SetActive(true);
                        break;
                    }
            }
        }
    }
    public bool isSkippingIconVisible
    {
        get
        {
            return skippingIcon.activeSelf;
        }
    }
    // Start is called once before the first execution of UpdateCkeckpointFields after the MonoBehaviour is created
    void Start()
    {
        dialogueOptionsPanel.gameObject.SetActive(false);
        dialogueSequencePanel.gameObject.SetActive(false);
        historyPanel.gameObject.SetActive(false);
        UIMode = EUIMode.NOTHING;
    }

    public void setDialogueSequence(string st, Speaker sp, string speakerMod)
    {
        dialogueSequencePanel.textToShowInDialogueField = st;
        dialogueSequencePanel.SpeakerCustomization(sp, speakerMod);
    }
    public void showDialogueText(int n)
    {
        dialogueSequencePanel.fillDialogueField(n);
    }
    public void lookUpHistory(bool history)
    {
        if(history & UIMode != EUIMode.HISTORY)
        {
            lastUIMode = UIMode;
            UIMode = EUIMode.HISTORY;
        }
        if (!history & UIMode == EUIMode.HISTORY)
        {
            UIMode = lastUIMode;
            lastUIMode = EUIMode.HISTORY;
        }
    }
    public void dialogueHistoryUpdate(DialogueSequence dial, DialogueLine line)
    {
        historyPanel.addNewEntryPartialSeq(dial,line);
    }
    public void dialogueHistoryRewrite()
    {
        historyPanel.RewriteHistory();
    }

    public void setSelect()
    {
        switch (m_UIMode)
        {
            case EUIMode.NOTHING:
                {
                    break;
                }
            case EUIMode.BUTTONS:
                {
                    if (!dialogueOptionsPanel.SelectFirst())
                        Debug.Log("Failed to select first dialogue option button");
                    break;
                }
            case EUIMode.DIALOGUE:
                { 
                    break;
                }
        }
    }
    public void setSkippingIconVisibility(bool skip)
    {
        skippingIcon.SetActive(skip);
    }

    public void setProceedIconVisibility(bool proceedable)
    {
        dialogueSequencePanel.proceedIcon.SetActive(proceedable);
    }

    public void SetDialogueOptionRead(DialogueOptionData DOD)
    {
        dialogueOptionsPanel.SetDialogueOptionRead(DOD);
    }

    public void setDialogueOptions(List<DialogueOptionData> dod, Action<DialogueOptionData> onClickHandler)
    {
        dialogueOptionsPanel.SetButtons(dod,onClickHandler);
    }
    public int updateDialogueOptions(List<PlotCheckpoint> listPlotCheckpointControl)
    {
        return dialogueOptionsPanel.UpdateDialogueOptionVisibilityBasedOncheckPointControl(listPlotCheckpointControl);
    }
}
public enum EUIMode : byte
{
    NOTHING,
    BUTTONS,
    DIALOGUE,
    HISTORY
}