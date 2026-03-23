using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class CanvasController : MonoBehaviour
{
    [SerializeField]
    GameObject buttonPanel;
    [SerializeField]
    GameObject dialoguePanel;
    public
    DialogueButton testButton0;
    public
    DialogueButton testButton1;
    [SerializeField]
    TMP_Text dialogueText;
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
        switch (UIMode)
        {
            case EUIMode.NOTHING:
                {
                    buttonPanel.gameObject.SetActive(false);
                    dialoguePanel.gameObject.SetActive(false);
                    break;
                }
            case EUIMode.BUTTONS:
                {
                    buttonPanel.gameObject.SetActive(true);
                    dialoguePanel.gameObject.SetActive(false);
                    testButton0.Button.Select();
                    break;
                }
            case EUIMode.DIALOGUE:
                {
                    buttonPanel.gameObject.SetActive(false);
                    dialoguePanel.gameObject.SetActive(true);
                    break;
                }
        }

    }
    public void setDialogueText(string s)
    {
        dialogueText.text = s;
    }

    public void flushButtonsNotRead()
    {
        testButton0.setRead(false);
        testButton1.setRead(false);
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
                    testButton0.Button.Select();
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

}
public enum EUIMode : byte
{
    NOTHING,
    BUTTONS,
    DIALOGUE
}