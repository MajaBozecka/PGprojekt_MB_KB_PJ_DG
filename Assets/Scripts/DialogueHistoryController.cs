using UnityEngine;
using static UnityEditor.Search.SearchColumn;

public class DialogueHistoryController : MonoBehaviour
{
    [Header("Prefabs and SO")]
    [SerializeField]
    public DataFlowSO data;
    [SerializeField]
    GameObject sequencePrefab;
    [SerializeField]
    HistoryEntryDialogueLine linePrefab;
    [Header("Transform handles and seq")]
    [SerializeField]
    Transform content;
    [SerializeField]
    Transform seqTransform;
    [SerializeField]
    DialogueSequence lastSeq;
    [SerializeField]
    HistoryEntryDialogueLine tempEntry;
    [SerializeField]
    int lastSpeakerId;
    public void addNewEntryWholeSequence(DialogueSequence dialSeq)
    {
        seqTransform = Instantiate(sequencePrefab, content).transform;
        tempEntry = null;
        lastSpeakerId = -1;
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
                tempEntry = addNewLineEntry(line);
                tempEntry.appendLine(line.dumpWholeLine());
            }
        }
    }
    private HistoryEntryDialogueLine addNewLineEntry(DialogueLine dialogueLine)
    {
        HistoryEntryDialogueLine lineEntry = Instantiate(linePrefab.gameObject, seqTransform).GetComponent<HistoryEntryDialogueLine>();
        lineEntry.prep(data.speaker(dialogueLine.speakerID));
        return lineEntry;
    }

    public void addNewEntry(DialogueSequence seq, DialogueLine line)
    {
        if(lastSeq!=seq)
        {
            seqTransform = Instantiate(sequencePrefab, content).transform;
            tempEntry = null;
            lastSpeakerId = -1;
            lastSeq = seq;
        }
        if (lastSpeakerId == line.speakerID)
        {
            if (tempEntry != null)
            {
                tempEntry.appendLine("\n");
                tempEntry.appendLine(line.dumpWholeLine());
            }
        }
        else
        {
            lastSpeakerId = line.speakerID;
            tempEntry = addNewLineEntry(line);
            tempEntry.appendLine(line.dumpWholeLine());
        }
    }
}
