using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;

public class SearchUnusedMaterials : EditorWindow
{
    [MenuItem("工具/查找/SearchUnusedMaterials")]
    static void DoSearch()
    {
        SearchUnusedMaterials window = (SearchUnusedMaterials)EditorWindow.GetWindow(typeof(SearchUnusedMaterials));
        window.Show();

    }


    /// <summary>
    /// 材质球信息
    /// key为guid，value为path
    /// </summary>
    static Dictionary<string, string> matMsgs = new Dictionary<string, string>();
    /// <summary>
    /// 预制件信息
    /// key为guid，value为内容
    /// </summary>
    static Dictionary<string, string> prefabMsgs = new Dictionary<string, string>();

    static List<string> resultMatGUIDs = new List<string>();



    //单位毫秒
    static long startTime;
    static long endTime;
    static Vector2 scrollPos = Vector2.zero;

    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Search", GUILayout.Width(100)))
        {
            startTime = TimeTool.GetNowStamp();
            gatherMatMsgs();
            gatherPrefabMsgs();
            search();
        }
        EditorGUILayout.EndHorizontal();

        showResult();

    }

    void gatherPrefabMsgs()
    {
        prefabMsgs.Clear();

        string[] allPrefabGUIDs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        int count = allPrefabGUIDs.Length;
        for (int i = 0; i < count; i++)
        {
            string guid = allPrefabGUIDs[i];
            string content = File.ReadAllText(AssetDatabase.GUIDToAssetPath(guid));
            prefabMsgs.Add(guid, content);
        }
        
    }
    void gatherMatMsgs()
    {
        matMsgs.Clear();

        string[] allMatGUIDs = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        int count = allMatGUIDs.Length;
        for (int i = 0; i < count; i++)
        {
            string guid = allMatGUIDs[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            matMsgs.Add(guid, path);
        }
        
    }

    void search()
    {
        resultMatGUIDs.Clear();
         
        int matCount = matMsgs.Count;
        int i = 0;
        foreach (KeyValuePair<string,string> item in matMsgs)
        {
            string matGUID = item.Key;
            string matPath = item.Value;

            bool flag = false;            
            if (EditorUtility.DisplayCancelableProgressBar("Checking Materials", matPath, i / (float)matCount))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;


            foreach (KeyValuePair<string,string> prefabMsg in prefabMsgs)
            {                
                string content = prefabMsg.Value;                
                if (content.Contains(matGUID))
                {
                    flag = true;
                    break;
                }
            }

            if (flag == false && resultMatGUIDs.Contains(matGUID) == false)
            {
                Debug.Log($"{matPath} have no refrence by prefab");
                resultMatGUIDs.Add(matGUID);
            }
        }
      

        EditorUtility.ClearProgressBar();

        endTime = TimeTool.GetNowStamp();
        Debug.Log($"search finished, usedTime(s):{(endTime - startTime) / 1000f}");
    }


    void showResult()
    {
        int resultMatCount = resultMatGUIDs.Count;
        if (resultMatCount > 0)
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();

            for (int i = 0; i < resultMatCount; i++)
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(resultMatGUIDs[i]));

                EditorGUILayout.ObjectField(mat, typeof(Object), true, GUILayout.Width(300));
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }
    }
}
