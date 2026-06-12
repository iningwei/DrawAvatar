using UnityEditor;
using UnityEditor.UI;
using ZGame.UGUIExtention;

[CustomEditor(typeof(Switch2Button), true)]
[CanEditMultipleObjects]
public class Switch2ButtonEditor : ButtonEditor
{
    SerializedProperty switchSprites;
    SerializedProperty curIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        switchSprites = serializedObject.FindProperty("switchSprites");
        curIndex = serializedObject.FindProperty("curIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(switchSprites);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(curIndex);

        serializedObject.ApplyModifiedProperties();
    }
}
