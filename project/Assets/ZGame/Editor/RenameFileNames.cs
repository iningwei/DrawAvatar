using System.Collections;
using System.Collections.Generic;
using System.IO; 
using UnityEditor;
using UnityEngine;
using ZGame.Obfuscation;

public class RenameFileNames
{
    [MenuItem("Assets/ZGame/对选择文件夹内所有文件名小写化")]
    public static void ToLower()
    {
        var allAssetPathList = GetAllTargetFilesFormSelectFolder();
        foreach (var path in allAssetPathList)
        {
            if (path.EndsWith(".meta") || path.EndsWith(".asset"))
            {
                continue;
            }
            string fileDirectory = new FileInfo(path).DirectoryName;//文件夹路径
            string fileName = new FileInfo(path).Name;//文件名
            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(path);//文件名不带后缀 
            string assetPath = path.Substring(path.IndexOf("Assets"));
            //Debug.Log("--------->");
            //Debug.Log("path:" + path);
            //Debug.Log("fileDirectory:" + fileDirectory);
            //Debug.Log("fileName:" + fileName);
            //Debug.Log("assetPath:" + assetPath);

            AssetDatabase.RenameAsset(assetPath, fileNameWithoutExt.ToLower());//调用unity提供的api，不会破坏资源的依赖关系
        }
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
    }

    static List<string> GetAllTargetFilesFormSelectFolder()
    {
        // 获取所有选中 文件、文件夹的 GUID
        string[] guids = Selection.assetGUIDs;
        List<string> allAssetPathList = new List<string>();
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            //Debug.Log("assetPath:" + assetPath);
            // 判断是否文件夹
            if (Directory.Exists(assetPath))
            {
                searchDirectory(assetPath, ref allAssetPathList);
            }
        }
        return allAssetPathList;
    }

    static void searchDirectory(string directory, ref List<string> outputList)
    {
        DirectoryInfo dInfo = new DirectoryInfo(directory);
        // 获取 文件夹以及子文件夹中所有文件
        FileInfo[] fileInfoArr = dInfo.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < fileInfoArr.Length; ++i)
        {
            string fullName = fileInfoArr[i].FullName;
            //Debug.Log(fullName);
            outputList.Add(fullName);
        }
    }
}
