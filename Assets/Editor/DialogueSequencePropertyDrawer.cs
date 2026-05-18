#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
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
        setProperties(property, false);
        EditorGUI.EndProperty();
    }
}

#endif