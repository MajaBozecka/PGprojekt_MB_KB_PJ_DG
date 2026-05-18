#if UNITY_EDITOR
using System.IO;
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
    bool isAnalisisFoldout;
    bool forceAnalisedFoldout;
    bool isPlaceholderFoldout;
    bool holdingNewPlaceholderSeq;
    bool insertFailed;
    bool insertFailReasonDecideder;
    private void OnEnable()
    {
        dataSO = (DataFlowSO)target;
        setFoldouts();
        insertFailed = false;
        holdingNewPlaceholderSeq = false;
    }
    private void setFoldouts()
    {
        isAnalisisFoldout = dataSO.analisedIndex >= 0 | forceAnalisedFoldout;
        isPlaceholderFoldout = holdingNewPlaceholderSeq;
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
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("SaveToJson"))
                {
                    SaveToJSON();
                }
                if (GUILayout.Button("LoadFromJson"))
                {
                    LoadFromJSON();
                }
                EditorGUILayout.EndHorizontal();
/*                if (GUILayout.Button("Debug"))
                {
                    Debug.Log($"hash count:{dataSO.dialogueSequenceHashSet.Count}");
                    Debug.Log($"ser count:{dataSO.serDialogueSequenceList.Count}");
                    for (int i = 0; i < dataSO.serDialogueSequenceList.Count; i++)
                    {
                        Debug.Log($"ser{i} id:{dataSO.serDialogueSequenceList[i].identifier}");
                        Debug.Log($"ser{i} runned:{dataSO.serDialogueSequenceList[i].runnedAlready}");
                        Debug.Log($"ser{i} count:{dataSO.serDialogueSequenceList[i].lines.Count}");
                    }
                }
                if (GUILayout.Button("cleanslate"))
                {
                    dataSO.serDialogueSequenceList.Clear();
                    dataSO.fillSeqIdsTab();
                    dataSO.OnAfterDeserialize();
                    Debug.Log($"hash count:{dataSO.dialogueSequenceHashSet.Count}");
                    Debug.Log($"ser count:{dataSO.serDialogueSequenceList.Count}");
                }*/
                if (EditorGUI.EndChangeCheck())
                {
                    dataSO.OnAfterDeserialize();
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
            isAnalisisFoldout = EditorGUILayout.Foldout(isAnalisisFoldout, "Analised sequence:");
        }
        if (popupReturnedId >= 0 & popupReturnedId < dataSO.seqIdTab.Length)
        {
            if (match != popupReturnedId | forceAnalisedFoldout)
            {
                dataSO.analisedIndex = popupReturnedId;
                dataSO.analisedIdentifier = dataSO.seqIdTab[popupReturnedId];
                DialogueSequence tgasfi =dataSO.TryGetAnalisedSequenceFromIndex;
                if (tgasfi is not null)
                {
                    tgasfi.CopyTo(dataSO.serializedAnalisedSequence);
                    isAnalisisFoldout = true;
                }
                else
                {
                    isAnalisisFoldout = false;
                }
                forceAnalisedFoldout = false;
            }
            if (isAnalisisFoldout)
            {
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
        }
    }
    private void DrawPlaceholderSequence()
    {
        if (!isPlaceholderFoldout)
        {
            if (GUILayout.Button("Add"))
            {
                AddSequence();
            }
            EditorGUILayout.Space();
        }
        else
        {
            isPlaceholderFoldout = EditorGUILayout.Foldout(isPlaceholderFoldout, "Placeholder");
        }
        if (isPlaceholderFoldout)
        {
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
            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.Space();
    }
    private void AddSequence()
    {
        if (dataSO.serializedPlaceholderSequence is null)
        {
            dataSO.serializedPlaceholderSequence = new DialogueSequence();
        }
        holdingNewPlaceholderSeq = true;
        setFoldouts();
    }
    private void DeleteSequence()
    {
        if (dataSO.serializedAnalisedSequence is not null)
        {
            DialogueSequence tried = dataSO.TryGetAnalisedSequenceFromIndex;
            if (tried is not null)
            {
                tried.CopyTo(dataSO.serializedAnalisedSequence);
                if (dataSO.serDialogueSequenceList.Remove(tried))
                {
                    dataSO.fillSeqIdsTab();
                    dataSO.swapBufferedSequences(true);
                    dataSO.OnAfterDeserialize();
                    forceAnalisedFoldout = true;
                    holdingNewPlaceholderSeq = true;
                    setFoldouts();
                    serializedObject.Update();
                }
            }
        }
    }
    private void InsertSequence()
    {
        dataSO.serializedPlaceholderSequence.identifier = holdSeq.FindPropertyRelative("identifier").stringValue;
        insertFailReasonDecideder = dataSO.serDialogueSequenceList.BinarySearch(dataSO.serializedPlaceholderSequence) >= 0;
        dataSO.placeholdIdentifier = dataSO.serializedPlaceholderSequence.identifier;
        insertFailed = insertFailReasonDecideder | string.IsNullOrEmpty(dataSO.placeholdIdentifier);
        if (insertFailed)
        {
            Debug.LogWarning(indexErrorMsg);
        }
        else
        {
            Debug.Log($"Inserting new sequence; Count before: {dataSO.serDialogueSequenceList.Count}");
            dataSO.serDialogueSequenceList.Add(dataSO.serializedPlaceholderSequence);
            dataSO.serDialogueSequenceList.Sort();
            dataSO.fillSeqIdsTab();
            dataSO.swapBufferedSequences(false);
            dataSO.OnAfterDeserialize();
            holdingNewPlaceholderSeq = true;
            setFoldouts();
            Debug.Log($"Inserted new sequence; Count after: {dataSO.dialogueSequenceHashSet.Count}");
            serializedObject.Update();
        }
    }
    private string indexErrorMsg { get { return insertFailReasonDecideder ? $"Cannot add existing index({dataSO.serializedPlaceholderSequence.identifier})!" : $"Index in placeholder is empty!"; } }

    private void SaveToJSON()
    {
        Debug.Log(pathToSave);
        dataSO.OnBeforeSerialize();
        string json = JsonUtility.ToJson(dataSO);
        File.WriteAllText(pathToSave, json);
    }
    private void LoadFromJSON()
    {
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (File.Exists(pathToSave))
        {
            string json = File.ReadAllText(pathToSave);
            JsonUtility.FromJsonOverwrite(json, dataSO);
            dataSO.OnAfterDeserialize();
        }
        else
        {
            Debug.Log($"File: '{pathToSave}' missing.");
        }

    }
    private string pathToSave { get { return Application.persistentDataPath + "/" + dataSO.sequencePack + ".json"; } }
}

#endif