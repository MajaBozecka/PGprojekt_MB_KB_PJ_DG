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
    [SerializeField]
    public HashSet<DialogueSequence> dialogueSequenceHashSet = new();//still dont know if hash or sorted
    //[Header("DialogueSequenceAnalysis")]
    //[HideInInspector]
    public DialogueSequence analisedSequence;
    //[HideInInspector]
    public DialogueSequence placeholderSequence;
    [HideInInspector]
    public List<DialogueSequence> serDialogueSequenceList;
    [HideInInspector]
    public List<string> seqIds;
    private string m_lastStoredIdentifier;
    private int m_lastStoredIdentifierIndex;
    public string analisedIdentifier
    {
        get
        {
            return m_lastStoredIdentifier;
        }
        set
        {
            m_lastStoredIdentifier = value;
        }
    }
    public int analisedIndex
    {
        get
        {
            return m_lastStoredIdentifierIndex;
        }
        set
        {
            m_lastStoredIdentifierIndex = value;
        }
    }
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

    public DialogueSequence getDialogueSequence(string id)
    {
        DialogueSequence ret = null;
        dialogueSequenceHashSet.TryGetValue(new DialogueSequence(id),out ret);
        return ret;
    }

    public List<string> GetListOfDialogueSequenceId()
    {
        List<string> ret = new();
        foreach (DialogueSequence seq in dialogueSequenceHashSet)
        {
            ret.Add(seq.identifier); 
        }
        return ret;
    }

    public int matchId
    {
        get
        {
            return seqIds.IndexOf(analisedIdentifier);
        }
    }
    public DialogueSequence GetAnalisedSequence
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

    public void OnBeforeSerialize()
    {
        if (serDialogueSequenceList == null)
            serDialogueSequenceList = new();
        serDialogueSequenceList.Clear();
        foreach (DialogueSequence item in dialogueSequenceHashSet)
        {
            serDialogueSequenceList.Add(item);
        }
        serDialogueSequenceList.Sort();
        seqIds.Clear();
        foreach (DialogueSequence item in serDialogueSequenceList)
        {
            seqIds.Add(item.identifier);
        }
    }

    public void OnAfterDeserialize()
    {
        if (dialogueSequenceHashSet == null)
            dialogueSequenceHashSet = new();
        if (serDialogueSequenceList != null)
        {
            dialogueSequenceHashSet.Clear();
            foreach (DialogueSequence item in serDialogueSequenceList)
            {
                dialogueSequenceHashSet.Add(item);
            }
            serDialogueSequenceList.Clear();
        }
    }
}