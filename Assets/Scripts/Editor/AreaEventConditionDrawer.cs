using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AreaEventCondition))]
public class AreaEventConditionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        SerializedProperty conditionTypeProp = property.FindPropertyRelative("conditionType");

        Rect conditionTypeRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(conditionTypeRect, conditionTypeProp, GUIContent.none);

        if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.ResourceValue)
        {
            SerializedProperty targetTagProp = property.FindPropertyRelative("targetTag");
            SerializedProperty resourceTypeProp = property.FindPropertyRelative("resourceType");
            SerializedProperty comparisonProp = property.FindPropertyRelative("comparison");
            SerializedProperty refValueProp = property.FindPropertyRelative("refValue");

            Rect targetTagRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width / 3, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(targetTagRect, targetTagProp, GUIContent.none);

            Rect resourceTypeRect = new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + 2) * 2, position.width / 3, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(resourceTypeRect, resourceTypeProp, GUIContent.none);

            Rect comparisonRect = resourceTypeRect; comparisonRect.x += position.width / 3;
            EditorGUI.PropertyField(comparisonRect, comparisonProp, GUIContent.none);

            Rect refValueRect = comparisonRect; refValueRect.x += position.width / 3;
            EditorGUI.PropertyField(refValueRect, refValueProp, GUIContent.none);
        }
        else if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.DialogueSaid)
        {
            SerializedProperty dialogueKeyProp = property.FindPropertyRelative("dialogueKey");

            Rect dialogueKeyRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(dialogueKeyRect, dialogueKeyProp);
        }
        else if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.DialogueEvent)
        {
            SerializedProperty dialogueEventNameProp = property.FindPropertyRelative("dialogueEventName");

            Rect dialogueEventNameRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(dialogueEventNameRect, dialogueEventNameProp);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        SerializedProperty conditionTypeProp = property.FindPropertyRelative("conditionType");

        if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.ResourceValue)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 8;
        }
        else if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.DialogueSaid)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }
        else if ((AreaEventCondition.ConditionType)conditionTypeProp.enumValueIndex == AreaEventCondition.ConditionType.DialogueEvent)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 4;
        }

        return EditorGUIUtility.singleLineHeight + 2;
    }
}
