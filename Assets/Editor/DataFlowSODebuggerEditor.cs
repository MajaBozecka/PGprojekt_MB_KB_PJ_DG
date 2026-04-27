using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// Custom Editor for DataFlowSODebugger to save sth in JSON file.
/// </summary>
[CustomEditor(typeof(DataFlowSODebugger))]
public class DataFlowSODebuggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DataFlowSODebugger SODebugger = (DataFlowSODebugger)target;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("GetKeys"))
        {
            SODebugger.populateKeyStrings();
        }
        DialogueLinePropertyDrawer.dataSO = SODebugger.data;
        DrawDefaultInspector();
        bool FindMatch(string s)
        {
            return s == SODebugger.lastStoredIdentifier;
        }
        int popupid = SODebugger.keyStrings.FindIndex(FindMatch);
        int i = EditorGUILayout.Popup("PickAnalysedSequence", popupid, SODebugger.keyStrings.ToArray());
        if (i >= 0) SODebugger.lastStoredIdentifier = SODebugger.keyStrings[i];


        //C:/Users/User/AppData/LocalLow/DefaultCompany/../savefile.json
        if (GUILayout.Button("Save"))
        {
            //takeADump.track.saveToJson();
        }
    }
    private void OnValidate()
    {

    }
}

[CustomPropertyDrawer(typeof(DialogueSequence), true)]
public class DialogueSequencePropertyDrawer : PropertyDrawer
{
    private const float FOLDOUT_HEIGHT = 16f;
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
        float height = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            for (int i = 0; i < l_lines.arraySize; i++)
            {
                height += EditorGUI.GetPropertyHeight(l_lines.GetArrayElementAtIndex(i),true);
            }
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        setProperties(property, true);
        Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOUT_HEIGHT);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        float addY = FOLDOUT_HEIGHT;
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
    public static DataFlowSO dataSO;

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
                height += EditorGUI.GetPropertyHeight(l_sublines.GetArrayElementAtIndex(i),true);
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
        setProperties(property, true);
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
        float addY = FOLDOUT_HEIGHT;
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            for (int i = 0; i < l_sublines.arraySize; i++)
            {
                rect = new Rect(position.x, position.y + addY, position.width, EditorGUI.GetPropertyHeight(l_sublines.GetArrayElementAtIndex(i),true));
                addY += rect.height;
                EditorGUI.PropertyField(rect, l_sublines.GetArrayElementAtIndex(i), new GUIContent($"SubLine({i}):"), true);
            }
            EditorGUI.indentLevel--;
            rect = new Rect(position.x, position.y + addY, position.width, EditorGUI.GetPropertyHeight(s_speakerID));
            addY += rect.height;
            speakerPopup(rect);
        }
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
        i = EditorGUI.Popup(rect, "SpeakerID:", i, dataSO.speakersTab);
        if (i >= 0)
        {
            s_speakerID.stringValue = dataSO.speaker(i);
        }
    }
    private int speakerID()
    {
        return dataSO.getSpeakerId(dataSO.speaker(s_speakerID.stringValue));
    }
}
#endif