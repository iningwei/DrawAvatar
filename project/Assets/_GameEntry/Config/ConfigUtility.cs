using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

public class ConfigUtility
{
    static Config _data;
    static bool _initialized = false;
    static readonly object _lock = new object();

    public static Config Data
    {
        get
        {
            return _data;
        }
    }

    public static string cfgFileName;
    //内置Resources目录下cfg文件的全路径
    public static string resConfigFilePath;
    //动态生成的cfg文件全路径
    static string dynamicConfigFilePath;
    static ConfigUtility()
    {
        ensureReadCfgData();
    }
    static void ensureReadCfgData()
    {
        if (_initialized)
        {
            return;
        }
        lock (_lock)
        {
            if (_initialized)
            {
                return;
            }

            readCfgData();
            _initialized = true;
        }
    }

    static void readCfgData()
    {
        _data = new Config();
        string configStr = "";
#if UNITY_ANDROID
            cfgFileName = "android_cfg";
#elif UNITY_IOS
            cfgFileName="ios_cfg";
#elif UNITY_STANDALONE_WIN
        cfgFileName = "pc_win_cfg";
#elif UNITY_STANDALONE_OSX
            cfgFileName="pc_mac_cfg";
#elif UNITY_WEBGL
            cfgFileName = "webgl_cfg";
#else
            cfgFileName="other_cfg";
#endif
        resConfigFilePath = Application.dataPath + "/ZGame/HotUpdate/Resources/Config/" + cfgFileName + ".bytes";

        dynamicConfigFilePath = Application.persistentDataPath + "/dynamic_cfg_v" + Application.version + "/" + cfgFileName + ".txt";
        if (File.Exists(dynamicConfigFilePath))
        {
            Debug.Log("get config data from dynamic path");
            configStr = File.ReadAllText(dynamicConfigFilePath);
        }
        else
        {
            Debug.Log("get config data from Resources path:" + "Config/" + cfgFileName);
            configStr = Resources.Load<TextAsset>("Config/" + cfgFileName).text;
        }

        if (string.IsNullOrEmpty(configStr))
        {
            Debug.LogError("error, configStr is empty!");
            return;
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("configStr:" + configStr);
#endif
        }

        try
        {
            _data = JsonUtility.FromJson<Config>(configStr);
        }
        catch (Exception ex)
        {
            Debug.LogError("error,while parse config JSON:" + ex.Message);
        }
    }

    /// <summary>
    /// 刷新  （用于热更或调试）
    /// </summary>
    public static void RefreshData()
    {
        lock (_lock)
        {
            _initialized = false;
            ensureReadCfgData();
        }
    }


    public static string GetLoginURL()
    {
        var channel = _data.ChannelInfo?.FirstOrDefault(a => a.ChannelID == _data.GameChannelId);
        var login = channel?.LoginData?.FirstOrDefault(a => a.LoginType == _data.LoginType);
        return login?.PostURL ?? string.Empty;
    }

    public static PackItem GetPackItem()
    {
        var channel = _data.ChannelInfo?.FirstOrDefault(c => c.ChannelID == _data.GameChannelId);
        return channel?.PackData?.FirstOrDefault(p => p.PackType == _data.PackType);
    }


    //////    public static string AssignConfigDataToJson(string productName, string appVersion, string appBundleVersion, string resVersion, string packTimeStamp, int gameChannelId,
    //////int paymentChannelId, bool isABResNameCrypto, string abResNameCryptoKey, int abResByteOffset, int screenOrientation, int gameInputType, int resLoadType, string firstOpenWindowName, Vector2 gameDesignRatio, List<LoginData> loginDataList, List<PackData> packDataList, string loginType,
    //////string packType, bool isRealPurchase, bool isShowProtoMsgLog, bool isShowDebugBtn,
    //////bool isEnableLogTrace, bool isEnableLogRealtimeWriteToLocal, bool isEnableLogUpdate2Server, bool isShowReporter)
    //////    {

    //////        Dictionary<string, object> configDic = new Dictionary<string, object>();
    //////        configDic["ProductName"] = productName;
    //////        configDic["AppVersion"] = appVersion;
    //////        configDic["AppBundleVersion"] = appBundleVersion;
    //////        configDic["ResVersion"] = resVersion;
    //////        configDic["PackTimeStamp"] = packTimeStamp;
    //////        configDic["GameChannelId"] = gameChannelId;
    //////        configDic["PaymentChannelId"] = paymentChannelId;
    //////        configDic["IsABResNameCrypto"] = isABResNameCrypto;
    //////        configDic["ABResNameCryptoKey"] = abResNameCryptoKey;
    //////        configDic["ABResByteOffset"] = abResByteOffset;
    //////        configDic["GameDesignRatio"] = gameDesignRatio.x + "," + gameDesignRatio.y;
    //////        configDic["ScreenOrientation"] = screenOrientation;
    //////        configDic["GameInputType"] = gameInputType;
    //////        configDic["ResLoadType"] = resLoadType;
    //////        configDic["FirstOpenWindowName"] = firstOpenWindowName;



    //////        //组装LoginData
    //////        List<object> loginList = new List<object>();
    //////        if (loginDataList != null)
    //////        {
    //////            int count = loginDataList.Count;
    //////            for (int i = 0; i < count; i++)
    //////            {
    //////                var tmpData = loginDataList[i];
    //////                Dictionary<string, object> loginDataDic = new Dictionary<string, object>();
    //////                loginDataDic["LoginType"] = tmpData.loginType;
    //////                loginDataDic["PostURL"] = tmpData.postURL;
    //////                loginList.Add(loginDataDic);
    //////            }
    //////        }
    //////        configDic["LoginData"] = loginList;

    //////        //组装PackData
    //////        List<object> packList = new List<object>();
    //////        if (packDataList != null)
    //////        {
    //////            int count = packDataList.Count;
    //////            for (int i = 0; i < count; i++)
    //////            {
    //////                var tmpData = packDataList[i];
    //////                Dictionary<string, object> packDataDic = new Dictionary<string, object>();
    //////                packDataDic["PackType"] = tmpData.packType;
    //////                packDataDic["FtpTxtFileURL"] = tmpData.ftpTxtFileUrl;
    //////                packDataDic["FtpZipFileURL"] = tmpData.ftpZipFileUrl;
    //////                packList.Add(packDataDic);
    //////            }
    //////        }
    //////        configDic["PackData"] = packList;

    //////        configDic["LoginType"] = loginType;
    //////        configDic["PackType"] = packType;
    //////        //其它  
    //////        configDic["IsRealPurchase"] = isRealPurchase;
    //////        configDic["IsShowProtoMsgLog"] = isShowProtoMsgLog;
    //////        configDic["IsShowDebugBtn"] = isShowDebugBtn;
    //////        configDic["IsEnableLogTrace"] = isEnableLogTrace;
    //////        configDic["IsEnableLogRealtimeWriteToLocal"] = isEnableLogRealtimeWriteToLocal;
    //////        configDic["IsEnableLogUpdate2Server"] = isEnableLogUpdate2Server;
    //////        configDic["IsShowReporter"] = isShowReporter;

    //////        string jsonStr = Json.Serialize(configDic);
    //////        return jsonStr;
    //////    }

    //////    public static void WriteToResConfig(string jsonStr)
    //////    {
    //////        IOTools.WriteString(resConfigFilePath, jsonStr);
    //////    }

    //////    public static void WriteToDynamicConfig(string jsonStr)
    //////    {
    //////        IOTools.WriteString(dynamicConfigFilePath, jsonStr);
    //////    }

    //////public static void SaveDynamicConfig(string loginType, string packType
    //////    , string isRealPurchase
    //////    , string isShowProtoMsgLog
    //////    , string isShowDebugBtn
    //////    , string isEnableLogTrace
    //////    , string isEnableLogRealtimeWriteToLocal
    //////    , string isEnableLogUpdate2Server
    //////    , string isShowReporter)
    //////{
    //////    var jsonStr = AssignConfigDataToJson(productName, appVersion, appBundleVersion, resVersion, packTimeStamp, gameChannelId, paymentChannelId, isABResNameCrypto, abResNameCryptoKey, abResByteOffset, screenOrientation, gameInputType, resLoadType, firstOpenWindowName, gameDesignRatio, loginDataList,
    //////          packDataList,
    //////       loginType, packType, bool.Parse(isRealPurchase), bool.Parse(isShowProtoMsgLog), bool.Parse(isShowDebugBtn), bool.Parse(isEnableLogTrace), bool.Parse(isEnableLogRealtimeWriteToLocal), bool.Parse(isEnableLogUpdate2Server), bool.Parse(isShowReporter)
    //////          );
    //////    WriteToDynamicConfig(jsonStr);
    //////}


    //////public static PackData GetPackData(string packType)
    //////{
    //////    for (int i = 0; i < packDataList.Count; i++)
    //////    {
    //////        if (packDataList[i].packType == packType)
    //////        {
    //////            return packDataList[i];
    //////        }
    //////    }
    //////    Debug.LogError("no PackData with PackType:" + packType);
    //////    return null;
    //////}
    //////public static LoginData GetLoginData(string loginType)
    //////{
    //////    for (int i = 0; i < loginDataList.Count; i++)
    //////    {
    //////        if (loginDataList[i].loginType == loginType)
    //////        {
    //////            return loginDataList[i];
    //////        }
    //////    }
    //////    Debug.LogError("no LoginData with LoginType:" + loginType);
    //////    return null;
    //////}
}