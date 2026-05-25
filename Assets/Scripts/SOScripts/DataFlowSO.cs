using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Identifiers;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
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
    List<Speaker> speakerList = new();
    private string[] m_speakerIdArray = new string[0];
    public History history = new();
    public string sequencePack;
    public bool skipping;
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    public float defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
    public List<DialogueOptionData> dialogueOptionsIdentifiers;
    public string backgroundImagePath = "FF_wejœcie";
    #region DialogueSequenceControl
    public HashSet<DialogueSequence> dialogueSequenceHashSet = new();//still dont know if hash or sorted
    //[HideInInspector]
    public List<DialogueSequence> dialogueSequenceList = new();
    //[HideInInspector]
    public DialogueSequence serializedAnalisedSequence;
    //[HideInInspector]
    public DialogueSequence serializedPlaceholderSequence;
    [HideInInspector]
    public string[] seqIdTab;
    [HideInInspector]
    public string analisedIdentifier;
    [HideInInspector]
    public int analisedIndex;
    [HideInInspector]
    public string placeholdIdentifier;
    #endregion

    public int getSpeakerId(string s)
    {
        for (int i = 0; i < speakerList.Count; i++)
        {
            if (speakerList[i].CompareTo(s)==0)
                return i;
        }
        return -1;
    }
    public Speaker getSpeaker(int i)
    {
        if (0 <= i & i < speakerList.Count)
            return speakerList[i];
        return null;
    }
    public Speaker getSpeaker(string s)
    {
        bool match(Speaker sp)
        {
            return sp.CompareTo(s)==0;
        }
        return speakerList.Find(match);
    }
    public string[] speakersTab
    {
        get
        {
            if(m_speakerIdArray.Length!=speakerList.Count)
            {
                m_speakerIdArray = new string[speakerList.Count];
            }
            for (int i = 0; i < m_speakerIdArray.Length; i++)
            {
                m_speakerIdArray[i] = speakerList[i].speakerId;
            }
            return m_speakerIdArray;
        }
    }

    public float getSpeakerUsusalYappingTime(DialogueLine line)
    {
        Speaker speak = getSpeaker(line.speakerID);
        return speak != null ? speak.timePerCharacterTalking : defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
    }
    public float getTimeForSingleCharDisplayCorrected(DialogueLine line, SubDialogueLine sub)
    {
        return sub.timeForSingleCharDisplay != 0 ? sub.timeForSingleCharDisplay : getSpeakerUsusalYappingTime(line);
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
            if(dialogueSequenceList!=null & (analisedIndex >= 0)&(analisedIndex<dialogueSequenceList.Count))
            {
                return dialogueSequenceList[analisedIndex];
            }
            return null;
        }
    }

    public void pushPlaceholderToAnalised()
    {
        serializedPlaceholderSequence.CopyTo(serializedAnalisedSequence);
        analisedIdentifier = placeholdIdentifier;
        analisedIndex = tryGetIndexOfAnalisedSequence;
        serializedPlaceholderSequence = null;
        placeholdIdentifier = "";
    }

    public void pushAnalisedToPlaceholder()
    {
        serializedAnalisedSequence.CopyTo(serializedPlaceholderSequence);
        placeholdIdentifier = analisedIdentifier;
        serializedAnalisedSequence = null;
        analisedIdentifier = "";
        analisedIndex = -1;
    }
    public void updateDialogueSequenceCollections()
    {
        dialogueSequenceHashSet.Clear();
        foreach (DialogueSequence item in dialogueSequenceList)
        {
            dialogueSequenceHashSet.Add(item);
        }
        if (seqIdTab.Length != dialogueSequenceList.Count)
            seqIdTab = new string[dialogueSequenceList.Count];
        for (int i = 0; i < dialogueSequenceList.Count; i++)
        {
            seqIdTab[i] = dialogueSequenceList[i].identifier;
        }
    }

    public void easeUpMemoryByFreeingListCollection()
    {
        if(dialogueSequenceList.Count > 0)
        {
            updateDialogueSequenceCollections();
            dialogueSequenceList.Clear();
            seqIdTab = new string[0];
            //idk what else? GarbageCollector?
        }
    }
    public void rebuildDialogueSequenceList()
    {
        dialogueSequenceList.Clear();
        foreach (DialogueSequence item in dialogueSequenceHashSet)
        {
            dialogueSequenceList.Add(item);
        }
        dialogueSequenceList.Sort();
    }

    public void SaveToJSON()
    {
        Debug.Log(pathToSave);
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(pathToSave, json);
    }
    public void LoadFromJSON()
    {
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (File.Exists(pathToSave))
        {
            string json = File.ReadAllText(pathToSave);
            JsonUtility.FromJsonOverwrite(json, this);
            updateDialogueSequenceCollections();
        }
        else
        {
            Debug.Log($"File: '{pathToSave}' missing.");
        }
        pushAnalisedToPlaceholder();
    }
    private string pathToSave { get { return Application.persistentDataPath + "/" + sequencePack + ".json"; } }
}