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
    private class SerializatorSpeaker
    {
        List<Speaker> speakerList;
        public SerializatorSpeaker(DataFlowSO data)
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
    #region SAVEDATA
    public History history = new();
    private class SerializatorSaveData
    {
        public History history = new();
        public List<PlotCheckpoint> listPlotCheckpoints = new ();
        public SerializatorSaveData(DataFlowSO data)
        {
            if (data is not null)
            {
                history = data.history;
                listPlotCheckpoints = data.listPlotCheckpoints;
            }
        }
        public SerializatorSaveData() { }
    }
    #endregion
    #region PLOTCHECKPOINTS
    public List<PlotCheckpoint> listPlotCheckpoints;
    public List<PlotCheckpoint> listPlotChekpointsEndChapter;
    private class SerializatorPlotCheckpoint
    {
        public List<PlotCheckpoint> listPlotCheckpoints;
        public List<PlotCheckpoint> listPlotChekpointsEndChapter;
        public SerializatorPlotCheckpoint(DataFlowSO data)
        {
            if (data is not null)
            {
                listPlotCheckpoints = data.listPlotCheckpoints;
                listPlotChekpointsEndChapter = data.listPlotChekpointsEndChapter;
            }
        }
    }
    public bool PlotCheckpointCheckForChapterEnd()
    {
        bool ret = true;
        foreach (PlotCheckpoint check in listPlotChekpointsEndChapter)
        {
            foreach (PlotCheckpoint plotCheckpointOfCollection in listPlotChekpointsEndChapter)
            {
                int indexOfMatchedCheckpointFromControl = listPlotCheckpoints.BinarySearch(plotCheckpointOfCollection);
                if (indexOfMatchedCheckpointFromControl < 0)
                    continue;
                if (listPlotCheckpoints[indexOfMatchedCheckpointFromControl].checkpointField != plotCheckpointOfCollection.checkpointField)
                {
                    ret = false;
                    break;
                }
            }
            if (!ret) break;
        }
        return ret;
    }

    #endregion
    public bool skipping;
    #region DEFAULTVALUES
    [Header("DefaultValues")]
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    public float defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
    [System.Serializable]
    private class SerializatorDefaultValues
    {
        public float defaultTimeTillTextSkippable;
        public float defaultTimeTillSubTextSkippable;
        public float defaultTimePerCharacterInCaseOfNoMatchWithSpeakerCollection;
        public SerializatorDefaultValues(DataFlowSO data)
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
    private class SerializatorDialogueSequenceCollection
    {
        public List<DialogueSequence> dialogueSequenceList = new();
        public SerializatorDialogueSequenceCollection(DataFlowSO data)
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
        if (serializedAnalisedSequence != null && serializedPlaceholderSequence != null)
        {
            serializedAnalisedSequence.CopyTo(serializedPlaceholderSequence);
        }

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
    private string pathToSerializedSaveData { get { return Application.persistentDataPath + "/" + "SaveData" + ".json"; } }

    public void SaveToJSON()
    {
        string json = JsonUtility.ToJson(new SerializatorSaveData(this));
        File.WriteAllText(pathToSerializedSaveData, json);
        Debug.Log("Zapisano stan gry do: " + pathToSerializedSaveData);
    }
    public void LoadFromJSON()
    {
        updateDialogueSequenceCollections();
        pushAnalisedToPlaceholder();
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (File.Exists(pathToSerializedSaveData))
        {
            string json = File.ReadAllText(pathToSerializedSaveData);
            SerializatorSaveData sr = new SerializatorSaveData();
            JsonUtility.FromJsonOverwrite(json, sr);

            this.history = sr.history;

            foreach (PlotCheckpoint item in sr.listPlotCheckpoints)
            {
                int i = this.listPlotCheckpoints.BinarySearch(item);
                if (i >= 0)
                {
                    this.listPlotCheckpoints[i] = item;
                }
                else
                {
                    this.listPlotCheckpoints.Add(item);
                    this.listPlotCheckpoints.Sort();
                }
            }
            Debug.Log("Wczytano stan gry.");
        }
        else
        {
            Debug.Log("Brak pliku zapisu, rozpoczynanie czystej gry.");
        }
    }
    #endregion
}