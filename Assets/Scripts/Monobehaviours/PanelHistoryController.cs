using System.Collections.Generic;
using UnityEngine;

public class PanelHistoryController : MonoBehaviour
{
    [Header("Prefabs and SO")]
    public DataFlowSO data;
    [SerializeField]
    GameObject sequencePrefab;
    [SerializeField]
    HistoryEntryDialogueLine linePrefab;
    [Header("Transform handles and seq")]
    [SerializeField]
    Transform contentTransform;
    [SerializeField]
    Transform lastSeqEntryTransform;
    [SerializeField]
    List<GameObject> sequenceEntryList = new();
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
        if(dialSeq == null)
        {
            Debug.Log("PanelHistoryController attempt to add whole sequence found NULL");
            return;
        }
        addNewSequenceEntry();
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
            addNewSequenceEntry();
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
    private void addNewSequenceEntry()
    {
        lastSeqEntryTransform = Instantiate(sequencePrefab, contentTransform).transform;
        sequenceEntryList.Add(lastSeqEntryTransform.gameObject);
        tempEntry = null;
        lastSpeakerId = "";
    }
    private HistoryEntryDialogueLine addNewLineEntry(DialogueLine dialogueLine)
    {
        HistoryEntryDialogueLine lineEntry = Instantiate(linePrefab.gameObject, lastSeqEntryTransform).GetComponent<HistoryEntryDialogueLine>();
        lineEntry.prepareSpeakerAndEmptyLine(data.speaker(dialogueLine.speakerID));
        return lineEntry;
    }

    public void RewriteHistory()
    {
        if(sequenceEntryList.Count>0)
        {
            foreach (GameObject entry in sequenceEntryList)
            {
                Destroy(entry);
            }
            foreach (string histId in data.history.dialogueSequenceIdentifiersList)
            {
                DialogueSequence tempSeq = new DialogueSequence(histId);
                data.dialogueSequenceHashSet.TryGetValue(tempSeq,out tempSeq);
                addNewEntryWholeSequence(tempSeq);
            }
        }
    }
}
