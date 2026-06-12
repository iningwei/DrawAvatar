using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;


//对spine原始文件的处理(从其它项目拆解的资源)
//1，移除所有.meta文件
//2，.atlas文件修改为.atlas.txt
//3，.json文件中，若spine版本为："spine":"3.8.XX"，则统一修改为："spine":"3.8"
public class SpineAnimProcess : Editor
{
    [MenuItem("Assets/ZGame/CocosCreator资源处理/Spine文件批处理")]
    static void Processss()
    {
        string targetPath = Application.dataPath + "/_test";
        Debug.LogError("targetPath:" + targetPath);

        string[] subdirectoryEntries = Directory.GetDirectories(targetPath);
        for (int i = 0; i < subdirectoryEntries.Length; i++)
        {
            string path = subdirectoryEntries[i];
            //step 1:delete all meta files
            deleteMetaFile(path);

            //step 2:rename all .atlas file with .atlas.txt
            renameAtlasFile(path);

            //step 3:modify json file,change spine version
            modifySpineVersion(path);
        }
    }


    //bug， 文件夹右键经常选不中，导致空选择
    [MenuItem("Assets/ZGame/CocosCreator资源处理/bug exist-选择文件夹内所有Spine文件批处理")]
    static void ProcessSpine()
    {
        List<string> pathList;
        if (EditorUtils.GetCurSelectedFolderPath(out pathList))
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                string path = pathList[i];
                //step 1:delete all meta files
                deleteMetaFile(path);

                //step 2:rename all .atlas file with .atlas.txt
                renameAtlasFile(path);

                //step 3:modify json file,change spine version
                modifySpineVersion(path);
            }
        }
        Debug.Log("spine process finished");
    }

    [MenuItem("Assets/ZGame/CocosCreator资源处理/bug-exist-选择文件夹内移除所有meta文件")]
    static void DeleteMeta()
    {
        List<string> pathList;
        if (EditorUtils.GetCurSelectedFolderPath(out pathList))
        {
            for (int i = 0; i < pathList.Count; i++)
            {
                string path = pathList[i];

                deleteMetaFile(path);

            }
        }
        Debug.Log("delete meta finished");
    }


    private static void modifySpineVersion(string path)
    {
        var jsonFiles = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories);
        int jsonFileCount = jsonFiles.Length;
        for (int i = jsonFileCount - 1; i >= 0; i--)
        {
            string tmpPath = jsonFiles[i];
            string content = File.ReadAllText(tmpPath);
            var jsonObj = JObject.Parse(content);
            var spineVersion = jsonObj["skeleton"]["spine"];
            if (spineVersion.ToString().Contains("3.8."))
            {
                jsonObj["skeleton"]["spine"] = "3.8";
                Debug.Log("modifySpineVersion to 3.8:" + tmpPath);
            }
            File.WriteAllText(tmpPath, jsonObj.ToString());
        }
    }

    private static void renameAtlasFile(string path)
    {
        var atlasFiles = Directory.GetFiles(path, "*.atlas", SearchOption.AllDirectories);
        int atlasFileCount = atlasFiles.Length;
        for (int i = atlasFileCount - 1; i >= 0; i--)
        {
            string srcPath = atlasFiles[i];
            string desPath = srcPath + ".txt";
            File.Move(srcPath, desPath);
            Debug.Log("step 2:" + srcPath);
        }
    }


    private static void deleteMetaFile(string path)
    {
        var metaFiles = Directory.GetFiles(path, "*.meta", SearchOption.AllDirectories);
        int metaFileCount = metaFiles.Length;
        for (int i = metaFileCount - 1; i >= 0; i--)
        {
            File.Delete(metaFiles[i]);
            Debug.Log("delete meta file:" + metaFiles[i]);
        }
    }

}
