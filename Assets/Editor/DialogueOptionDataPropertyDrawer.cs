#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DialogueOptionData), true)]
public class DialogueOptionDataPropertyDrawer : PropertyDrawer
{
    private SerializedProperty s_identifier;
    private SerializedProperty s_text;
    private SerializedProperty b_read;
    private void setProperties(SerializedProperty property, bool setup = true)
    {
        if (setup)
        {
            if (s_identifier == null)
            {
                s_identifier = property.FindPropertyRelative("identifier");
            }
            if (s_text == null)
            {
                s_text = property.FindPropertyRelative("buttonText");
            }
            if (b_read == null)
            {
                b_read = property.FindPropertyRelative("read");
            }
        }
        else
        {
            s_identifier = null;
            s_text = null;
            b_read = null;
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        setProperties(property, true);
        float height = EditorGUIUtility.singleLineHeight;
        if (property.isExpanded)
        {
            height += seqID() >= 0 ? 0 : 2 * EditorGUIUtility.singleLineHeight;
            height += 3 * EditorGUIUtility.singleLineHeight;
        }
        setProperties(property, false);
        return height;
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        Rect rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        setProperties(property, true);
        property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);
        if (property.isExpanded)
        {
            rect.y += rect.height;
            rect = seqIdPopup(rect);
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(s_text);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("buttonText"));
            rect.y += rect.height;
            rect.height = EditorGUI.GetPropertyHeight(b_read);
            EditorGUI.PropertyField(rect, property.FindPropertyRelative("read"));
        }
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
    private Rect seqIdPopup(Rect rect)
    {
        int i = seqID();
        if (i < 0)
        {
            rect.height += EditorGUIUtility.singleLineHeight;
            EditorGUI.HelpBox(rect, $"Warning! Previous ID({s_identifier.stringValue}) is not valid!\nCheck if proper List is loaded", MessageType.Warning);
            rect.y += 2 * EditorGUIUtility.singleLineHeight;
            rect.height = EditorGUIUtility.singleLineHeight;
        }
        i = EditorGUI.Popup(rect, "SequenceID:", i, DataFlowSO.get.seqIdTab);
        if (i >= 0)
        {
            s_identifier.stringValue = DataFlowSO.get.seqIdTab[i];
        }
        return rect;
    }
    private int seqID()
    {
        string s = s_identifier.stringValue;
        string[] strings = DataFlowSO.get.seqIdTab;
        for (int i = 0; i < strings.Length; i++)
        {
            if (strings[i] == s)
            {
                return i;
            }
        }
        return -1;
    }
}

#endif