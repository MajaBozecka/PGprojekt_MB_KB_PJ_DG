using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Custom Editor for DataFlowSODebugger to save sth in JSON file.
/// </summary>
[CustomEditor(typeof(DataFlowSO))]
public class DataFlowSODebuggerEditor : Editor
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
        if(defaultInsp = EditorGUILayout.Foldout(defaultInsp, "DefaultInspector"))
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("serializedAnalisedSequence"));
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
                if (GUILayout.Button("SaveToJson"))
                {
                    SaveToJSON();
                }
                if (GUILayout.Button("Debug"))
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
                }
                if (EditorGUI.EndChangeCheck())
                {
                    dataSO.OnAfterDeserialize();
                }
            }
        }
        if(serializedObject.hasModifiedProperties)
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
                dataSO.TryGetAnalisedSequenceFromIndex.CopyTo(dataSO.serializedAnalisedSequence);
                isAnalisisFoldout = true;
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
            if(tried is not null)
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
        dataSO.serializedPlaceholderSequence.identifier=holdSeq.FindPropertyRelative("identifier").stringValue;
        insertFailReasonDecideder= dataSO.serDialogueSequenceList.BinarySearch(dataSO.serializedPlaceholderSequence) >= 0;
        dataSO.placeholdIdentifier=dataSO.serializedPlaceholderSequence.identifier;
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
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
    }
}

[CustomPropertyDrawer(typeof(DialogueSequence), true)]
public class DialogueSequencePropertyDrawer : PropertyDrawer
{
    private SerializedProperty l_lines;
    private SerializedProperty s_identifier;
    static Color drawerColor = Color.darkSlateGray;
    private void setProperties(SerializedProperty property, bool setup = true)
    {
        if (setup)
        {
            if (l_lines == null)
            {
                l_lines = property.FindPropertyRelative("lines");
            }
            if (s_identifier == null)
            {
                s_identifier = property.FindPropertyRelative("identifier");
            }
        }
        else
        {
            l_lines = null;
            s_identifier = null;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        setProperties(property, true);
        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(l_lines, true);
            height += EditorGUI.GetPropertyHeight(s_identifier, true);
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
       // property.serializedObject.Update();
        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label)), drawerColor);
        setProperties(property, true);
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            rect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUI.GetPropertyHeight(s_identifier));
            EditorGUI.LabelField(rect, "Identifier:", "\'" + s_identifier.stringValue + "\'");
            rect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUI.GetPropertyHeight(l_lines));
            EditorGUI.PropertyField(rect, l_lines, true);
            EditorGUI.indentLevel--;
        }
        //property.serializedObject.ApplyModifiedProperties();
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
}

[CustomPropertyDrawer(typeof(DialogueLine), true)]
public class DialogueLinePropertyDrawer : PropertyDrawer
{
    private SerializedProperty l_sublines;
    private SerializedProperty f_lingering;
    private SerializedProperty s_speakerID;
    static Color color = Color.black;
    private void setProperties(SerializedProperty property, bool setup = true)
    {
        if (setup)
        {
            if (l_sublines == null)
            {
                l_sublines = property.FindPropertyRelative("subLines");
            }
            if (s_speakerID == null)
            {
                s_speakerID = property.FindPropertyRelative("speakerID");
            }
        }
        else
        {
            l_sublines = null;
            s_speakerID = null;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        setProperties(property, true);
        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            height += EditorGUI.GetPropertyHeight(l_sublines);
            height += speakerID() >= 0 ? 0 : EditorGUIUtility.singleLineHeight;
            height += EditorGUI.GetPropertyHeight(s_speakerID);
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
       // property.serializedObject.Update();
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label)), color);
        setProperties(property, true);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            rect = new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, EditorGUI.GetPropertyHeight(l_sublines));
            EditorGUI.PropertyField(rect, l_sublines, true);
            EditorGUI.indentLevel--;
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(s_speakerID);
            speakerPopup(rect);
        }
       // property.serializedObject.ApplyModifiedProperties();
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
    private void speakerPopup(Rect rect)
    {
        int i = speakerID();
        if (i < 0)
        {
            EditorGUI.HelpBox(rect, $"Warning! Previous ID({s_speakerID.stringValue}) is not valid anymore!", MessageType.Warning);
            rect = new Rect(rect.x, rect.y + rect.height, rect.width, 16);
        }
        i = EditorGUI.Popup(rect, "SpeakerID:", i, DataFlowSO.get.speakersTab);
        if (i >= 0)
        {
            s_speakerID.stringValue = DataFlowSO.get.speaker(i);
        }
    }
    private int speakerID()
    {
        return DataFlowSO.get.getSpeakerId(DataFlowSO.get.speaker(s_speakerID.stringValue));
    }
}

#endif