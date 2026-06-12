using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Obfuscation;

public class HybridCLRResUpdateTool
{
    public static string hybridCLRPlatformFolderName
    {
        get
        {
            string name = "";
#if UNITY_ANDROID
            name = "Android";
#elif UNITY_IOS
             name = "iOS";
#elif UNITY_STANDALONE_WIN
            name = "StandaloneWindows64";
#elif UNITY_WEBGL
            name = "WebGL";
#elif UNITY_STANDALONE_OSX
            Debug.LogError("TODO:set name!!");
#else
            Debug.LogError("TODO:set name!!");     
#endif
            return name;
        }

    }

    //目前热更dll和元数据dll都是做了字节头偏移的

    [MenuItem("HybridCLRHotUpdate/移动待更新DLL到ResEX目录")]
    public static void MoveHotupdateDll2ResEx()
    {
        string fileName = "Assembly-CSharp.dll";
        string fileNameFinal = DES.EncryptStrToHex(fileName, ConfigUtility.Data.ABResNameCryptoKey);
        string sourcePath = Application.dataPath + string.Format("/../HybridCLRData/HotUpdateDlls/{0}/", hybridCLRPlatformFolderName) + fileName;



        string destPath = Application.dataPath + string.Format("/../ResEx/{0}/", IOTools.PlatformFolderName) + fileNameFinal + ".bytes";

        //先删除
        if (File.Exists(destPath))
        {
            File.Delete(destPath);
        }
        //拷贝并 增加字节头 
        IOTools.CopyAndAddBytesForFile(sourcePath, destPath, ConfigUtility.Data.ABResByteOffset);

        //////File.Copy(sourcePath, destPath, true);
        Debug.Log($"copy {sourcePath} to {destPath}");
        AssetDatabase.Refresh();
    }

    [MenuItem("HybridCLRHotUpdate/移动待更新DLL到StreamingAssets or StreamingAssetsHotupdateDll  目录")]
    public static void MoveHotupdateDll2StreamingAssets()
    {
        string fileName = "Assembly-CSharp.dll";
        string fileNameFinal = (ConfigUtility.Data.IsABResNameCrypto ? HybridCLRDES.EncryptStrToHex(fileName, ConfigUtility.Data.ABResNameCryptoKey) : fileName);
        string sourcePath = Application.dataPath + string.Format("/../HybridCLRData/HotUpdateDlls/{0}/", hybridCLRPlatformFolderName) + fileName;

        string destPath;
        if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.AssetBundle)
        {
            destPath = Application.streamingAssetsPath + "/" + fileNameFinal + ".bytes";
        }
        else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
        {
            IOTools.CreateDirectorySafe(Application.streamingAssetsPath + "HotupdateDll");
            destPath = Application.streamingAssetsPath + "HotupdateDll/" + fileNameFinal + ".bytes";
        }
        else
        {
            Debug.LogError("cur action not support resLoadType:" + ConfigUtility.Data.ResLoadType);
            return;
        }
        IOTools.CopyAndAddBytesForFile(sourcePath, destPath, ConfigUtility.Data.ABResByteOffset);
        Debug.Log($"copy {sourcePath} to {destPath}");
        AssetDatabase.Refresh();
    }

    [MenuItem("HybridCLRHotUpdate/移动内置匹配当前平台的DLL到StreamingAssets目录")]
    static void MoveInnerDll2StreamingAssets()
    {
        List<string> aotMetaAssemblyFiles = AOTMetaAssembly.GetOriginDllNames();

        List<string> sourcePaths = new List<string>();
        for (int i = 0; i < aotMetaAssemblyFiles.Count; i++)
        {
            sourcePaths.Add($"{Application.dataPath}/../HybridCLRData/AssembliesPostIl2CppStrip/{hybridCLRPlatformFolderName}/{aotMetaAssemblyFiles[i]}");
        }

        List<string> desPaths = new List<string>();
        for (int i = 0; i < aotMetaAssemblyFiles.Count; i++)
        {
            string finalName = (ConfigUtility.Data.IsABResNameCrypto ? HybridCLRDES.EncryptStrToHex(aotMetaAssemblyFiles[i], ConfigUtility.Data.ABResNameCryptoKey) : aotMetaAssemblyFiles[i]) + ".bytes";
            desPaths.Add($"{Application.streamingAssetsPath}/{finalName}");
        }


        for (int i = 0; i < sourcePaths.Count; i++)
        {
            //拷贝并 增加字节头 
            IOTools.CopyAndAddBytesForFile(sourcePaths[i], desPaths[i], ConfigUtility.Data.ABResByteOffset);
            Debug.Log($"copy {sourcePaths[i]} to {desPaths[i]}");
        }
        AssetDatabase.Refresh();
    }

}
