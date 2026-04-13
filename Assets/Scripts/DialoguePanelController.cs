using TMPro;
using UnityEngine;

public class DialoguePanelController : MonoBehaviour
{
    public GameObject proceedIcon;
    public TMP_Text dialogueText;
    public GameObject panel
    {
        get
        {
            return gameObject;
        }
    }
}
