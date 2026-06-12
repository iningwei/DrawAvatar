using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using ZGame.Obfuscation;


//需支持断点下载
//服务器资源可以有多个版本，每个版本都有个filelist，每个flielist都有完整的所有资源清单（有序的文件名，md5，包括配置表（如果需要更新配置表的话））
public class HybridCLRResourceUpdate : Singleton<HybridCLRResourceUpdate>
{
    int curDownLoadIndex = -1;

    //当前要下载的某个版本资源列表
    List<HybridCLRDownLoadZipResFileInfo> resServerFileInfos = new List<HybridCLRDownLoadZipResFileInfo>();
    //当前要下载的某个版本资源的总大小, 单位byte
    public long curHandleResVersionResTotalSize = 0;
    //当前要下载的某个版本资源  已经下载的文件大小
    public long curHandleResVersionResDownloadedSize = 0;

    //服务端资源版本号
    public int serverResVer;
    //当前正在操作的资源版本
    public int curHandleResVersion;
    //开始下载，首次操作的资源版本
    public int startHandleResVersion = -1;

    GameEntry gameEntry;
    /// <summary>
    /// 
    /// </summary>
    public void Enter(GameEntry gameEntry)
    {
        this.gameEntry = gameEntry;

        curDownLoadIndex = -1;


        //检测资源是否需要更新
        var localResVer = int.Parse(GetLocalResVersion());
        serverResVer = int.Parse(ServerList.Instance.GetAppMaxResVersion(ConfigUtility.Data.AppVersion));


        ReadABMsgToDic();



        if (serverResVer > localResVer)
        {
            Debug.Log("res need update,sverResVer:" + serverResVer + ", localResVer：" + localResVer);

            curHandleResVersion = localResVer + 1;
            if (startHandleResVersion == -1)
            {
                startHandleResVersion = curHandleResVersion;
            }
            DownloadFileList(curHandleResVersion);
        }
        else if (serverResVer == localResVer)
        {
            Debug.Log($"res already update to newest res version:{serverResVer},enterGame！");

            EnterGame();
        }
        else
        {
            //sverver<localver也直接进入游戏
            //造成这种情况的原因，比如某用户通过商店下载个资源版本为10的版本，但是ftp那边配置的最大版本是9，并没有及时切换到10
            Debug.LogWarning("Warning：sverver<localver, sverVer:" + serverResVer + ", localVer:" + localResVer);
            EnterGame();
        }
    }


    void EnterGame()
    {
        if (CheckUpdateDirABCompletion())
        {
            Debug.Log("资源更新完毕，进入游戏!");
            this.gameEntry.EnterGame();
        }
        else
        {
            Debug.LogError("error,pppath下ab包完整性检测失败");
            this.gameEntry.ShowErrorTip("客户端完整性检测失败!");
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetLocalResVersion()
    {
        //TODO:资源更新的版本号写到PlayerPrefs中，如果用户覆盖安装了一个低版本的呢，会造成什么影响？？


        //先判断playerprefs是否有版本号，如果没有则从包配置里读取
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        Debug.Log("resversion_" + ConfigUtility.Data.AppVersion + "-->" + "localResVersion：" + localResVersion);
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }

        return localResVersion.ToString();
    }

    public void SetLocalResVersion(string version)
    {
        PlayerPrefs.SetString("resversion_" + ConfigUtility.Data.AppVersion, version);
        PlayerPrefs.Save();
    }

    public int GetLocalResMaxZippedIndex(int resVersion)
    {
        return PlayerPrefs.GetInt("maxzippedindex_" + ConfigUtility.Data.AppVersion + "_" + resVersion, -1);
    }

    public void SetLocalResMaxZippedIndex(int resVersion, int index)
    {
        PlayerPrefs.SetInt("maxzippedindex_" + ConfigUtility.Data.AppVersion + "_" + resVersion, index);
        PlayerPrefs.Save();
    }


    void DownloadFileList(int resVersion)
    {
        Debug.Log("begin download versionlist, resVersion:" + resVersion);
        //获得 versionlist.json中的内容
        var vlUrl = ConfigUtility.GetPackItem().FtpTxtFileURL + "/" + IOTools.PlatformFolderName + "/channel_" + ConfigUtility.Data.GameChannelId + "/" + ConfigUtility.Data.AppVersion + "/" + resVersion + "/versionlist.json";

        HybridCLRHttpTool.Get(vlUrl, IdAssginer.GetID(IdAssginer.IdType.WWW), VersionListDownloadCmp, VersionListDownloadFail);
    }

    private void VersionListDownloadCmp(long id, byte[] datas)
    {
        int zipFileCount = 0;
        int localZippedIndex = GetLocalResMaxZippedIndex(curHandleResVersion);


        resServerFileInfos.Clear();
        var versionList = System.Text.Encoding.Default.GetString(datas);
        Debug.Log("versionlist download finish:" + versionList);
        var flData = Json.Deserialize(versionList) as Dictionary<string, object>;
        zipFileCount = int.Parse(flData["zipfilecount"].ToString());

        Debug.Log("local zippedindex:" + localZippedIndex + ",zipFileCount:" + zipFileCount);


        //----------->

        //////resServerFileInfos.Clear();
        //////var resfileList = System.Text.Encoding.Default.GetString(datas);
        //////DebugExt.Log("resfilelist download finished:" + resfileList); 
        curHandleResVersionResTotalSize = 0;//单位为Byte
        curHandleResVersionResDownloadedSize = 0;

        var list = flData["ziplist"] as List<object>;
        for (int i = 0; i < list.Count; i++)
        {
            var itemDic = list[i] as Dictionary<string, object>;
            var name = itemDic["name"].ToString();
            var index = int.Parse(name.Replace(".zip", "").Split('_')[3]);
            if (index <= localZippedIndex || index > zipFileCount)
            {
                continue;
            }



            var info = new HybridCLRDownLoadZipResFileInfo();
            info.name = name;
            info.index = int.Parse(name.Replace(".zip", "").Split('_')[3]);
            info.md5 = itemDic["md5"].ToString().ToLower();
            info.size = System.Convert.ToInt32(itemDic["size"].ToString());
            curHandleResVersionResTotalSize += info.size;
            info.needDownload = true;
            resServerFileInfos.Add(info);
        }

        //根据索引排序
        resServerFileInfos.Sort((t1, t2) => { return t1.index.CompareTo(t2.index); });

#if UNITY_EDITOR
        Debug.Log("need download res count:" + resServerFileInfos.Count + ",list：");
        for (int i = 0; i < resServerFileInfos.Count; i++)
        {
            Debug.Log(resServerFileInfos[i].name);
        }
#endif

        //////EventDispatcher.Instance.DispatchEvent(EventID.OnBeginDownloadHotResFiles, curHandleResVersionResTotalSize.ToString());

        DownLoadNext();
    }




    /// <summary>
    /// 检测更新目录下AB资源的完整性
    /// </summary>
    /// <returns></returns>
    bool CheckUpdateDirABCompletion()
    {
        Debug.Log("--------> begin check AB Completion");
        //第一步，根据allABMsgDic的数据来逐一比对AB文件数据。
        //第二步，再检测所有的AB文件是否存在于allABMsgDic中
        //两遍检测保证一一对应
        string[] allABFiles = IOTools.getAllABFilesInUpdateDir();
        Debug.Log("--------> total ab res count in hotupdate folder：" + allABFiles.Length);

        //第一步检测
        Debug.Log("--------> first step check");
        foreach (KeyValuePair<string, string> item in allABMsgDic)
        {
            var fileData = IOTools.GetResFileDataFromUpdateDir(item.Key);
            if (fileData == null)
            {
                Debug.LogError("--------> res not complete,no：" + item.Key);
                return false;
            }
            if (MD5.Get(fileData) != item.Value)
            {
                Debug.LogError("--------> res md5 not the same：" + item.Key);
                return false;
            }
        }

        //第二步检测
        Debug.Log("--------> second step check");
        for (int j = 0; j < allABFiles.Length; j++)
        {
            string name = Path.GetFileName(allABFiles[j]);
            if (allABMsgDic.ContainsKey(name) == false)
            {
                Debug.LogError("--------> abmsg.txt not contain：" + name);
                return false;
            }
        }


        Debug.Log("--------> res is complete！！");
        return true;
    }

    void ReadABMsgToDic()
    {
        string[] lines = IOTools.readAllLinesFromUpdateDir("abmsg.txt");
        if (lines != null)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                var datas = lines[i].Split('|');
                allABMsgDic[datas[0]] = datas[1];
            }
        }
    }


    public void UpdateABMsgTxtFile()
    {
        string allStrs = "";
        var keys = new List<string>(allABMsgDic.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            allStrs += (keys[i] + "|" + allABMsgDic[keys[i]]);
            if (i < keys.Count - 1)
            {
                allStrs += "\r\n";
            }
        }

        IOTools.WriteStringToUpdateDir("abmsg.txt", allStrs);
    }

    /// <summary>
    /// 
    /// </summary>
    void DownLoadNext()
    {
        curDownLoadIndex++;
        if (curDownLoadIndex >= resServerFileInfos.Count)
        {
            //写入最新资源版本号            
            SetLocalResVersion(curHandleResVersion.ToString());
            SetLocalResMaxZippedIndex(curHandleResVersion, curDownLoadIndex);
            //更新ABMsgDic中数据到本地
            UpdateABMsgTxtFile();

            Debug.Log("zip hotupdate res downloaded!!!");
            Enter(this.gameEntry);
        }
        else
        {
            var info = resServerFileInfos[curDownLoadIndex];
            if (!info.needDownload)
            {
                DownLoadNext();
            }
            else
            {
                var url = ConfigUtility.GetPackItem().FtpTxtFileURL + "/" + IOTools.PlatformFolderName + "/channel_" + ConfigUtility.Data.GameChannelId + "/" + ConfigUtility.Data.AppVersion + "/" + curHandleResVersion + "/res/" + info.name;

                Debug.Log("begindown：" + info.name);

                HybridCLRHttpTool.Get(url, IdAssginer.GetID(IdAssginer.IdType.WWW), fileDownloadCmp, fileDownloadFail, fileDownloadUpdate);
            }
        }
    }


    ///////// <summary>
    ///////// 当下载失败后，继续下载时调用
    ///////// </summary>
    //////public void ResumeDownload()
    //////{
    //////    DebugExt.Log("ResumeDownload");
    //////    curDownLoadIndex--;
    //////    DownLoadNext();
    //////}



    /// <summary>
    /// 
    /// </summary>
    /// <param name="downloaded">某文件已下载大小</param>
    /// <param name="downloadLength">某文件总大小</param>
    private void fileDownloadUpdate(long downloaded, long downloadLength)
    {
        float p = ((float)downloaded) / downloadLength;
        var info = resServerFileInfos[curDownLoadIndex];
        //////DebugExt.Log("file:" + info.name + ",progress: " + p + ", downloaded:" + downloaded + ", total:" + downloadLength);

        //在当前资源版本中已下载的比例
        float resVersionRatio = (curHandleResVersionResDownloadedSize + downloaded) / ((float)curHandleResVersionResTotalSize);

        this.gameEntry.ShowProgress(resVersionRatio);
    }


    /// <summary>
    /// 记录ab信息
    /// </summary>
    Dictionary<string, string> allABMsgDic = new Dictionary<string, string>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="www"></param>
    void fileDownloadCmp(long id, byte[] datas)
    {
        var info = resServerFileInfos[curDownLoadIndex];
        Debug.Log("res file download finish:" + info.name);
        curHandleResVersionResDownloadedSize += info.size;
        if (info.md5 != MD5.Get(datas))
        {
            Debug.LogError("error, md5 not the same：" + info.name);
            return;
        }


        //解压
        ResFileCombo cb = new ResFileCombo(datas);
        var files = cb.ReadAllFile();
        foreach (var v in files)
        {
            Debug.Log("unzip file：" + v.name);
            allABMsgDic[v.name] = MD5.Get(v.data);
            IOTools.WriteFileToUpdateDir(v.name, v.data);
        }

        //下载下一个文件
        DownLoadNext();
    }

    public void AddKVToABMsgDic(string k, string v)
    {
        allABMsgDic[k] = v;
    }

    /// <summary>
    /// 
    /// </summary>
    void fileDownloadTimeOut()
    {
        var info = resServerFileInfos[curDownLoadIndex];
        Debug.Log("res file download timeout :" + info.name);
    }

    /// <summary>
    /// 
    /// </summary>
    void fileDownloadFail(long id, string errorMsg)
    {
        var info = resServerFileInfos[curDownLoadIndex];
        Debug.LogError("res file download fail:" + info.name);

        //网络错误 
        this.gameEntry.ShowErrorTip("网络错误:2");
    }

    /// <summary>
    /// 
    /// </summary>
    void FileListDownloadTimeOut()
    {
        Debug.Log("fileList download timeout");
    }

    /// <summary>
    /// 
    /// </summary>
    void VersionListDownloadFail(long id, string errorMsg)
    {
        Debug.LogError("fileList download fail,errorMsg:" + errorMsg);
        this.gameEntry.ShowErrorTip("网络错误:3");
    }
}
