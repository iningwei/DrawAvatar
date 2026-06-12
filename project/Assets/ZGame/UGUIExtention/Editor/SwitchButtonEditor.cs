using UnityEditor;
using UnityEditor.UI;
using ZGame.UGUIExtention;

[CustomEditor(typeof(SwitchButton), true)]
[CanEditMultipleObjects]
public class SwitchButtonEditor : ButtonEditor
{
    SerializedProperty switchTargets;
    SerializedProperty curIndex;

    protected override void OnEnable()
    {
        base.OnEnable();
        switchTargets = serializedObject.FindProperty("switchTargets");
        curIndex = serializedObject.FindProperty("curIndex");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        EditorGUILayout.PropertyField(switchTargets);
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(curIndex);

        serializedObject.ApplyModifiedProperties();
    }
}
