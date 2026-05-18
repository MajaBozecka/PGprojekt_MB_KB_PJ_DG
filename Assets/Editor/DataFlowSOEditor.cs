#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
/// <summary>
/// Custom Editor for DataFlowSODebugger to save sth in JSON file.
/// </summary>
[CustomEditor(typeof(DataFlowSO))]
public class DataFlowSOEditor : Editor
{
    DataFlowSO dataSO;
    SerializedProperty holdSeq;
    SerializedProperty holdSeqlId;
    static bool defaultInsp = false;
    static bool isOtherDataFoldout = false;
    static bool isDSCFoldout = true;
    bool showPlaceholderSeq;
    bool insertFailed;
    bool insertFailReasonDecideder;
    private void OnEnable()
    {
        dataSO = (DataFlowSO)target;
        insertFailed = false;
        showPlaceholderSeq = false;
    }
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (defaultInsp = EditorGUILayout.Foldout(defaultInsp, "DefaultInspector"))
        { DrawDefaultInspector(); }
        else
        {
            isOtherDataFoldout = EditorGUILayout.Foldout(isOtherDataFoldout, "OtherData");
            if (isOtherDataFoldout)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(serializedObject.FindProperty("speakersList"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("history"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("skipping"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTimeTillTextSkippable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTimeTillSubTextSkippable"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sequencePack"));
                EditorGUI.indentLevel--;
            }
            isDSCFoldout = EditorGUILayout.Foldout(isDSCFoldout, "DialogueSequenceAnalysis");
            if (isDSCFoldout)
            {
                holdSeq = serializedObject.FindProperty("serializedPlaceholderSequence");
                holdSeqlId = holdSeq.FindPropertyRelative("identifier");
                EditorGUI.BeginChangeCheck();
                EditorGUI.indentLevel++;
                DrawAnalisedSequence();
                EditorGUILayout.Space();
                DrawPlaceholderSequence();
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Saving/Loading with JSON");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("SaveToJson"))
                {
                    dataSO.SaveToJSON();
                }
                if (GUILayout.Button("LoadFromJson"))
                {
                    dataSO.LoadFromJSON();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Debugging buttons:");
                if (GUILayout.Button("Debug"))
                {
                    Debug.Log($"hash count:{dataSO.dialogueSequenceHashSet.Count}");
                    Debug.Log($"ser count:{dataSO.dialogueSequenceList.Count}");
                    for (int i = 0; i < dataSO.dialogueSequenceList.Count; i++)
                    {
                        Debug.Log($"ser{i} id:{dataSO.dialogueSequenceList[i].identifier}");
                        Debug.Log($"ser{i} runned:{dataSO.dialogueSequenceList[i].runnedAlready}");
                        Debug.Log($"ser{i} count:{dataSO.dialogueSequenceList[i].lines.Count}");
                    }
                }
                if (GUILayout.Button("cleanslate"))
                {
                    dataSO.dialogueSequenceList.Clear();
                    dataSO.updateDialogueSequenceCollections();
                    Debug.Log($"hash count:{dataSO.dialogueSequenceHashSet.Count}");
                    Debug.Log($"ser count:{dataSO.dialogueSequenceList.Count}");
                }
                if (EditorGUI.EndChangeCheck())
                {
                    //dataSO.OnAfterDeserialize();
                }
            }
        }
        if (serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();
    }

    private void DrawAnalisedSequence()
    {
        int match = dataSO.tryGetIndexOfAnalisedSequence;
        int popupReturnedId = EditorGUILayout.Popup("PickAnalysedSequence", match, dataSO.seqIdTab);
        if (dataSO.seqIdTab.Length == 0)
        {
            EditorGUILayout.HelpBox("There is no DialogueSequence in collection.\nPlease insert new sequences.", MessageType.Info);
        }
        else
        {
            if (popupReturnedId >= 0 & popupReturnedId < dataSO.seqIdTab.Length)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Analised sequence:");
                if (match != popupReturnedId)
                {
                    dataSO.analisedIndex = popupReturnedId;
                    dataSO.analisedIdentifier = dataSO.seqIdTab[popupReturnedId];
                    DialogueSequence tgasfi = dataSO.TryGetAnalisedSequenceFromIndex;
                    if (tgasfi is not null)
                    {
                        tgasfi.CopyTo(dataSO.serializedAnalisedSequence);
                    }
                }
                EditorGUI.indentLevel++;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Deleting a sequence moves copy to placeholder", MessageType.None);
                if (GUILayout.Button("Delete"))
                {
                    DeleteSequence();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("serializedAnalisedSequence"), true);
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.HelpBox("Please choose valid id, to show DialogueSequenceContent", MessageType.Info);
            }
        }
    }
    private void DrawPlaceholderSequence()
    {
        if (!showPlaceholderSeq)
        {
            if (GUILayout.Button("Add"))
            {
                showPlaceholderSeq = true;
            }
        }
        else
        {
            showPlaceholderSeq = EditorGUILayout.Foldout(showPlaceholderSeq, "Placeholder");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(holdSeq, true);
            if (insertFailed)
            {
                EditorGUILayout.HelpBox(indexErrorMsg, MessageType.Warning);
            }
            holdSeqlId.stringValue = EditorGUILayout.DelayedTextField("New Identifier:", holdSeqlId.stringValue);
            dataSO.serializedPlaceholderSequence.identifier = holdSeqlId.stringValue;
            if (GUILayout.Button("Insert"))
            {
                InsertSequence();
            }
            EditorGUI.indentLevel--;
        }
    }
    private void DeleteSequence()
    {
        if (dataSO.serializedAnalisedSequence is not null)
        {
            DialogueSequence tried = dataSO.TryGetAnalisedSequenceFromIndex;
            if (tried is not null)
            {
                tried.CopyTo(dataSO.serializedAnalisedSequence);
                if (dataSO.dialogueSequenceList.Remove(tried))
                {
                    dataSO.updateDialogueSequenceCollections();
                    dataSO.pushAnalisedToPlaceholder();
                    showPlaceholderSeq = true;
                    serializedObject.Update();
                }
            }
        }
    }
    private void InsertSequence()
    {
        dataSO.serializedPlaceholderSequence.identifier = holdSeq.FindPropertyRelative("identifier").stringValue;
        insertFailReasonDecideder = dataSO.dialogueSequenceList.BinarySearch(dataSO.serializedPlaceholderSequence) >= 0;
        dataSO.placeholdIdentifier = dataSO.serializedPlaceholderSequence.identifier;
        insertFailed = insertFailReasonDecideder | string.IsNullOrEmpty(dataSO.placeholdIdentifier);
        if (insertFailed)
        {
            Debug.LogWarning(indexErrorMsg);
        }
        else
        {
            Debug.Log($"Inserting new sequence; Count before: {dataSO.dialogueSequenceList.Count}");
            dataSO.dialogueSequenceList.Add(dataSO.serializedPlaceholderSequence);
            dataSO.dialogueSequenceList.Sort();
            dataSO.updateDialogueSequenceCollections();
            dataSO.pushPlaceholderToAnalised();
            showPlaceholderSeq = true;
            Debug.Log($"Inserted new sequence; Count after: {dataSO.dialogueSequenceHashSet.Count}");
            serializedObject.Update();
        }
    }
    private string indexErrorMsg { get { return insertFailReasonDecideder ? $"Cannot add existing index({dataSO.serializedPlaceholderSequence.identifier})!" : $"Index in placeholder is empty!"; } }

}

#endif