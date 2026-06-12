using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using MiniJSON;

public class HybridCLRAppMsgData
{
    public string LoginType;
    public string CurMaxResVersion;
    public string WhitelistCurMaxResVersion;
    public bool IsPassed;
    public HybridCLRAppMsgData(string loginType, string curMaxResVersion, string whitelistCurMaxResVersion, bool isPassed)
    {
        this.LoginType = loginType;
        this.CurMaxResVersion = curMaxResVersion;
        this.WhitelistCurMaxResVersion = whitelistCurMaxResVersion;
        this.IsPassed = isPassed;
    }
}

/// <summary>
/// 服务器列表
/// </summary>
public class ServerList : Singleton<ServerList>
{

    public string CurMaxAppVersion;
    public string CanRunMinAppVersion;

    public string IOSJumpStoreKey;       //大版本更新对应的打开商店的地址 
    public string AndroidJumpStoreKey;//

    public Dictionary<string, HybridCLRAppMsgData> AppMsgDic = new Dictionary<string, HybridCLRAppMsgData>();//具体某个版本app信息

    public void Init(string json)
    {

        var dic = Json.Deserialize(json) as Dictionary<string, object>;


        this.CurMaxAppVersion = dic["CurMaxAppVersion"].ToString();
        this.CanRunMinAppVersion = dic["CanRunMinAppVersion"].ToString();

        this.IOSJumpStoreKey = dic["IOSJumpStoreKey"].ToString();
        this.AndroidJumpStoreKey = dic["AndroidJumpStoreKey"].ToString();

        var list = dic["AppMsg"] as List<object>;

        for (int i = 0; i < list.Count; i++)
        {
            var appDic = list[i] as Dictionary<string, object>;

            var appVersion = appDic["AppVersion"].ToString();
            var loginType = appDic["LoginType"].ToString();
            var curMaxResVersion = appDic["CurMaxResVersion"].ToString();
            var whitelistCurMaxResVersion = appDic["WhitelistCurMaxResVersion"].ToString();
            var isPassed = Boolean.Parse(appDic["IsPassed"].ToString());
            AppMsgDic.Add(appVersion, new HybridCLRAppMsgData(loginType, curMaxResVersion, whitelistCurMaxResVersion, isPassed));
        }

    }


    public string GetAppMaxResVersion(string appVersion)
    {
        if (AppMsgDic.ContainsKey(appVersion))
        {
            if (PlayerPrefs.GetInt("IsWhitelist", 0) == 1)//白名单客户
            {
                return AppMsgDic[appVersion].WhitelistCurMaxResVersion;
            }
            return AppMsgDic[appVersion].CurMaxResVersion;
        }
        else
        {
            Debug.LogError($"GetAppMaxResVersion error, no appVersion:{appVersion} in serverlist");
            return "0";
        }
    }

    public bool IsPassed(string appVersion)
    {
#if HybridCLR_HOTUPDATE
        if (AppMsgDic.ContainsKey(appVersion))
        {
            return AppMsgDic[appVersion].IsPassed;
        }
        else
        {
            Debug.LogError($"IsPassed error, no appVersion:{appVersion} in serverlist");
            return false;
        }
#endif

        return true;
    }
}