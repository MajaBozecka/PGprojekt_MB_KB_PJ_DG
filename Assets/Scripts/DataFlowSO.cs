using UnityEngine;

[CreateAssetMenu(fileName = "DataFlowSO", menuName = "Scriptable Objects/DataFlowSO")]
public class DataFlowSO : ScriptableObject
{
    public string [] testStrings = { "aaa", "bbb", "ccc" };
    public bool skipping;
}
