using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;

public class MeshRendererCheckTool : Editor
{

    [MenuItem("工具/检测/使用了多个材质球的MeshRenderer")]
    public static void Export()
    {
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int count = prefabGuids.Length;
        for (int i = 0; i < count; i++)
        { 
            string filePath = AssetDatabase.GUIDToAssetPath(prefabGuids[i]);
            if (EditorUtility.DisplayCancelableProgressBar("检测使用了多个材质球的MeshRenderer", filePath, i * 1f / (count - 1)))
            {
                EditorUtility.ClearProgressBar();
                return;
            }


            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath(filePath, typeof(GameObject));
            var childs = obj.GetComponentsInChildren<Transform>();
            for (int j = 0; j < childs.Length; j++)
            {
                var mr = childs[j].GetComponent<MeshRenderer>();
                if (mr != null && mr.sharedMaterials.Length > 1)
                {
                    Debug.LogError($"prefab:{filePath}中， 节点：" + mr.gameObject.GetHierarchy() + ", 包含多个材质球");
                }
            }

        }

        //TODO:检测场景

        EditorUtility.ClearProgressBar();
        Debug.Log("检测完毕！");
    }
}
