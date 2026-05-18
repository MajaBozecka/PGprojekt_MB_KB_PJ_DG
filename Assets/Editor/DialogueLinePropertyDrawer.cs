#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

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