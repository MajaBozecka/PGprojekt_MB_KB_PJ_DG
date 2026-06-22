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
            if (f_lingering == null)
            {
                f_lingering = property.FindPropertyRelative("lingering");
            }
            if (s_speakerID == null)
            {
                s_speakerID = property.FindPropertyRelative("speakerID");
            }
        }
        else
        {
            l_sublines = null;
            f_lingering = null;
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
            height += speakerID() >= 0 ? 0 : 2*EditorGUIUtility.singleLineHeight;
            height += EditorGUI.GetPropertyHeight(s_speakerID);
            height += EditorGUI.GetPropertyHeight(f_lingering);
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, GetPropertyHeight(property, label)), color);
        setProperties(property, true);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(s_speakerID);
            rect = speakerPopup(rect);
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(l_sublines);
            EditorGUI.PropertyField(rect, l_sublines, true);
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(f_lingering);
            EditorGUI.PropertyField(rect, f_lingering);
            EditorGUI.indentLevel--;
        }
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
    private Rect speakerPopup(Rect rect)
    {
        int i = speakerID();
        if (i < 0)
        {
            EditorGUI.HelpBox(rect, $"Warning! Previous ID({s_speakerID.stringValue}) is not valid anymore!", MessageType.Warning);
            rect.y += EditorGUIUtility.singleLineHeight;
        }
        i = EditorGUI.Popup(rect, "SpeakerID:", i, DataFlowSO.get.speakersTab);
        if (i >= 0)
        {
            s_speakerID.stringValue = DataFlowSO.get.getSpeaker(i).speakerId;
        }
        return rect;
    }
    private int speakerID()
    {
        return DataFlowSO.get.getSpeakerId(s_speakerID.stringValue);
    }
}

#endif