using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
{
    public DialogueSequence[] seq = new DialogueSequence[2];
    [SerializeField]
    List<string> speakersList = new();
    public DialogueHistory history = new();
    [SerializeField]
    public HashSet<DialogueSequence> dialogueSequenceHashSet = new();
    public bool skipping;
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    public string speaker(int i) { return (i >= 0 & i < speakersList.Count) ? speakersList[i] : "###"; }
    public string speaker(string s)
    {
        return speakersList.Contains(s) ? s : "###";
    }
    public int getSpeakerId(string s)
    {
        bool FindMatch(string ls)
        {
            return ls == s;
        }
        return speakersList.FindIndex(FindMatch);
    }
    public string[] speakersTab
    {
        get
        {
            return speakersList.ToArray();
        }
    }
}