using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSODebugger", menuName = "Scriptable Objects/DataFlowSODebugger")]
public class DataFlowSODebugger : ScriptableObject
{
    public DataFlowSO data;
    public List<string> keyStrings;
    private string m_lastStoredIdentifier;
    public DialogueSequence testSeq;
    public string lastStoredIdentifier
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
    public void populateKeyStrings()
    {
        foreach (DialogueSequence item in data.dialogueSequenceHashSet)
        {
            keyStrings.Add(item.identifier);
        }
    }
}