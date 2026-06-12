using System.Text.RegularExpressions;
using System.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using ZGame;
#if HybridCLR_INSTALLED
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;
#endif

public class PackTool
{

    static string isPackKey = "isPack";//是否在打包
    static string curMacrosKey = "curMacros";//当前编辑器设置的宏
    static string targetMacrosKey = "targetMacros";//打包需要设置的目标宏
    static string buildFuncNameKey = "buildFuncName";//打包函数名

    static string remoteShareDiskPath = @"\\172.16.10.128\share\GamePakg\元宇宙";


    [MenuItem("工具/打包/IOS/打全量XCode工程")]
    public static void BuildFullXCodeProj()
    {
        //////string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        //////if (EditorUtility.DisplayDialog("警告", "打全量XCode工程  当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        //////{
        //////    BuildLuaBundle.build();

        //////    Debug.LogError("copyResFilesToStreamingAssets");
        //////    copyAllResFilesToStreamingAssets();

        //////    Debug.LogError("build XCode");
        //////    // buildXCode("f");
        //////    EditorPrefs.SetInt("isPack", 1);
        //////    MacroCodeDetection("buildXCode", "f");

        //////    Debug.Log("全量 XCode 工程输出完毕！");
        //////    AssetDatabase.Refresh();
        //////}
        //////else
        //////{
        //////    Debug.LogError("取消打包");
        //////}
    }
    [MenuItem("工具/打包/IOS/打 热更 XCode工程")]
    public static void BuildHotXCodeProj()
    {
        //////string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS);
        //////if (EditorUtility.DisplayDialog("警告", "打 热更 XCode工程  当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        //////{
        //////    BuildLuaBundle.build();

        //////    Debug.LogError("copyHotUpdateInitResFilesToStreamingAssets");
        //////    bool cr = copyHotUpdateInitResFilesToStreamingAssets();
        //////    if (cr == false)
        //////    {
        //////        return;
        //////    }

        //////    Debug.LogError("build XCode");
        //////    // buildXCode("h");
        //////    EditorPrefs.SetInt("isPack", 1);
        //////    MacroCodeDetection("buildXCode", "h");
        //////    Debug.Log("热更 XCode 工程输出完毕！");
        //////    AssetDatabase.Refresh();

        //////}
        //////else
        //////{
        //////    Debug.LogError("取消打包");
        //////}
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="macros">;分割各个宏</param>
    [MenuItem("工具/打包/安卓/打apk")]
    public static void BuildFullAPK(string targetMacros)
    {
        string curMacros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        Debug.Log("cur macros:" + curMacros);

#if HybridCLR_HOTUPDATE
        SetHybridCLRHotUpdateAssemblies();

        Debug.Log("begin-> HybridCLR Generate All");
        PrebuildCommand.GenerateAll();

        Debug.Log("begin->热更DLL移动到ResEx目录");
        HybridCLRResUpdateTool.MoveHotupdateDll2ResEx();
        AssetDatabase.Refresh();
#else
        ClearHybridCLRHotUpdateAssemblies();
        AssetDatabase.Refresh();
#endif




        Debug.Log("begin-> copyResFilesToStreamingAssets");
        copyAllResFilesToStreamingAssets();
        string outputFolderPath = buildWithTargetMacros("buildAndroid", curMacros, targetMacros);

        string outputFolderName = Path.GetFileName(outputFolderPath);

        //删除安卓打包后生成的无用文件夹：
        string outputAPKName = outputFolderName.Substring(0, outputFolderName.LastIndexOf('_'));
        IOTools.DeleteFolder($"{outputFolderPath}\\{outputAPKName}_BackUpThisFolder_ButDontShipItWithYourGame");
        IOTools.DeleteFolder($"{outputFolderPath}\\{outputAPKName}_BurstDebugInformation_DoNotShip");

        AssetDatabase.Refresh();
        CopyToShareDisk.DoCopy2SMB(outputFolderPath, remoteShareDiskPath + "\\" + IOTools.PlatformFolderName + "\\" + outputFolderName);
    }



    [MenuItem("工具/打包/安卓/打热更apk")]
    public static void BuildHotupdateAPK()
    {
        //////string macros = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android);
        //////if (EditorUtility.DisplayDialog("警告", "打热更apk 当前宏为：" + macros + "------->是否继续??？", "OK", "Cancel"))
        //////{
        //////    //打lua ab
        //////    //BuildLuaBundle.build();
        //////    //不用打了，否则就是最新的lua脚本了

        //////    //从热更目录中 拷贝初始资源到StreamingAssets目录
        //////    bool cr = copyHotUpdateInitResFilesToStreamingAssets();
        //////    if (cr == false)
        //////    {
        //////        return;
        //////    }

        //////    // buildAndroid("h");
        //////    EditorPrefs.SetInt("isPack", 1);
        //////    MacroCodeDetection("buildAndroid", "h");

        //////    Debug.Log("安卓  一键打热更包完毕！");
        //////    AssetDatabase.Refresh();
        //////}
        //////else
        //////{
        //////    Debug.LogError("取消打包");
        //////}
    }
    static string buildWithTargetMacros(string buildFuncName, string curMacros, string targetMacros)
    {
        EditorPrefs.SetInt(isPackKey, 1);
        EditorPrefs.SetString(buildFuncNameKey, buildFuncName);
        EditorPrefs.SetString(targetMacrosKey, targetMacros);
        EditorPrefs.SetString(curMacrosKey, curMacros);


        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), targetMacros);

        string outputName = "";
        try
        {
            Type type = typeof(PackTool);//获取类名
            MethodInfo mt = type.GetMethod(buildFuncName, System.Reflection.BindingFlags.IgnoreCase
                    | System.Reflection.BindingFlags.NonPublic
                    | System.Reflection.BindingFlags.Static);//获取方法信息
            object obj = System.Activator.CreateInstance(type);
            Debug.Log($"call buildFuncName:{buildFuncName}");
            outputName = (string)mt.Invoke(obj, null);
            reset();
        }
        catch (NullReferenceException e)
        {
            Debug.LogError("error while buildWithTargetMacros:" + e);
            reset();
        }
        return outputName;
    }


    static void buildXCode(string suffix)
    {
        var scenes = getBuildScenes();

        setVersion();
        setProductName();

        AssetDatabase.SaveAssets();

        string locatePathName = $"{Application.dataPath}/../{ConfigUtility.Data.ProductName}-XCodeProj-v{ConfigUtility.Data.AppVersion}-{ConfigUtility.Data.AppBundleVersion}-{suffix}";

        BuildPipeline.BuildPlayer(scenes, locatePathName, BuildTarget.iOS, BuildOptions.None);
    }

    static void outputAndroidStudio()
    {

    }
    static string buildAndroid()
    {
        var scenes = getBuildScenes();
        SetKeystore();
        setVersion();
        setProductName();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string apkName = $"{ConfigUtility.Data.ProductName}_v{ConfigUtility.Data.AppVersion}_{ConfigUtility.Data.ResVersion}";

        string targetFolder = $"{Application.dataPath}/../output_apk/{apkName}_{TimeTool.GetyyyyMMddHHmm(DateTime.Now, "")}";
        if (IOTools.CreateDirectorySafe(targetFolder))
        {
            string locatePathName = $"{targetFolder}/{apkName}.apk";

            BuildPipeline.BuildPlayer(scenes, locatePathName, BuildTarget.Android, BuildOptions.None);
            Debug.Log("build android success:" + locatePathName);

            //--------------->output build details
            //copy cfg file
            string cfgPath = $"{targetFolder}/{ConfigUtility.cfgFileName}.bytes";
            IOTools.CopyCfgFile(cfgPath);
            //log buildMsg
            string buildMsgPath = $"{targetFolder}/buildMsg.txt";
            string buildMsg = "macros:" + EditorPrefs.GetString(targetMacrosKey) + "\r\n";
            buildMsg += "buildScenes:" + getBuildScenesStr() + "\r\n";
            IOTools.WriteString(buildMsgPath, buildMsg);
            Debug.Log("output build details finished：" + targetFolder);
            return targetFolder;
        }
        Debug.LogError("build android failed!!!");
        return "";
    }
    static string[] getBuildScenes()
    {
        List<string> names = new List<string>();

        foreach (EditorBuildSettingsScene e in EditorBuildSettings.scenes)
        {
            if (e == null) { continue; }
            if (e.enabled) { names.Add(e.path); }
        }
        return names.ToArray();
    }
    static string getBuildScenesStr()
    {
        var scenes = getBuildScenes();
        string scenesStr = "";
        for (int i = 0; i < scenes.Length; i++)
        {
            scenesStr += scenes[i] + ";";
            scenesStr += ";";
        }
        scenesStr = scenesStr.TrimEnd(';');
        return scenesStr;
    }
    static void setVersion()
    {
        PlayerSettings.bundleVersion = ConfigUtility.Data.AppVersion;
        Debug.Log("set app version（bundleVersion）:" + ConfigUtility.Data.AppVersion);
#if UNITY_ANDROID
        PlayerSettings.Android.bundleVersionCode = int.Parse(ConfigUtility.Data.AppBundleVersion);
        Debug.Log("set bundleVersionCode:" + PlayerSettings.Android.bundleVersionCode);
#elif UNITY_IOS
        PlayerSettings.iOS.buildNumber =ConfigUtility.Data.AppBundleVersion;
        Debug.LogError("set buildNumber:" + PlayerSettings.iOS.buildNumber);
#endif
    }

    [MenuItem("工具/打包/安卓/设置keystore")]
    public static void SetKeystore()
    {
        string bundleId = "com.xingqimetaverse.an";
        PlayerSettings.Android.keystoreName = Application.dataPath + "/../keystore/" + bundleId + "/user.keystore";
        PlayerSettings.Android.keystorePass = bundleId;
        PlayerSettings.Android.keyaliasName = bundleId;
        PlayerSettings.Android.keyaliasPass = bundleId;
        PlayerSettings.Android.useCustomKeystore = true;
        Debug.Log("keystore设置完成");
    }
    static void setProductName()
    {
        PlayerSettings.productName = ConfigUtility.Data.ProductName;
    }


    public static void SetHybridCLRHotUpdateAssemblies()
    {
#if HybridCLR_INSTALLED
        //TODO:后面改成通过配置表配置
        HybridCLRSettings.Instance.hotUpdateAssemblies = new string[] { "Assembly-CSharp" };
        Debug.Log("set hotUpdateAssemblies:Assembly-CSharp");
        HybridCLRSettings.Save();
#endif
    }

    public static void ClearHybridCLRHotUpdateAssemblies()
    {
#if HybridCLR_INSTALLED
        HybridCLRSettings.Instance.hotUpdateAssemblies = null;
        Debug.Log("clear hotUpdateAssemblies");
        HybridCLRSettings.Save();
#endif

    }


    [MenuItem("工具/打包/拷贝ResEx资源到StreamingAssets")]
    static void copyAllResFilesToStreamingAssets()
    {
        string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        string targetPath = Application.dataPath + "/StreamingAssets/ResEx";
        IOTools.CreateDirectorySafe(targetPath);
        Debug.Log("先删除StreamingAssets/ResEx下所有文件");
        IOTools.DeleteAllFiles(targetPath, true);
        IOTools.MoveFiles(sourcePath, targetPath, true);
        Debug.Log("copy finished");
        AssetDatabase.Refresh();
    }

    static bool copyHotUpdateInitResFilesToStreamingAssets()
    {
        string sourcePath = Application.dataPath + "/../hotupdate/" + IOTools.PlatformFolderName + "/channel_" + ConfigUtility.Data.GameChannelId + "/" + ConfigUtility.Data.AppVersion + "/" + ConfigUtility.Data.ResVersion + "/cur/res";
        if (Directory.Exists(sourcePath) == false)
        {
            Debug.LogError("directory not exist:" + sourcePath);
            return false;
        }
        string targetPath = Application.dataPath + "/StreamingAssets/ResEx";
        IOTools.CreateDirectorySafe(targetPath);

        IOTools.MoveFiles(sourcePath, targetPath, true);

        return true;
    }
    [UnityEditor.Callbacks.DidReloadScripts]
    private static void onScriptDefineSymbolsReset()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            EditorApplication.delayCall = null;
            EditorApplication.delayCall += delayCall;
            return;
        }
        EditorApplication.delayCall = null;
        EditorApplication.delayCall += delayCall;
    }
    static void delayCall()
    {
        var isPack = EditorPrefs.GetInt(isPackKey, -1);
        if (isPack == 1)
        {
            var buildFuncName = EditorPrefs.GetString(buildFuncNameKey);
            var curMacros = EditorPrefs.GetString(curMacrosKey);
            var targetMacros = EditorPrefs.GetString(targetMacrosKey);
            buildWithTargetMacros(buildFuncName, curMacros, targetMacros);
        }
    }
    static void reset()
    {
        BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
        var curMacros = EditorPrefs.GetString(curMacrosKey);

        //delete before reset symbols,in case of onScriptDefineSymbolsReset trigger!
        EditorPrefs.DeleteKey(buildFuncNameKey);
        EditorPrefs.DeleteKey(curMacrosKey);
        EditorPrefs.DeleteKey(targetMacrosKey);
        EditorPrefs.DeleteKey(isPackKey);


        PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.FromBuildTargetGroup(buildTargetGroup), curMacros);
        Debug.Log($"revert macros with：{curMacros}");
    }
}
