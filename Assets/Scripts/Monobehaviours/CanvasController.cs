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
            switch (m_UIMode)
            {
                case EUIMode.NOTHING:
                    {
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIMode = EUIMode.BUTTONS;
        dialogueOptionsPanel.flushButtonsNotRead();
    }

    public void setDialogueText(string s)
    {
        dialogueSequencePanel.textToShowInDialogueField = s;
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

    public void SetDialogueOptionRead(string identifier)
    {
        dialogueOptionsPanel.SetDialogueOptionRead(identifier);
    }
}
public enum EUIMode : byte
{
    NOTHING,
    BUTTONS,
    DIALOGUE,
    HISTORY
}