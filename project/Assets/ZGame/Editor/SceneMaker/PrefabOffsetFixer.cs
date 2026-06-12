using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PrefabOffsetFixer : EditorWindow
{
    private GameObject root;
    private GameObject targetChild;

    [MenuItem("ZGame/SceneMaker/Prefab Offset Fixer")]
    public static void ShowWindow()
    {
        GetWindow<PrefabOffsetFixer>("Prefab Offset Fixer");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("美术资源偏移修复工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        root = (GameObject)EditorGUILayout.ObjectField("Root 根节点", root, typeof(GameObject), true);
        targetChild = (GameObject)EditorGUILayout.ObjectField("Target 子节点", targetChild, typeof(GameObject), true);

        EditorGUILayout.Space();
        if (root == null || targetChild == null)
        {
            EditorGUILayout.HelpBox("请拖入 Root 和 Target 子节点", MessageType.Info);
            return;
        }

        if (!targetChild.transform.IsChildOf(root.transform))
        {
            EditorGUILayout.HelpBox("Target 必须是 Root 的子节点！", MessageType.Error);
            return;
        }

        // 计算偏移
        Vector3 rootWorldPos = root.transform.position;
        Vector3 targetWorldPos = targetChild.transform.position;
        Vector3 offset = targetWorldPos - rootWorldPos;

        EditorGUILayout.LabelField("当前偏移 (World Space):", EditorStyles.boldLabel);
        EditorGUILayout.Vector3Field("Offset", offset);

        EditorGUILayout.Space();

        if (GUILayout.Button("一键修复偏移", GUILayout.Height(40)))
        {
            FixOffset();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("操作会把 Target localPosition 归零，并同步移动其他子节点保持相对位置。\n推荐在 Prefab 编辑模式或 Scene 中操作后 Apply。", MessageType.Info);
    }

    private void FixOffset()
    {
        if (root == null || targetChild == null) return;

        Undo.RecordObject(root.transform, "Fix Prefab Offset");

        Vector3 rootWorldPos = root.transform.position;
        Vector3 targetWorldPos = targetChild.transform.position;
        Vector3 offset = targetWorldPos - rootWorldPos;

        if (offset == Vector3.zero)
        {
            Debug.Log("偏移已为0，无需修复");
            return;
        }

        // 记录所有子节点用于 Undo
        List<Transform> allChildren = new List<Transform>();
        GetAllChildren(root.transform, allChildren);

        foreach (Transform child in allChildren)
        {
            Undo.RecordObject(child, "Fix Child Offset");
        }

        
        // 1. 计算需要补偿的向量（世界空间）
        Vector3 correctionWorld = root.transform.position - targetChild.transform.position;

        // 2. 把 Target 归零
        targetChild.transform.localPosition = Vector3.zero;

        // 3. 对其他所有子节点进行局部空间补偿（更安全）
        foreach (Transform child in allChildren)
        {
            if (child != targetChild.transform)
            {
                // 转成 Root 局部空间的偏移，再应用
                Vector3 localCorrection = root.transform.InverseTransformVector(correctionWorld);
                child.localPosition += localCorrection;
            }
        }

        Debug.Log($"修复完成！原偏移: {offset}，已将 Target 归零并同步其他子节点。");
        EditorUtility.SetDirty(root);
    }

    private void GetAllChildren(Transform parent, List<Transform> list)
    {
        foreach (Transform child in parent)
        {
            list.Add(child);
            GetAllChildren(child, list);
        }
    }

    private void OnSelectionChange()
    {
        Repaint();
    }
}