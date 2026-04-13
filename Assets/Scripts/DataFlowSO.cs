using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
{
    public DialogueSequence[] seq = new DialogueSequence[2];
    [SerializeField]
    string[] speakers = new string[2];
    public bool skipping;
    public float defaultTimeTillTextSkippable;
    public float defaultTimeTillSubTextSkippable;
    public string speaker(int i) { return (i >= 0 & i < speakers.Length) ? speakers[i] : "###"; }
}