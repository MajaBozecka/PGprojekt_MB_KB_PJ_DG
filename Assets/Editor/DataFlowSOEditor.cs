using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Custom Editor for DataFlowSODebugger to save sth in JSON file.
/// </summary>
[CustomEditor(typeof(DataFlowSO))]
public class DataFlowSODebuggerEditor : Editor
{
    DataFlowSO dataSO;
    SerializedProperty analSeq;
    SerializedProperty holdSeq;
    bool isAnalisisFoldout;
    int analId;
    bool isPlaceholderFoldout;
    bool insertFailed;
    bool defaultInsp;
    private void OnEnable()
    {
        dataSO = (DataFlowSO)target;
        setProperties(true);
        insertFailed = false;
        isAnalisisFoldout = dataSO.analisedSequence != null;
        isPlaceholderFoldout = dataSO.placeholderSequence != null;
        analId = -1;
    }
    private void OnDisable()
    {
        setProperties(false);
    }
    private void setProperties(bool setup = true)
    {
        if (setup)
        {
            if (analSeq == null)
            {
                analSeq = serializedObject.FindProperty("analisedSequence");
            }
            if (holdSeq == null)
            {
                holdSeq = serializedObject.FindProperty("placeholderSequence");
            }
        }
        else
        {
            analSeq = null;
            holdSeq = null;
        }
    }
    public override void OnInspectorGUI()
    {
        if(defaultInsp = EditorGUILayout.Foldout(defaultInsp, "DefaultInspector"))
        { DrawDefaultInspector(); }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speakersList"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("history"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("skipping"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTimeTillTextSkippable"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultTimeTillSubTextSkippable"));
        isAnalisisFoldout = EditorGUILayout.Foldout(isAnalisisFoldout, "DialogueSequenceAnalysis");
        if (isAnalisisFoldout)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add"))
            {
                if (dataSO.placeholderSequence is null)
                    dataSO.placeholderSequence = new DialogueSequence();
            }
            if (GUILayout.Button("Delete"))
            {
                if (dataSO.analisedSequence is not null)
                {
                    dataSO.placeholderSequence = dataSO.analisedSequence;
                    dataSO.analisedSequence = null;
                    dataSO.analisedIdentifier = "";
                }
            }
            EditorGUILayout.EndHorizontal();
            AnalisedSequence();
            EditorGUILayout.Space();
            PlaceholderSequence();
            EditorGUI.indentLevel--;
        }
        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (GUILayout.Button("SaveToJson"))
        {
            Debug.Log($"hash count:{dataSO.dialogueSequenceHashSet.Count}");
            Debug.Log($"ser count:{dataSO.serDialogueSequenceList.Count}");
            Debug.Log($"anal null:{dataSO.analisedSequence == null}");
            Debug.Log($"place null:{dataSO.placeholderSequence == null}");
        }
    }

    private void AnalisedSequence()
    {
        analId = EditorGUILayout.Popup("PickAnalysedSequence", dataSO.matchId, dataSO.seqIds.ToArray());
        if (analId >= 0 & analId < dataSO.seqIds.Count)
        {
            dataSO.analisedIndex = analId;
            dataSO.analisedIdentifier = dataSO.seqIds[analId];
            dataSO.analisedSequence = dataSO.GetAnalisedSequence;
            EditorGUILayout.PropertyField(analSeq, new GUIContent("Analised Seq:"), true);
        }
    }
    private void PlaceholderSequence()
    {
        isPlaceholderFoldout = EditorGUILayout.Foldout(isPlaceholderFoldout, "Placeholder");
        if (isPlaceholderFoldout)
        {
            EditorGUILayout.PropertyField(holdSeq, new GUIContent("Proposed Seq:"), true);
            if (insertFailed)
            {
                EditorGUILayout.HelpBox("Failed to insert new Sequence. Repeating index.", MessageType.Warning);
            }
            if (GUILayout.Button("Insert"))
            {
                insertFailed = dataSO.serDialogueSequenceList.Contains(dataSO.placeholderSequence);
                if (insertFailed)
                {
                    Debug.LogWarning("Failed to insert new Sequence. Repeating index.");
                }
                else
                {
                    dataSO.serDialogueSequenceList.Add(dataSO.placeholderSequence);
                    Debug.LogWarning($"It should have added. ser:{dataSO.serDialogueSequenceList.Count}");
                }
            }
            EditorGUILayout.Space();
        }
    }
}

/*[CustomPropertyDrawer(typeof(DialogueSequence), true)]
public class DialogueSequencePropertyDrawer : PropertyDrawer
{
    private SerializedProperty l_lines;
    private SerializedProperty s_identifier;

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
            for (int i = 0; i < l_lines.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(l_lines.GetArrayElementAtIndex(i), true);
            }
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        setProperties(property, true);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        //property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, property.isExpanded, label);
        float addY = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < l_lines.arraySize; i++)
            {
                Rect rect = new Rect(position.x, position.y + addY, position.width, EditorGUI.GetPropertyHeight(l_lines.GetArrayElementAtIndex(i)));
                rect = EditorGUI.IndentedRect(rect);
                addY += rect.height;
                EditorGUI.PropertyField(rect, l_lines.GetArrayElementAtIndex(i), new GUIContent($"Line({i}):"), true);
            }
            EditorGUI.indentLevel--;
        }
        //EditorGUI.EndFoldoutHeaderGroup();
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
}


[CustomPropertyDrawer(typeof(DialogueLine), true)]
public class DialogueLinePropertyDrawer : PropertyDrawer
{
    private const float FOLDOUT_HEIGHT = 16f;
    private SerializedProperty l_sublines;
    private SerializedProperty f_lingering;
    private SerializedProperty s_speakerID;

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
        float height = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            for (int i = 0; i < l_sublines.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(l_sublines.GetArrayElementAtIndex(i), true);
            }
            height += speakerID() >= 0 ? 0 : FOLDOUT_HEIGHT;
            height += EditorGUI.GetPropertyHeight(s_speakerID);
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rect;
        Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);
        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label)), Color.black);
        setProperties(property, true);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        float addY = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < l_sublines.arraySize; i++)
            {
                rect = new Rect(position.x, position.y + addY, position.width, EditorGUI.GetPropertyHeight(l_sublines.GetArrayElementAtIndex(i), true));
                addY += rect.height;
                EditorGUI.PropertyField(rect, l_sublines.GetArrayElementAtIndex(i), new GUIContent($"SubLine({i}):"), true);
            }
            EditorGUI.indentLevel--;
            rect = new Rect(position.x, position.y + addY, position.width, EditorGUI.GetPropertyHeight(s_speakerID));
            addY += rect.height;
            speakerPopup(rect);
        }
        property.serializedObject.ApplyModifiedProperties();
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
}*/
#endif