using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueButton : MonoBehaviour
{
    [SerializeField]
    TMP_Text dispalyedText;
    [SerializeField]
    GameObject readIcon;
    [SerializeField]
    Button selfButton;
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
