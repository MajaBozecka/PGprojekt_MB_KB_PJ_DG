using TMPro;
using UnityEngine;

public class PanelDialogueSequenceController : MonoBehaviour
{
    public GameObject proceedIcon;
    public GameObject panel
    {
        get
        {
            return gameObject;
        }
    }

    [SerializeField]
    private TMP_Text dialogueText;
    public string textToShowInDialogueField;
    public void fillDialogueField(int n)
    {
        if(n<0)
        {
            dialogueText.text = textToShowInDialogueField;
        }
        else
        {
            dialogueText.text = textToShowInDialogueField[..(n)];
        }
    }
    public void SpeakerCustomization(Speaker s)
    {
        dialogueText.fontSize = s.fontSize;
        dialogueText.fontStyle = s.styles;
        dialogueText.color = s.color;
    }
}
