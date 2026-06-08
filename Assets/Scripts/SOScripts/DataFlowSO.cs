using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
{
    #region SINGLETON
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
    #region SPEAKER
    [Header("GlobalData")]
    [SerializeField]
    List<Speaker> speakerList = new();
    private string[] m_speakerIdArray = new string[0];
    [System.Serializable]
    private class SerializatorSpeaker : SerializatorAbstract
    {
        List<Speaker> speakerList;
        public SerializatorSpeaker(DataFlowSO data) : base(data)
        {
            if (data is not null)
            {
                speakerList = data.speakerList;
            }
        }
    }
    public int getSpeakerId(string s)
    {
        for (int i = 0; i < speakerList.Count; i++)
        {
            if (speakerList[i].CompareTo(s) == 0)
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
            return sp.CompareTo(s) == 0;
        }
        return speakerList.Find(match);
    }
    public string[] speakersTab
    {
        get
        {
            if (m_speakerIdArray.Length != speakerList.Count)
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
    #endregion
    public History history = new();
    public List<PlotCheckpoint> listPlotCheckpoints;
    public bool skipping;
    #region DEFAULTVALUES
    [Header("DefaultValues")]
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    public float defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
    [System.Serializable]
    private class SerializatorDefaultValues : SerializatorAbstract
    {
        public float defaultTimeTillTextSkippable;
        public float defaultTimeTillSubTextSkippable;
        public float defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
        public SerializatorDefaultValues(DataFlowSO data) : base(data)
        {
            if(data is not null)
            {
                defaultTimeTillTextSkippable = data.defaultTimeTillTextSkippable;
                defaultTimeTillSubTextSkippable = data.defaultTimeTillSubTextSkippable;
                defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection = data.defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
            }
        }
    }
    #endregion
    [Header("TBD")]
    public string backgroundImagePath = "FF_wejœcie";
    #region DIALOGUESEQUENCECONTROL
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
    [System.Serializable]
    private class SerializatorDialogueSequenceCollection : SerializatorAbstract
    {
        public List<DialogueSequence> dialogueSequenceList = new();
        public SerializatorDialogueSequenceCollection(DataFlowSO data) : base(data)
        {
            if (data is not null)
            {
                dialogueSequenceList = data.dialogueSequenceList;
            }
        }

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
            if (dialogueSequenceList != null & (analisedIndex >= 0) & (analisedIndex < dialogueSequenceList.Count))
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

    public DialogueSequence getDialogueSequence(string id)
    {
        DialogueSequence ret = null;
        dialogueSequenceHashSet.TryGetValue(new DialogueSequence(id), out ret);
        return ret;
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
        if (dialogueSequenceList.Count > 0)
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
    #endregion

    #region JSONSAVING&LOADING
    [Header("Saving/Loading with JSON")]
    public string chapter;
    private string pathToSerializedDefaultValues { get { return Application.persistentDataPath + "/" + "DefaultValues" + ".json"; } }
    private string pathToSerializedSpeakers { get { return Application.persistentDataPath + "/Chapter"+ chapter + "/Speakers.json"; } }
    private string pathToSerializedDialogueSequence { get { return Application.persistentDataPath + "/Chapter"+ chapter + "/Sequences.json"; } }
    public void SaveToJSON()
    {
        if(Application.isEditor)
        {
            if(Application.isPlaying)
            {
                Debug.Log("Cannot make manual save while application is running. Serialized List is empty.");
            }
            else
            {
                string json;
                //defaults
                Debug.Log(pathToSerializedDefaultValues);
                json = JsonUtility.ToJson(new SerializatorDefaultValues(this));
                File.WriteAllText(pathToSerializedDefaultValues, json);
                //Speaker
                Debug.Log(pathToSerializedSpeakers);
                json = JsonUtility.ToJson(new SerializatorSpeaker(this));
                File.WriteAllText(pathToSerializedSpeakers, json);
                //Dialogues
                Debug.Log(pathToSerializedDialogueSequence);
                json = JsonUtility.ToJson(new SerializatorDialogueSequenceCollection(this));
                File.WriteAllText(pathToSerializedDialogueSequence, json);
            }
        }
    }
    public void LoadFromJSON()
    {
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (File.Exists(pathToSerializedDefaultValues))
        {
            string json = File.ReadAllText(pathToSerializedDefaultValues);
            JsonUtility.FromJsonOverwrite(json, this);
            updateDialogueSequenceCollections();
        }
        else
        {
            Debug.Log($"File: '{pathToSerializedDefaultValues}' missing.");
        }
        if (File.Exists(pathToSerializedSpeakers))
        {
            string json = File.ReadAllText(pathToSerializedSpeakers);
            JsonUtility.FromJsonOverwrite(json, this);
            updateDialogueSequenceCollections();
        }
        else
        {
            Debug.Log($"File: '{pathToSerializedSpeakers}' missing.");
        }
        if (File.Exists(pathToSerializedDialogueSequence))
        {
            string json = File.ReadAllText(pathToSerializedDialogueSequence);
            JsonUtility.FromJsonOverwrite(json, this);
            updateDialogueSequenceCollections();
        }
        else
        {
            Debug.Log($"File: '{pathToSerializedDialogueSequence}' missing.");
        }
        pushAnalisedToPlaceholder();
    }
    #endregion
}