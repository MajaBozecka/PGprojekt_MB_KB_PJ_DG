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
    public EUIMode UIMode;
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
        setUIMode(EUIMode.BUTTONS);
        flushButtonsNotRead();
    }
    public void setUIMode(EUIMode mode)
    {
        if (mode == UIMode) return;
        UIMode = mode;
        buttonPanel.gameObject.SetActive(false);
        dialoguePanel.gameObject.SetActive(false);
        historyPanel.gameObject.SetActive(false);
        switch (UIMode)
        {
            case EUIMode.NOTHING:
                {
                    break;
                }
            case EUIMode.BUTTONS:
                {
                    buttonPanel.gameObject.SetActive(true);
                    if(testButtons.Count >0 )
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
    public void setDialogueText(string s)
    {
        dialoguePanel.dialogueText.text = s;
    }

    public void dialogueHistoryUpdate(DialogueSequence dial)
    {
        historyPanel.addNewSequenceEntry(dial);
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
        switch (UIMode)
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