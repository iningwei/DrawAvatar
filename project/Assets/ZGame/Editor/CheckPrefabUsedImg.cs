using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;

/// <summary>
/// 检测预制件使用到的图片资源
/// 直接引用的通过检测预制件的txt是否包含图片guid内容来确定
/// 间接引用，比如通过材质球，需要间接判断
/// --TODO：这里没有考虑到通过第三方脚本上绑定的资源
/// </summary>
public class CheckPrefabUsedImg : EditorWindow
{
    [MenuItem("Assets/ZGame/CheckUsedImg")]
    static void DoCheck()
    {
        CheckPrefabUsedImg window = (CheckPrefabUsedImg)EditorWindow.GetWindow(typeof(CheckPrefabUsedImg));
        window.Show();


        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        if (assets.Length > 0)
        {
            targetPrefab = assets[0];
            doCheck();
        }
    }


    static UnityEngine.Object targetPrefab;


    static string targetPrefabContent;

    /// <summary>
    /// 图片信息缓存，key为guid，value为path
    /// </summary>
    static Dictionary<string, string> imgMsgs = new Dictionary<string, string>();

    /// <summary>
    /// 材质球内容缓存，key为guid，value为内容
    /// </summary>
    static Dictionary<string, string> matContentCaches = new Dictionary<string, string>();



    static List<string> resultImgGUIDs = new List<string>();

    static Vector2 scrollPos = Vector2.zero;

    //单位毫秒
    static long startTime;
    static long endTime;
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();
        targetPrefab = EditorGUILayout.ObjectField(targetPrefab, typeof(UnityEngine.Object), true, GUILayout.Width(200));


        if (GUILayout.Button("Check", GUILayout.Width(100)))
        {
            doCheck();
        }
        EditorGUILayout.EndHorizontal();


        showResult();
    }

    static void doCheck()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("error,no targetPrefab selected");
            return;
        }
        startTime = TimeTool.GetNowStamp();
        string targetPrefabPath = AssetDatabase.GetAssetPath(targetPrefab);
        targetPrefabContent = File.ReadAllText(targetPrefabPath);

        gatherImgMsgs();
        gatherMatMsgs();
        check();

    }

    static void gatherMatMsgs()
    {
        matContentCaches.Clear();

        string[] allMatGUIDs = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });
        int matCount = allMatGUIDs.Length;
        for (int i = 0; i < matCount; i++)
        {
            string guid = allMatGUIDs[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            matContentCaches.Add(guid, File.ReadAllText(path));
        }
    }

    static void gatherImgMsgs()
    {
        imgMsgs.Clear();

        string[] allImgGUIDs = AssetDatabase.FindAssets("t:Texture", new[] { "Assets" });

        int imgCount = allImgGUIDs.Length;
        for (int i = 0; i < imgCount; i++)
        {
            string guid = allImgGUIDs[i];
            string path = AssetDatabase.GUIDToAssetPath(guid);
            imgMsgs.Add(guid, path);
        }

    }

    static void check()
    {
        resultImgGUIDs.Clear();

        int imgCount = imgMsgs.Count;
        int i = 0;
        foreach (KeyValuePair<string, string> item in imgMsgs)
        {
            string imgGUID = item.Key;
            string imgPath = item.Value;
            if (EditorUtility.DisplayCancelableProgressBar("Checking Imgs", imgPath, i / (float)imgCount))
            {
                EditorUtility.ClearProgressBar();
                return;
            }
            i++;

            if (targetPrefabContent.Contains(imgGUID) && resultImgGUIDs.Contains(imgGUID) == false)
            {
                resultImgGUIDs.Add(imgGUID);
            }

            foreach (KeyValuePair<string, string> matCache in matContentCaches)
            {
                if (matCache.Value.Contains(imgGUID) && resultImgGUIDs.Contains(imgGUID) == false)
                {
                    resultImgGUIDs.Add(imgGUID);
                }
            }

        }


        EditorUtility.ClearProgressBar();

        endTime = TimeTool.GetNowStamp();
        Debug.Log($"check finished, usedTime(s):{(endTime - startTime) / 1000f}");
    }


    static void showResult()
    {
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("result:");

        int resultImgCount = resultImgGUIDs.Count;
        if (resultImgCount > 0)
        {
            //BeginScrollView和BeginVertical的顺序没有讲究，都可先可后
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            EditorGUILayout.BeginVertical();
            for (int i = 0; i < resultImgCount; i++)
            {
                Texture tex = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(resultImgGUIDs[i]));

                EditorGUILayout.ObjectField(tex, typeof(UnityEngine.Object), true, GUILayout.Width(300));
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }


    }
}
