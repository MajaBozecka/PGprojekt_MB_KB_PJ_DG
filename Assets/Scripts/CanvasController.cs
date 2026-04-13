using System.Collections.Generic;
using UnityEngine;
public class CanvasController : MonoBehaviour
{
    [SerializeField]
    GameObject buttonPanel;
    [SerializeField]
    DialoguePanelController dialoguePanel;
    [SerializeField]
    DialogueHistoryController historyPanel;
    public List<DialogueButton> testButtons = new();
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
            buttonPanel.gameObject.SetActive(false);
            dialoguePanel.gameObject.SetActive(false);
            historyPanel.gameObject.SetActive(false);
            switch (m_UIMode)
            {
                case EUIMode.NOTHING:
                    {
                        break;
                    }
                case EUIMode.BUTTONS:
                    {
                        buttonPanel.gameObject.SetActive(true);
                        if (testButtons.Count > 0)
                            testButtons[0].Button.Select();
                        break;
                    }
                case EUIMode.DIALOGUE:
                    {
                        dialoguePanel.gameObject.SetActive(true);
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
        flushButtonsNotRead();
    }

    public void setDialogueText(string s)
    {
        dialoguePanel.textToShowInDialogueField = s;
    }
    public void showDialogueText(int n)
    {
        dialoguePanel.fillDialogueField(n);
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
        historyPanel.addNewEntry(dial,line);
    }

    public void flushButtonsNotRead()
    {
        foreach (DialogueButton butt in testButtons)
        {
            butt.setRead(false);
        }
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
                    if (testButtons.Count > 0)
                        testButtons[0].Button.Select();
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
        dialoguePanel.proceedIcon.SetActive(proceedable);
    }

}
public enum EUIMode : byte
{
    NOTHING,
    BUTTONS,
    DIALOGUE,
    HISTORY
}