using System.Collections.Generic;
using UnityEngine;
using System.IO;

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
    List<string> speakersList = new();
    public History history = new();
    public string sequencePack;
    public bool skipping;
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    #region DialogueSequenceControl
    public HashSet<DialogueSequence> dialogueSequenceHashSet = new();//still dont know if hash or sorted
    [HideInInspector]
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

    }
    private string pathToSave { get { return Application.persistentDataPath + "/" + sequencePack + ".json"; } }
}