using TMPro;
using UnityEngine;

public class HistoryEntryDialogueLine : MonoBehaviour
{
    [SerializeField] TMP_Text speaker;
    [SerializeField] TMP_Text line;
    //In the future it should also read fonts and such
    public void prepareSpeakerAndEmptyLine(Speaker speaker)
    {
        this.speaker.text = speaker.speakerName;
        this.speaker.color = speaker.color;
        this.speaker.fontStyle = speaker.styles;
        this.speaker.fontSize = speaker.fontSize;
        line.text = "";
    }
    public void appendLine(string s)
    {
        line.text += s;
    }
}
