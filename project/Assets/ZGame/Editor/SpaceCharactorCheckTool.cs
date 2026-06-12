using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 空格符检测
/// 对所有预制件检测其子节点名称 是否 包含空格符号
/// </summary>
public class SpaceCharactorCheckTool : Editor
{
    [MenuItem("工具/检测/检测节点是否包含空格符")]
    public static void DoSpaceCharactorCheck()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int count = prefabGuids.Length;
        for (int i = 0; i < count; i++)
        {
            string filePath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            if (EditorUtility.DisplayCancelableProgressBar("检测节点名称是否包含空格符", filePath, i * 1f / (count - 1)))
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
            var childs = obj.GetComponentsInChildren<Transform>();
            for (int j = 0; j < childs.Length; j++)
            {
                if (childs[j].name.StartsWith(" ") || childs[j].name.EndsWith(" "))
                {
                    Debug.LogError($"prefab:{filePath}中， 节点：" + childs[j].GetHierarchy() + ",  首/尾 包含空格符");
                }
                else
                {
                    if (childs[j].name.Trim().Contains(" "))
                    {
                        Debug.LogWarning($"prefab:{filePath}中， 节点：" + childs[j].GetHierarchy() + ", 内部包含空格符");
                    }
                }

            }
        }

        EditorUtility.ClearProgressBar();
        Debug.Log("检测完毕！");
    }
}
