using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;

public class PackVisualizeTool : EditorWindow
{
    public static string productName;
    public static string appVersion;
    public static string appBundleVersion;
    public static string resVersion;
    public static string packTimeStamp;

    public static int gameChannelId;
    public static int paymentChannelId;
    public static bool isABResNameCrypto;
    public static string abResNameCryptoKey;
    public static int abResByteOffset;
    public static ScreenOrientation screenOrientation;
    public static int gameInputType;
    public static int resLoadType;
    public static string firstOpenWindowName;

    public static Vector2 gameDesignRatio;

    //////public static List<LoginData> loginDataList = new List<LoginData>();
    //////public static List<PackData> packDataList = new List<PackData>();

    public static string loginType;
    public static string packType;
    public static bool isRealPurchase;
    public static bool isShowProtoMsgLog;
    public static bool isShowDebugBtn;
    public static bool isEnableLogTrace;
    public static bool isEnableLogRealtimeWriteToLocal;
    public static bool isEnableLogUpdate2Server;
    public static bool isShowReporter;

    public static bool[] diyMacros;
    public static string[] macroNames = { "HybridCLR_HOTUPDATE", "HybridCLR_INSTALLED", "SKIP_SPLASH", "IAP", };

    public static bool isDIYMacros;
    public static bool isProjectMacros;

    Vector2 scrollPos;
    bool configFoldOut = true;
    bool packFoldOut = true;
    bool macrosFoldOut = true;

    [MenuItem("工具/打包/打包预览")]
    static void PackVisual()
    {
        PackVisualizeTool visualizeTool = EditorWindow.GetWindow(typeof(PackVisualizeTool)) as PackVisualizeTool;
        //visualizeTool.Show();
        init();
    }

    private static void init()
    {
        //////productName = ConfigUtility.Data.ProductName;

        //////appVersion = ConfigUtility.Data.AppVersion;
        //////appBundleVersion = ConfigUtility.Data.AppBundleVersion;
        //////resVersion = ConfigUtility.Data.ResVersion;
        //////packTimeStamp = Config.packTimeStamp;

        //////gameChannelId = ConfigUtility.Data.GameChannelId;
        //////paymentChannelId = Config.paymentChannelId;
        //////isABResNameCrypto = ConfigUtility.Data.IsABResNameCrypto;
        //////abResNameCryptoKey = ConfigUtility.Data.ABResNameCryptoKey;
        //////abResByteOffset = ConfigUtility.Data.ABResByteOffset;
        //////screenOrientation = (ScreenOrientation)Config.screenOrientation;
        //////gameInputType = Config.gameInputType;
        //////resLoadType = ConfigUtility.Data.ResLoadType;
        //////firstOpenWindowName = Config.firstOpenWindowName;
        //////gameDesignRatio = Config.gameDesignRatio;

        //////loginDataList = Config.loginDataList;
        //////packDataList = Config.packDataList;

        //////loginType = Config.loginType;
        //////packType = Config.packType;
        //////isRealPurchase = Config.isRealPurchase;
        //////isShowProtoMsgLog = Config.isShowProtoMsgLog;
        //////isShowDebugBtn = Config.isShowDebugBtn;
        //////isEnableLogTrace = Config.isEnableLogTrace;
        //////isEnableLogRealtimeWriteToLocal = Config.isEnableLogRealtimeWriteToLocal;
        //////isEnableLogUpdate2Server = Config.isEnableLogUpdate2Server;
        //////isShowReporter = Config.isShowReporter;

        //////diyMacros = new bool[macroNames.Length];
    }

    private void OnGUI()
    {

        //////        configFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(configFoldOut, "配置");
        //////        if (configFoldOut)
        //////        {
        //////            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        //////            GUILayout.Label("产品名", EditorStyles.boldLabel);
        //////            productName = EditorGUILayout.TextField("ProductName:", productName);
        //////            GUILayout.Label("版本信息：", EditorStyles.boldLabel);
        //////            appVersion = EditorGUILayout.TextField("AppVersion:", appVersion);
        //////            appBundleVersion = EditorGUILayout.TextField("AppBundleVersion:", appBundleVersion);
        //////            resVersion = EditorGUILayout.TextField("ResVersion:", resVersion);
        //////            GUILayout.BeginHorizontal();
        //////            packTimeStamp = EditorGUILayout.TextField("PackTimeStamp", packTimeStamp, GUILayout.ExpandWidth(true));
        //////            //GUILayout.FlexibleSpace();
        //////            if (GUILayout.Button("刷新时间戳"))
        //////            {
        //////                string stamp = TimeTool.GetyyyyMMddHHmm(DateTime.Now, "").Substring(2);
        //////                Debug.Log("refresh time stamp:" + stamp);
        //////                packTimeStamp = stamp;
        //////            }
        //////            GUILayout.EndHorizontal();

        //////            GUILayout.Label("渠道信息：", EditorStyles.boldLabel);
        //////            gameChannelId = EditorGUILayout.IntField("GameChannelId:", gameChannelId);
        //////            paymentChannelId = EditorGUILayout.IntField("paymentChannelId:", paymentChannelId);

        //////            GUILayout.Label("加密信息：", EditorStyles.boldLabel);
        //////            isABResNameCrypto = bool.Parse(EditorGUILayout.TextField("IsABResNameCrypto:", isABResNameCrypto.ToString()));
        //////            abResNameCryptoKey = EditorGUILayout.TextField("ABResNameCryptoKey:", abResNameCryptoKey);
        //////            abResByteOffset = EditorGUILayout.IntField("ABResByteOffset:", abResByteOffset);
        //////            GUILayout.Label("显示信息：", EditorStyles.boldLabel);
        //////            screenOrientation = (ScreenOrientation)EditorGUILayout.EnumPopup("横屏or竖屏", screenOrientation);

        //////            GUILayout.Label("操作类型，1：鼠键，2：触屏", EditorStyles.boldLabel);
        //////            gameInputType = EditorGUILayout.IntField("GameInputType:", gameInputType);
        //////            GUILayout.Space(10);

        //////            GUILayout.Label("资源加载类型，0：AssetBundle，1：Resources", EditorStyles.boldLabel);
        //////            resLoadType = EditorGUILayout.IntField("ResLoadType:", resLoadType);
        //////            GUILayout.Space(10);

        //////            firstOpenWindowName = EditorGUILayout.TextField("FirstOpenWindowName:", firstOpenWindowName);
        //////            GUILayout.Space(10);

        //////            gameDesignRatio = EditorGUILayout.Vector2Field("GameDesignRatio", gameDesignRatio);

        //////            GUILayout.Space(10);
        //////            GUILayout.Label("登录服务器信息：", EditorStyles.boldLabel);
        //////            if (loginDataList != null)
        //////            {
        //////                for (int i = 0; i < loginDataList.Count; i++)
        //////                {
        //////                    var tmp = loginDataList[i];
        //////                    GUILayout.Label("服务器 " + (i + 1) + " ----------->");
        //////                    tmp.loginType = EditorGUILayout.TextField("LoginType:", tmp.loginType);
        //////                    tmp.postURL = EditorGUILayout.TextField("URL:", tmp.postURL);
        //////                }
        //////            }

        //////            GUILayout.Space(10);
        //////            GUILayout.Label("热更资源CDN信息：", EditorStyles.boldLabel);
        //////            if (packDataList != null)
        //////            {
        //////                for (int i = 0; i < packDataList.Count; i++)
        //////                {
        //////                    var tmp = packDataList[i];
        //////                    GUILayout.Label("热更资源 " + (i + 1) + " ----------->");
        //////                    tmp.packType = EditorGUILayout.TextField("PackType:", tmp.packType);
        //////                    tmp.ftpTxtFileUrl = EditorGUILayout.TextField("FtpTxtFileUrl:", tmp.ftpTxtFileUrl);
        //////                    tmp.ftpZipFileUrl = EditorGUILayout.TextField("FtpZipFileUrl:", tmp.ftpZipFileUrl);
        //////                }
        //////            }
        //////            GUILayout.Space(10);
        //////            GUILayout.Label("包体配置信息：", EditorStyles.boldLabel);
        //////            loginType = EditorGUILayout.TextField("登录服务器(PUB、DEV):", loginType);
        //////            packType = EditorGUILayout.TextField("热更CDN(PUB、DEV、TEST):", packType);
        //////            isRealPurchase = bool.Parse(EditorGUILayout.TextField("IsRealPurchase:", isRealPurchase.ToString()).ToString());
        //////            isShowProtoMsgLog = bool.Parse(EditorGUILayout.TextField("IsShowProtoMsgLog:", isShowProtoMsgLog.ToString()).ToString());
        //////            isShowDebugBtn = bool.Parse(EditorGUILayout.TextField("IsShowDebugBtn:", isShowDebugBtn.ToString()).ToString());
        //////            isEnableLogTrace = bool.Parse(EditorGUILayout.TextField("IsEnableLogTrace:", isEnableLogTrace.ToString()).ToString());
        //////            isEnableLogRealtimeWriteToLocal = bool.Parse(EditorGUILayout.TextField("IsEnableLogRealtimeWriteToLocal:", isEnableLogRealtimeWriteToLocal.ToString()).ToString());
        //////            isEnableLogUpdate2Server = bool.Parse(EditorGUILayout.TextField("IsEnableLogUpdate2Server:", isEnableLogUpdate2Server.ToString()).ToString());
        //////            isShowReporter = bool.Parse(EditorGUILayout.TextField("IsShowReporter:", isShowReporter.ToString()).ToString());

        //////            GUILayout.Space(10);

        //////            if (GUILayout.Button("保存配置"))
        //////            {
        //////                var jsonStr = Config.AssignConfigDataToJson(productName, appVersion, appBundleVersion, resVersion, packTimeStamp, gameChannelId, paymentChannelId, isABResNameCrypto, abResNameCryptoKey, abResByteOffset, (int)screenOrientation, gameInputType, resLoadType, firstOpenWindowName, gameDesignRatio, loginDataList,
        //////                      packDataList,
        //////loginType, packType, isRealPurchase, isShowProtoMsgLog, isShowDebugBtn, isEnableLogTrace, isEnableLogRealtimeWriteToLocal, isEnableLogUpdate2Server,
        //////isShowReporter);
        //////                Config.WriteToResConfig(jsonStr);
        //////                Debug.Log("save success:--->" + jsonStr);
        //////                AssetDatabase.Refresh();//必须刷新一下，否则Config.RefreshData()中调用的Resources.Load读取的是缓存中的老数据
        //////                Config.RefreshData();
        //////            }

        //////            EditorGUILayout.EndScrollView();
        //////        }

        //////        //获取当前是哪个平台
        //////        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        //////        //获得当前平台已有的的宏定义
        //////        var curMacros = PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup));

        //////        EditorGUILayout.EndFoldoutHeaderGroup();
        //////        macrosFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(macrosFoldOut, "宏");
        //////        if (macrosFoldOut)
        //////        {
        //////            isDIYMacros = EditorGUILayout.BeginToggleGroup("自定义宏", isDIYMacros);
        //////            if (isDIYMacros)
        //////            {
        //////                for (int i = 0; i < macroNames.Length; i++)
        //////                {
        //////                    diyMacros[i] = EditorGUILayout.Toggle(macroNames[i], diyMacros[i]);
        //////                }
        //////            }

        //////            EditorGUILayout.EndToggleGroup();

        //////            if (isDIYMacros)
        //////            {
        //////                isProjectMacros = false;
        //////            }
        //////            else
        //////            {
        //////                isProjectMacros = true;
        //////            }


        //////            isProjectMacros = EditorGUILayout.ToggleLeft("使用项目当前宏", isProjectMacros);
        //////            if (isProjectMacros)
        //////            {
        //////                EditorGUILayout.LabelField("------->宏：" + curMacros);
        //////                isDIYMacros = false;
        //////            }
        //////            else
        //////            {
        //////                isDIYMacros = true;
        //////            }
        //////        }
        //////        EditorGUILayout.EndFoldoutHeaderGroup();

        //////        string targetMacros = "";
        //////        if (isDIYMacros)
        //////        {
        //////            for (int i = 0; i < diyMacros.Length; i++)
        //////            {
        //////                if (diyMacros[i] == true)
        //////                {
        //////                    targetMacros += (macroNames[i] + ";");
        //////                }
        //////            }
        //////            targetMacros.TrimEnd(';');
        //////        }
        //////        else
        //////        {
        //////            targetMacros = curMacros;
        //////        }
        //////        packFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(packFoldOut, "打包");
        //////        if (packFoldOut)
        //////        {
        //////            if (GUILayout.Button("打APK包"))
        //////            {
        //////                if (EditorUtility.DisplayDialog("警告", "打APK, 目标宏为：" + targetMacros + "------->是否继续??？", "OK", "Cancel"))
        //////                {
        //////                    PackTool.BuildFullAPK(targetMacros);
        //////                }
        //////            }
        //////            if (GUILayout.Button("整理热更代码和热更资源"))
        //////            {
        //////                if (EditorUtility.DisplayDialog("警告", "整理热更代码和热更资源, appVersion:" + ConfigUtility.Data.AppVersion + ",resVersion:" + ConfigUtility.Data.ResVersion + ",channelId:" + ConfigUtility.Data.GameChannelId + "------->是否继续??？", "OK", "Cancel"))
        //////                {
        //////                    new HotResCollector().Build();
        //////                }
        //////            }

        //////            //if (GUILayout.Button("打热更APK包"))
        //////            //{
        //////            //    PackTool.BuildHotupdateAPK();
        //////            //}
        //////            //if (GUILayout.Button("打全量Xcode包"))
        //////            //{
        //////            //    PackTool.BuildFullXCodeProj();
        //////            //}
        //////            //if (GUILayout.Button("打热更Xcode包"))
        //////            //{
        //////            //    PackTool.BuildHotXCodeProj();
        //////            //}
        //////        }
        //////        EditorGUILayout.Space(10);
        //////        GUILayout.Label("---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------",
        //////            GUILayout.ExpandWidth(true)
        //////            );
        //////        EditorGUILayout.EndFoldoutHeaderGroup();

    }

}
