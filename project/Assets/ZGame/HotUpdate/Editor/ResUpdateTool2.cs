using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ZGame;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class ResUpdateTool2 : Editor
{
    [MenuItem("HotUpdate/游戏热更/获得当前资源版本信息")]
    public static void GetResVersion()
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }

        Debug.Log("resversion_" + ConfigUtility.Data.AppVersion + ", " + " localResVersion:" + localResVersion.ToString());
    }


    [MenuItem("HotUpdate/游戏热更/获得当前资源版本 zipped信息")]
    public static void GetMaxZippedIndex()
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }


        int index = PlayerPrefs.GetInt("maxzippedindex_" + ConfigUtility.Data.AppVersion + "_" + localResVersion, -1);

        Debug.Log("maxzippedindex：" + index.ToString());
    }
    [MenuItem("工具/PlayerPrefs/删除本项目Editor编辑器环境下的所有PlayerPrefs信息")]
    public static void DoReset1()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("删除本项目Editor下PlayerPrefs信息完成!");

        //编辑器下不要使用DoReset2中用到的删除注册表方式
        //会导致编辑器下再存储PlayerPrefs报错。只有重新启动Unity编辑器才能解决 
    }


    [MenuItem("工具/PlayerPrefs/删除本项目 PC包对应的所有PlayerPrefs信息")]
    public static void DoReset2()
    {
        string path = Application.dataPath + "/PlayerPrefs-Tool/PC-Tool.bat";
        string companyName = Application.companyName;
        string productName = Application.productName;
        Process.Start(path, $"{companyName} {productName}");
    }


    [MenuItem("HotUpdate/游戏热更/重置当前资源版本信息")]
    public static void DoResetResVersion()
    {
        PlayerPrefs.SetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        PlayerPrefs.Save();
        Debug.Log("设置完成!");
    }
}
