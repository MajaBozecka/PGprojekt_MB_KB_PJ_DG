using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CanvasController : MonoBehaviour
{
    [SerializeField]
    Button testButton;
    [SerializeField]
    GameObject dialoguePanel;
    [SerializeField]
    TMP_Text dialogueText;
    public EUIMode UIMode;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        setUIMode(EUIMode.BUTTONS);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void setUIMode(EUIMode mode)
    {
        if (mode == UIMode) return;
        UIMode = mode;
        switch (UIMode)
        {
            case EUIMode.NOTHING:
                {
                    testButton.gameObject.SetActive(false);
                    dialoguePanel.gameObject.SetActive(false);
                    break;
                }
            case EUIMode.BUTTONS:
                {
                    testButton.gameObject.SetActive(true);
                    dialoguePanel.gameObject.SetActive(false);
                    break;
                }
            case EUIMode.DIALOGUE:
                {
                    testButton.gameObject.SetActive(false);
                    dialoguePanel.gameObject.SetActive(true);
                    break;
                }
        }

    }
    public void setDialogueText(string s)
    {
        dialogueText.text = s;
    }
}
public enum EUIMode : byte
{
    NOTHING,
    BUTTONS,
    DIALOGUE
}