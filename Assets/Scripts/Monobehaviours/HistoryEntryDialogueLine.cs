using TMPro;
using UnityEngine;

public class HistoryEntryDialogueLine : MonoBehaviour
{
    [SerializeField] TMP_Text speaker;
    [SerializeField] TMP_Text line;
    //In the future it should also read fonts and such
    public void prepareSpeakerAndEmptyLine(string speakerName)
    {
        speaker.text = speakerName;
        line.text = "";
    }
    public void appendLine(string s)
    {
        line.text += s;
    }
}
