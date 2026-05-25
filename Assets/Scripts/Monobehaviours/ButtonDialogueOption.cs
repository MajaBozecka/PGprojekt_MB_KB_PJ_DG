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
    public string dialogueSequenceId;
    public Action<string> onClick;
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
        readIcon.SetActive(read);
    }
    private void OnClicked()
    {
        if(onClick is not null)
            onClick.Invoke(dialogueSequenceId);
        else
        {
            Debug.Log("Button '"+name+"' has no assigned action for OnClick");
        }
    }
}
