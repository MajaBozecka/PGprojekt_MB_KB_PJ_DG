using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject, ISerializationCallbackReceiver
{
    #region singleton
    static DataFlowSO singleton;
    public DataFlowSO()
    {
        if (DataFlowSO.singleton != null)
            Destroy(this);
        else
        {
            singleton = this;
        }
    }
    ~DataFlowSO()
    {
        if (singleton == this)
            singleton = null;
    }
    public static DataFlowSO get { get { return singleton; } }
    #endregion
    [Header("GlobalData")]
    [SerializeField]
    List<string> speakersList = new();
    public DialogueHistory history = new();
    public bool skipping;
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    #region DialogueSequenceControl
    [SerializeField]
    public HashSet<DialogueSequence> dialogueSequenceHashSet = new();//still dont know if hash or sorted
    //[HideInInspector]
    public DialogueSequence serializedAnalisedSequence;
    //[HideInInspector]
    public DialogueSequence serializedPlaceholderSequence;
    [HideInInspector]
    public List<DialogueSequence> serDialogueSequenceList;
    [HideInInspector]
    public string[] seqIdTab;
    [HideInInspector]
    public string analisedIdentifier;
    [HideInInspector]
    public int analisedIndex;
    [HideInInspector]
    public string placeholdIdentifier;
    #endregion

    public string speaker(int i) { return (i >= 0 & i < speakersList.Count) ? speakersList[i] : "###"; }
    public string speaker(string s)
    {
        return speakersList.Contains(s) ? s : "###";
    }
    public int getSpeakerId(string s)
    {
        return speakersList.IndexOf(s);
    }
    public string[] speakersTab
    {
        get
        {
            return speakersList.ToArray();
        }
    }

    public float getSpeakerUsusalYappingTime(DialogueLine line)
    {
        //temp solution
        return 0.03f;
    }
    public float getTimeForSingleCharDisplayCorrected(DialogueLine line, SubDialogueLine sub)
    {
        return sub.timeForSingleCharDisplay >= 0 ? sub.timeForSingleCharDisplay : getSpeakerUsusalYappingTime(line);
    }

    public DialogueSequence getDialogueSequence(string id)
    {
        DialogueSequence ret = null;
        dialogueSequenceHashSet.TryGetValue(new DialogueSequence(id),out ret);
        return ret;
    }

    public int tryGetIndexOfAnalisedSequence
    {
        get
        {
            if (string.IsNullOrEmpty(analisedIdentifier)) return -1;
            for (int i = 0; i < seqIdTab.Length; i++)
            {
                if (seqIdTab[i] == analisedIdentifier) return i;
            }
            return -1;
        }
    }
    public DialogueSequence TryGetAnalisedSequenceFromIndex
    {
        get
        {
            if(serDialogueSequenceList!=null & (analisedIndex >= 0)&(analisedIndex<serDialogueSequenceList.Count))
            {
                return serDialogueSequenceList[analisedIndex];
            }
            return null;
        }
    }

    public void swapBufferedSequences(bool toPlaceholder)
    {
        if (toPlaceholder)
        {
            serializedAnalisedSequence.CopyTo(serializedPlaceholderSequence);
            placeholdIdentifier = analisedIdentifier;
            serializedAnalisedSequence = null;
            analisedIdentifier = "";
            analisedIndex = -1;
        }
        else
        {
            serializedPlaceholderSequence.CopyTo(serializedAnalisedSequence);
            analisedIdentifier = placeholdIdentifier;
            analisedIndex = tryGetIndexOfAnalisedSequence;
            serializedPlaceholderSequence = null;
            placeholdIdentifier = "";
        }
    }

    public void OnBeforeSerialize()
    {
        if (serDialogueSequenceList == null)
            serDialogueSequenceList = new();
        if (serDialogueSequenceList.Count != dialogueSequenceHashSet.Count)
        {
            serDialogueSequenceList.Clear();
            foreach (DialogueSequence item in dialogueSequenceHashSet)
            {
                serDialogueSequenceList.Add(item);
            }
            serDialogueSequenceList.Sort();
            fillSeqIdsTab();
        }
    }

    public void OnAfterDeserialize()
    {
        if (dialogueSequenceHashSet == null)
            dialogueSequenceHashSet = new();
        if (serDialogueSequenceList != null)
        {
            DialogueSequence analisedTarget = TryGetAnalisedSequenceFromIndex;
            if (analisedTarget != null & serializedAnalisedSequence != null)
            {
                if (analisedIdentifier == analisedTarget.identifier)
                    serializedAnalisedSequence.CopyTo(analisedTarget);
            }
            dialogueSequenceHashSet.Clear();
            foreach (DialogueSequence item in serDialogueSequenceList)
            {
                dialogueSequenceHashSet.Add(item);
            }
            serDialogueSequenceList.Clear();
        }
    }

    public void fillSeqIdsTab()
    {
        seqIdTab = new string[serDialogueSequenceList.Count];
        for (int i = 0; i < serDialogueSequenceList.Count; i++)
        {
            seqIdTab[i] = serDialogueSequenceList[i].identifier;
        }
    }
}