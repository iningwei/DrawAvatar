#if HybridCLR_INSTALLED
using HybridCLR.Editor.Commands;
#endif
using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Obfuscation;

public class HotResCollector
{
    int maxZipFileSize = 1 * 1024 * 1024;//最大尺寸1MB
    public string appVersion;
    public string resVersion;
    public string channelId;



    string curResDir;//当前版本对应的全量资源

    string needUpdateDir;
    string needUpdateResDir;//相对于上个版本需要更新的资源
    string needUpdateZipDir;//相对于上个版本需要更新的资源  打成zip后的目录

    string outputDir;
    string outputResDir;

    string getLastResVersionCurResDir()
    {
        return Application.dataPath + "/../hotupdate/" + IOTools.PlatformFolderName + "/channel_" + channelId + "/" + appVersion + "/" + (int.Parse(resVersion) - 1) + "/cur/res";
    }

    bool initDir()
    {
        string resVersionPath = Application.dataPath + "/../hotupdate/" + IOTools.PlatformFolderName + "/channel_" + channelId + "/" + appVersion + "/" + resVersion;
        if (Directory.Exists(resVersionPath))
        {
            Debug.LogError("already exist:" + resVersionPath + ", delete first!");
            Directory.Delete(resVersionPath, true);
        }

        curResDir = resVersionPath + "/cur/res";
        IOTools.CreateDirectorySafe(curResDir);

        needUpdateDir = resVersionPath + "/needUpdate";
        needUpdateResDir = needUpdateDir + "/res";
        needUpdateZipDir = needUpdateDir + "/zip";

        Directory.CreateDirectory(needUpdateDir);
        Directory.CreateDirectory(needUpdateResDir);
        Directory.CreateDirectory(needUpdateZipDir);


        outputDir = resVersionPath + "/output";
        outputResDir = outputDir + "/res";

        Directory.CreateDirectory(outputDir);
        Directory.CreateDirectory(outputResDir);

        return true;
    }


    //根据原始资源和当前资源，获得需要更新的资源
    /// <summary>
    /// 
    /// </summary>
    /// <param name="originPath">原始包使用的资源</param>
    /// <param name="curPath">当前资源</param>
    /// <param name="needUpdatePath">需要更新的资源</param>
    public int ExtractNeedUpdateFiles(string originPath, string curPath, string needUpdatePath)
    {
        if (Directory.Exists(originPath) == false)
        {
            return 0;
        }

        //先清空needUpdatePath
        IOTools.DeleteAllFiles(needUpdatePath, true);

        string[] originFiles = Directory.GetFiles(originPath);
        string[] curFiles = Directory.GetFiles(curPath);

        List<string> targetFiles = new List<string>();
        //curFiles中存在的，而originFiles中不存在的文件，加入目标文件列表中
        //文件名相同的，若md5码不一致，说明需要更新，亦需加入目标文件列表中
        for (int i = 0; i < curFiles.Length; i++)
        {

            string fileName = Path.GetFileName(curFiles[i]);

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);

            if (File.Exists(originPath + "/" + fileName) == false)
            {
                targetFiles.Add(curFiles[i]);
                if (ConfigUtility.Data.IsABResNameCrypto)
                {
                    Debug.Log($"new file:{DES.DecryptStrFromHex(fileNameWithoutExt, ConfigUtility.Data.ABResNameCryptoKey)}, cryptoName:{fileNameWithoutExt}, not exist in {originPath}");
                }
                else
                {
                    Debug.Log($"new file:{fileNameWithoutExt}, not exist in {originPath}");
                }
            }
            else
            {
                //compare md5
                string curMd5 = ZGame.Obfuscation.MD5.Get(File.ReadAllBytes(curFiles[i]));
                string originMd5 = ZGame.Obfuscation.MD5.Get(File.ReadAllBytes(originPath + "/" + fileName));
                if (curMd5 != originMd5)
                {
                    targetFiles.Add(curFiles[i]);
                    if (ConfigUtility.Data.IsABResNameCrypto)
                    {
                        Debug.Log($"changed file:{DES.DecryptStrFromHex(fileNameWithoutExt, ConfigUtility.Data.ABResNameCryptoKey)}, cryptoName:{fileNameWithoutExt} ");
                    }
                    else
                    {
                        Debug.Log($"changed file:{fileNameWithoutExt} ");
                    }
                }
            }
        }



        Debug.Log(" 一共需要更新的文件个数为：" + targetFiles.Count);
        //筛选出的文件输出到needUpdatePath下
        for (int i = 0; i < targetFiles.Count; i++)
        {
            string fileName = Path.GetFileName(targetFiles[i]);

            string filePath = needUpdatePath + "/" + fileName;
            File.Copy(targetFiles[i], filePath);

            string fileName2 = DES.DecryptStrFromHex(Path.GetFileNameWithoutExtension(targetFiles[i]), ConfigUtility.Data.ABResNameCryptoKey);
            Debug.Log("需要更新：" + filePath + ", originFileName:" + fileName2);
        }

        return targetFiles.Count;
    }



    /// <summary>
    /// 需要更新的文件打包为zip
    /// zip文件名格式为：a_版本号_资源号_index.zip
    /// </summary>
    /// <param name="needUpdatePath"></param>
    /// <param name="finalPath"></param>
    void zipFiles(string needUpdatePath, string finalPath)
    {
        IOTools.DeleteAllFiles(finalPath, true);


        int newFileIndex = 0;
        var allFiles = Directory.GetFiles(needUpdatePath);
        ResFileCombo combo = new ResFileCombo();
        int cursize = 0;
        foreach (var f in allFiles)
        {
            //对于子游戏对应的资源，不压zip
            var name = Path.GetFileNameWithoutExtension(f);
            if (ConfigUtility.Data.IsABResNameCrypto)
            {
                var name2 = DES.DecryptStrFromHex(name, ConfigUtility.Data.ABResNameCryptoKey);
                Debug.Log(" zipFile---->开始读取文件:" + f + ", originName:" + name2);
            }
            else
            {
                Debug.Log(" zipFile---->开始读取文件:" + f + ", originName:" + name);
            }

            var data = File.ReadAllBytes(f);

            if (cursize > maxZipFileSize)
            {
                File.WriteAllBytes(Path.GetFullPath(finalPath + "/a_" + appVersion + "_" + resVersion + "_" + (newFileIndex++) + ".zip"), combo.ToBytes());
                combo = new ResFileCombo();
                cursize = 0;
            }
            cursize += data.Length;
            combo.WriteString(Path.GetFileName(f));
            combo.WriteBytes(data);
        }
        if (cursize > 0)
        {
            File.WriteAllBytes(Path.GetFullPath(finalPath + "/a_" + appVersion + "_" + resVersion + "_" + (newFileIndex++) + ".zip"), combo.ToBytes());
        }

    }


    void OutputResFileList(string fileDir, string outputFileName)
    {
        if (!Directory.Exists(fileDir))
        {
            Debug.LogError("error, fileDir not exist:" + fileDir);
            return;
        }
        var files = Directory.GetFiles(fileDir);

        List<string> filesList = new List<string>();
        foreach (var item in files)
        {
            if (item.EndsWith(".zip"))
            {
                filesList.Add(item);
            }
        }

        filesList.Sort((a, b) =>
        {
            var aIndex = int.Parse(Path.GetFileNameWithoutExtension(a).Split('_')[3]);
            var bIndex = int.Parse(Path.GetFileNameWithoutExtension(b).Split('_')[3]);
            return aIndex.CompareTo(bIndex);
        });


        int maxZippedIndex = 0;
        Dictionary<string, object> rootDic = new Dictionary<string, object>();

        List<object> fileObjs = new List<object>();
        foreach (var f in filesList)
        {
            string fileName = Path.GetFileName(f);
            string fileNameNoExt = Path.GetFileNameWithoutExtension(f);
            int index = int.Parse(fileNameNoExt.Split('_')[3]);
            if (index > maxZippedIndex)
            {
                maxZippedIndex = index;
            }

            var fdata = File.ReadAllBytes(f);

            Dictionary<string, object> fileObjDic = new Dictionary<string, object>();

            fileObjDic["name"] = fileName;
            fileObjDic["size"] = fdata.Length;
            fileObjDic["md5"] = MD5.Get(fdata);

            fileObjs.Add(fileObjDic);
        }
        rootDic["maxzippedindex"] = maxZippedIndex.ToString();
        rootDic["list"] = fileObjs;

        File.WriteAllText(fileDir + "/" + outputFileName + ".json", Json.Serialize(rootDic));
    }

    void outputVersionList(string outputFileName, string outputDic, string zipFileDic)
    {
        if (!Directory.Exists(zipFileDic))
        {
            Debug.LogError("error, fileDir not exist:" + zipFileDic);
            return;
        }
        var zippedFiles = Directory.GetFiles(zipFileDic);


        Dictionary<string, object> rootZipDic = new Dictionary<string, object>();

        List<object> zipFileObjs = new List<object>();
        foreach (var f in zippedFiles)
        {
            int index = int.Parse(Path.GetFileNameWithoutExtension(f).Split('_')[3]);

            var fdata = File.ReadAllBytes(f);
            string fileName = Path.GetFileName(f);


            Dictionary<string, object> fileObjDic = new Dictionary<string, object>();

            fileObjDic["name"] = fileName;
            fileObjDic["size"] = fdata.Length;
            fileObjDic["md5"] = MD5.Get(fdata);

            zipFileObjs.Add(fileObjDic);
        }
        rootZipDic["zipfilecount"] = zipFileObjs.Count;
        rootZipDic["ziplist"] = zipFileObjs;


        File.WriteAllText(outputDic + "/" + outputFileName + ".json", Json.Serialize(rootZipDic));
    }


    public void ProcessHotUpdateRes()
    {
        bool r = initDir();
        if (r == false)
        {
            return;
        }

        //把AB导出目录下的文件全部拷贝到cur目录下
        string abPath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
        IOTools.MoveFiles(abPath, curResDir, true);
        //上个资源版本的全量资源目录
        string lastResVersionCurResDic = getLastResVersionCurResDir();
        if (Directory.Exists(lastResVersionCurResDic) == false)
        {
            Debug.LogError($"appVersion:{appVersion},resVersion:{resVersion}, has no need to update comparing last version, for last version not exist, last version dic:{lastResVersionCurResDic}");
            if (resVersion != "0")
            {
                return;
            }
        }

        int count = ExtractNeedUpdateFiles(lastResVersionCurResDic, curResDir, needUpdateResDir);
        if (count == 0)
        {
            Debug.Log("当前无可更新资源！ ");
        }
        zipFiles(needUpdateResDir, needUpdateZipDir);

        outputVersionList("versionlist", outputDir, needUpdateZipDir);

        //把需要更新的zip资源拷贝到最终目录下
        IOTools.MoveFiles(needUpdateZipDir, outputResDir, false);
    }
    public void Build()
    {
#if HybridCLR_HOTUPDATE
        PackTool.SetHybridCLRHotUpdateAssemblies();

        Debug.Log("begin-> HybridCLR Generate All");
        PrebuildCommand.GenerateAll();

        Debug.Log("begin->热更DLL移动到ResEx目录");
        HybridCLRResUpdateTool.MoveHotupdateDll2ResEx();
        AssetDatabase.Refresh();
#else
        PackTool.ClearHybridCLRHotUpdateAssemblies();
#endif


        this.ProcessHotUpdateRes();
    }
    public HotResCollector()
    {
        appVersion = ConfigUtility.Data.AppVersion;
        resVersion = ConfigUtility.Data.ResVersion;
        channelId = ConfigUtility.Data.GameChannelId.ToString();

        Debug.Log($"appVersion:{appVersion}, resVersion:{resVersion}, channelId:{channelId}");
    }
}
