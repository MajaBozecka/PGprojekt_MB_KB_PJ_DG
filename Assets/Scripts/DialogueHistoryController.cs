using UnityEngine;

public class DialogueHistoryController : MonoBehaviour
{
    [SerializeField]
    Transform content;
    [SerializeField]
    public DataFlowSO data;
    [SerializeField]
    GameObject sequencePrefab;
    [SerializeField]
    HistoryEntryDialogueLine linePrefab;
    public void addNewSequenceEntry(DialogueSequence dialSeq)
    {
        GameObject seq = Instantiate(sequencePrefab, content);
        HistoryEntryDialogueLine tempEntry = null;
        int lastSpeakerId = -1;
        foreach (DialogueLine line in dialSeq.lines)
        {
            if(lastSpeakerId == line.speakerID)
            {
                if (tempEntry != null)
                {
                    tempEntry.appendLine("\n");
                    tempEntry.appendLine(line.dumpWholeLine());
                }
            }else
            {
                lastSpeakerId = line.speakerID;
                tempEntry = addNewLineEntry(seq.transform, line);
            }
        }
    }
    private HistoryEntryDialogueLine addNewLineEntry(Transform seq, DialogueLine dialogueLine)
    {
        HistoryEntryDialogueLine lineEntry = Instantiate(linePrefab.gameObject, seq).GetComponent<HistoryEntryDialogueLine>();
        lineEntry.speaker.text = data.speaker(dialogueLine.speakerID);
        lineEntry.line.text = "";
        lineEntry.appendLine(dialogueLine.dumpWholeLine());
        return lineEntry;
    }
}
