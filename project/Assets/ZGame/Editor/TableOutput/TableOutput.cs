using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using ZGame;
using ZGame.Ress;

public class TableOutput : Editor
{
    [MenuItem("工具/Table/output")]
    static void Output()
    {
        string path = Application.dataPath + "/../table_tools/output.bat";
        UnityEngine.Debug.Log("process path:" + path);
        Process.Start(path);

    }

    [MenuItem("工具/Table/Open Excel sheet folder _#&T")]
    private static void OpenExcelFolder()
    {
        string path = Application.dataPath + "/../../table";
        Process.Start(path);
    }
    [MenuItem("工具/Table/Move/Move table data to target folder")]
    private static void MoveWebGLTableToDesFolder()
    {
        if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
        {
            Debug.Log("resLoadType is Resources， move table data to Resources folder");
            MoveTableToResources();
        }
        else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
        {
            Debug.Log("resLoadType is Addressable， move table data to ResourcesTable folder");
            MoveTableToResourcesTable();
        }
    }


    private static void MoveTableToResources()
    {
        string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        string targetPath = Application.dataPath + "/Resources/ResEx/" + IOTools.PlatformFolderName;

        IOTools.CreateDirectorySafe(targetPath);
        Debug.Log("delete all files under Resources/ResEx");
        IOTools.DeleteAllFiles(targetPath, true);
        IOTools.MoveFilesWithNewSurfix(sourcePath, targetPath, "bytes");
        Debug.Log("copy finished");
        AssetDatabase.Refresh();
    }

    private static void MoveTableToResourcesTable()
    {
        string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        string targetPath = Application.dataPath + "/ResourcesTable/ResEx/" + IOTools.PlatformFolderName;

        IOTools.CreateDirectorySafe(targetPath);
        //Debug.Log("delete all files under Resources/ResEx");
        //IOTools.DeleteAllFiles(targetPath, true);//addressable下若先删除文件，再移动的话，会导致Addressables Groups中之前设置的相关资源失效，需要重新设置，故这里取消删除，需要自己注意这部分资源的处理
        IOTools.MoveFilesWithNewSurfix(sourcePath, targetPath, "bytes");
        Debug.Log($"copy finished,sourcePath:{sourcePath},  targetPath:{targetPath}");
        AssetDatabase.Refresh();
    }
}
