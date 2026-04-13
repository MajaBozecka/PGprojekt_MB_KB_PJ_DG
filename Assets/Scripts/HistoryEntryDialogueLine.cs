using TMPro;
using UnityEngine;

public class HistoryEntryDialogueLine : MonoBehaviour
{
    public TMP_Text speaker;
    public TMP_Text line;
    public void appendLine(string s)
    {
        line.text += s;
    }
}
