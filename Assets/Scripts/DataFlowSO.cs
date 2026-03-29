using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
{
    public DialogueSequence[] seq = new DialogueSequence[2];
    public bool skipping;
    public float defaultTimeTillTextSkippable = 0.5f;
}