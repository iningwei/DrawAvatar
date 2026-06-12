//using MiniJSON;
//using System.Collections;
//using System.Collections.Generic;
//using System.IO;
//using UnityEngine;

////热更资源CDN相关信息
//public class HybridCLRPackData
//{
//    public string packType;
//    public string ftpTxtFileUrl;
//    public string ftpZipFileUrl;
//}
//public class HybridCLRConfig
//{
//    public static string appVersion;
//    public static string resVersion;
//    public static int resLoadType;
//    public static bool isABResNameCrypto;
//    public static string abResNameCryptoKey;
//    public static int abResByteOffset;

//    public static string loginType;
//    public static string packType;
//    public static int gameChannelId;

//    public static List<HybridCLRPackData> packDataList = new List<HybridCLRPackData>();

//    static string cfgFileName;
//    //内置Resources目录下cfg文件的全路径
//    public static string resConfigFilePath;
//    //动态生成的cfg文件全路径
//    static string dynamicConfigFilePath;

//    static HybridCLRConfig()
//    {
//        string configStr = "";

//#if UNITY_ANDROID
//        cfgFileName = "android_cfg";
//#elif UNITY_IOS
//        cfgFileName="ios_cfg";
//#elif UNITY_STANDALONE_WIN
//        cfgFileName = "pc_win_cfg";
//#elif UNITY_STANDALONE_OSX
//        cfgFileName="pc_mac_cfg";
//#elif UNITY_WEBGL
//        cfgFileName = "webgl_cfg";
//#else
//        cfgFileName="other_cfg";
//#endif

//        resConfigFilePath = Application.dataPath + "/ZGame/HotUpdate/Resources/Config/" + cfgFileName + ".bytes";
//        dynamicConfigFilePath = Application.persistentDataPath + "/dynamic_cfg_v" + Application.version + "/" + cfgFileName + ".txt";
//        if (File.Exists(dynamicConfigFilePath))
//        {
//            //Debug.Log("HybridCLRConfig get config from dynamic path");
//            configStr = File.ReadAllText(dynamicConfigFilePath);
//        }
//        else
//        {
//            //Debug.Log("HybridCLRConfig get config from Resources path");
//            configStr = Resources.Load<TextAsset>("Config/" + cfgFileName).text;
//        }

//        if (string.IsNullOrEmpty(configStr))
//        {
//            Debug.LogError("error, configStr is empty!");
//        }
//        else
//        {
//#if UNITY_EDITOR
//            Debug.Log("configStr:" + configStr);
//#endif
//        }

//        var dic = Json.Deserialize(configStr) as Dictionary<string, object>;

//        appVersion = (string)dic["AppVersion"];

//        resVersion = (string)dic["ResVersion"];

//        gameChannelId = (int)(long)dic["GameChannelId"];

//        loginType = (string)dic["LoginType"];
//        packType = (string)dic["PackType"];
//        resLoadType = (int)(long)dic["ResLoadType"];
//        isABResNameCrypto = bool.Parse(dic["IsABResNameCrypto"].ToString());
//        abResNameCryptoKey = (string)dic["ABResNameCryptoKey"];
//        abResByteOffset = (int)(long)dic["ABResByteOffset"];


//        var packDataArray = dic["PackData"] as List<object>;
//        for (int i = 0; i < packDataArray.Count; i++)
//        {
//            var packDataDic = packDataArray[i] as Dictionary<string, object>;
//            HybridCLRPackData data = new HybridCLRPackData();
//            data.packType = (string)packDataDic["PackType"];
//            data.ftpTxtFileUrl = (string)packDataDic["FtpTxtFileURL"];
//            data.ftpZipFileUrl = (string)packDataDic["FtpZipFileURL"];

//            packDataList.Add(data);
//        }
//    }
//    public static HybridCLRPackData GetPackData(string packType)
//    {
//        for (int i = 0; i < packDataList.Count; i++)
//        {
//            if (packDataList[i].packType == packType)
//            {
//                return packDataList[i];
//            }
//        }
//        Debug.LogError("no PackData with PackType:" + packType);
//        return null;
//    }
//}
