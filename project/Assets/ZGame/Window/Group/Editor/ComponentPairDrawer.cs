using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[CustomPropertyDrawer(typeof(ComponentPair))]
public class ComponentPairDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float lineHeight = EditorGUIUtility.singleLineHeight + 2;

        // 第一行：Key（根据子类不同，显示不同名称更友好）
        Rect keyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty keyProp = property.FindPropertyRelative("target");
        EditorGUI.PropertyField(keyRect, keyProp, new GUIContent("Target"));

        // 第二行：Value Type
        Rect typeRect = new Rect(position.x, position.y + lineHeight, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty typeProp = property.FindPropertyRelative("valueType");
        EditorGUI.PropertyField(typeRect, typeProp, new GUIContent("Value Type"));

        // 第三行：根据类型显示对应 Value
        Rect valueRect = new Rect(position.x, position.y + lineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
        SerializedProperty valueProp = null;
        string valueLabel = "Value";

        switch ((ValueType)typeProp.enumValueIndex)
        {
            case ValueType.String: valueProp = property.FindPropertyRelative("stringValue"); valueLabel = "String"; break;
            case ValueType.Int: valueProp = property.FindPropertyRelative("intValue"); valueLabel = "Integer"; break;
            case ValueType.Float: valueProp = property.FindPropertyRelative("floatValue"); valueLabel = "Float"; break;
            case ValueType.Bool: valueProp = property.FindPropertyRelative("boolValue"); valueLabel = "Boolean"; break;
            case ValueType.Object: valueProp = property.FindPropertyRelative("objectValue"); valueLabel = "Object"; break;
        }

        if (valueProp != null)
            EditorGUI.PropertyField(valueRect, valueProp, new GUIContent(valueLabel));

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return (EditorGUIUtility.singleLineHeight + 2) * 3;
    }
}