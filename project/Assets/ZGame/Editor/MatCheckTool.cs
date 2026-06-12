using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class MatCheckTool
{


    [MenuItem("工具/查找/检测未用到的材质球")]
    public static void CheckUnusedMat()
    {
        string[] allMats = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        List<string> matList = new List<string>();
        matList.AddRange(allMats);

        string[] allPrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        List<string> prefabList = new List<string>();
        prefabList.AddRange(allPrefabs);

         

        for (int i = 0; i < matList.Count; i++)
        {

            bool flag = false;
            string matPath = AssetDatabase.GUIDToAssetPath(matList[i]);
            if (EditorUtility.DisplayCancelableProgressBar("Checking Materials", matPath, i / (float)matList.Count))
            {
                EditorUtility.ClearProgressBar();
                return;
            }




            for (int j = 0; j < prefabList.Count; j++)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(prefabList[j]);


                string content = File.ReadAllText(prefabPath);
                if (content.Contains(matList[i]))
                {
                    flag = true;
                    break;
                }
            }
            if (flag == false)
            {
                Debug.LogError($"{matPath} 无引用", AssetDatabase.LoadAssetAtPath<Material>(matPath));
            }
        }
        EditorUtility.ClearProgressBar();
        Debug.Log("检测完毕！");
    }



}
