using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Obfuscation;
using ZGame.RessEditor;

public class EncryptAvatar
{
    [MenuItem("Assets/ZGame/对选择文件夹内所有png格式图片加密")]
    public static void DoEncrypt()
    {
        string key = "whosyourdaddy";
        var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.png");
        foreach (var path in allAssetPathList)
        {
            //string assetPath = path.Substring(path.IndexOf("Assets"));
            //var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture)) as Texture;
            var fileData = File.ReadAllBytes(path);
            XOR.EncryptFile(path, key);
        }
    }

    static List<string> GetAllTargetFilesFormSelectFolder(string _extension)
    {
        // 获取所有选中 文件、文件夹的 GUID
        string[] guids = Selection.assetGUIDs;
        List<string> allAssetPathList = new List<string>();
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // 判断是否文件夹
            if (Directory.Exists(assetPath))
            {
                searchDirectory(assetPath, _extension, ref allAssetPathList);
            }
        }
        return allAssetPathList;
    }

    static void searchDirectory(string directory, string _extension, ref List<string> outputList)
    {
        DirectoryInfo dInfo = new DirectoryInfo(directory);
        // 获取 文件夹以及子文件加中所有扩展名为  _extension 的文件
        FileInfo[] fileInfoArr = dInfo.GetFiles(_extension, SearchOption.AllDirectories);
        for (int i = 0; i < fileInfoArr.Length; ++i)
        {
            string fullName = fileInfoArr[i].FullName;
            Debug.Log(fullName);
            outputList.Add(fullName);
        }
    }
}
