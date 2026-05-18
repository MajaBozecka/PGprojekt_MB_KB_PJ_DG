using TMPro;
using UnityEngine;
using UnityEngine.UI;

<<<<<<<< HEAD:Assets/Scripts/Monobehaviours/ButtonDialogueOption.cs
public class ButtonDialogueOption : MonoBehaviour
========
public class ButtonDialogueSequence : MonoBehaviour
>>>>>>>> ee53a3bd059a6d4c5b46541e1a2a0eec15eaf839:Assets/Scripts/Monobehaviours/ButtonDialogueSequence.cs
{
    [SerializeField]
    TMP_Text dispalyedText;
    [SerializeField]
    GameObject readIcon;
    [SerializeField]
    Button selfButton;
    public string dialogueSequenceId;
    public Button Button { get { return selfButton; } }
    /*private void Start()
    {
        dispalyedText = transform.Find("DisplayedText").GetComponent<TMP_Text>();
        readIcon = transform.Find("AlreadyReadIcon").gameObject;
    }*/
    public void setText(string s)
    {
        dispalyedText.text = s;
    }
    public void setRead(bool read)
    {
        readIcon.SetActive(read);
    }
}
