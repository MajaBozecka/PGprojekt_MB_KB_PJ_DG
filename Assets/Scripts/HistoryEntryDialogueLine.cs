using TMPro;
using UnityEngine;

public class HistoryEntryDialogueLine : MonoBehaviour
{
    [SerializeField] TMP_Text speaker;
    [SerializeField] TMP_Text line;

    public void prep(string speakerName)
    {
        speaker.text = speakerName;
        line.text = "";
    }
    public void appendLine(string s)
    {
        line.text += s;
    }
}
