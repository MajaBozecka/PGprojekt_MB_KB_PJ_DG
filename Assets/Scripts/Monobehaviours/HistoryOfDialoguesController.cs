using System.Collections.Generic;
using UnityEngine;

public class HistoryOfDialoguesController : MonoBehaviour
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
    Transform contentTransform;
    [SerializeField]
    Transform seqTransform;
    [SerializeField]
    List<Transform> seqTransformList = new();
    [SerializeField]
    DialogueSequence lastSeq;
    [SerializeField]
    HistoryEntryDialogueLine tempEntry;
    [SerializeField]
    string lastSpeakerId;
    /// <summary>
    /// This method is prepared in case I need to fill it up anew;
    /// IE when oppening new session and need to fill up empty page.
    /// </summary>
    /// <param name="dialSeq"></param>
    public void addNewEntryWholeSequence(DialogueSequence dialSeq)
    {
        seqTransform = Instantiate(sequencePrefab, contentTransform).transform;
        tempEntry = null;
        lastSpeakerId = "";
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
    public void addNewEntryPartialSeq(DialogueSequence seq, DialogueLine line)
    {
        if(lastSeq!=seq)
        {
            seqTransform = Instantiate(sequencePrefab, contentTransform).transform;
            tempEntry = null;
            lastSpeakerId = "";
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
    private HistoryEntryDialogueLine addNewLineEntry(DialogueLine dialogueLine)
    {
        HistoryEntryDialogueLine lineEntry = Instantiate(linePrefab.gameObject, seqTransform).GetComponent<HistoryEntryDialogueLine>();
        lineEntry.prepareSpeakerAndEmptyLine(data.speaker(dialogueLine.speakerID));
        return lineEntry;
    }

}
