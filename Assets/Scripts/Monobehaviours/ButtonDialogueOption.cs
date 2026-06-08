using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonDialogueOption : MonoBehaviour
{
    [SerializeField]
    TMP_Text dispalyedText;
    [SerializeField]
    GameObject readIcon;
    [SerializeField]
    Button selfButton;
    public DialogueOptionData dialogueOptionData;
    public Action<DialogueOptionData> onClick;
    public Button Button { get { return selfButton; } }
    private void Start()
    {
        Button.onClick.AddListener(OnClicked);
        setRead(false);
    }
    public void setText(string s)
    {
        dispalyedText.text = s;
    }
    public void setRead(bool read)
    {
        if(read)
        {
            dialogueOptionData.read = true;
        }
        readIcon.SetActive(dialogueOptionData.read);
    }
    private void OnClicked()
    {
        if(onClick is not null)
            onClick.Invoke(dialogueOptionData);
        else
        {
            Debug.Log("Button '"+name+"' has no assigned action for OnClick");
        }
    }
}
