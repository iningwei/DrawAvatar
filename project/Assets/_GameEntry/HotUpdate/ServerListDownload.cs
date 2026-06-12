using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//aa 资源serverlist下载
public class ServerListDownload : Singleton<ServerListDownload>
{
    string serverResRootUrl;
    GameEntry gameEntry;

    string serverlistUrl;
    public void Download(GameEntry gameEntry, string serverResRootUrl)
    {
        this.gameEntry = gameEntry;
        this.serverResRootUrl = serverResRootUrl;
        Debug.Log("begin download serverlist");
        //下载serverlist 
        //url后加个时间戳，防止cdn缓存
        serverlistUrl = serverResRootUrl + "/serverlist" + ".json?ts=" + TimeTool.GetNowStamp();
        HybridCLRHttpTool.Get(serverlistUrl, IdAssginer.GetID(IdAssginer.IdType.WWW), onCmp, onFail, fileDownloadUpdate);
    }


    private void fileDownloadUpdate(long downloaded, long downloadLength)
    {
        Debug.Log("serverList download msg:" + downloaded + ", downloadLength:" + downloadLength);
    }


    private void onFail(long id, string errorMsg)
    {
        Debug.LogError("serverlist download failed, id:" + id + ", url:" + serverlistUrl + "====>errorMsg:" + errorMsg);

        //网络错误
        this.gameEntry.ShowErrorTip("网络错误:1");
    }

    private void onCmp(long id, byte[] datas)
    {
        string str = System.Text.Encoding.Default.GetString(datas);
        Debug.Log("serverlist download success,id:" + id + ", content:" + str);

        ServerList.Instance.Init(str);
        Debug.Log("channelId:" + ConfigUtility.Data.GameChannelId);

#if HybridCLR_HOTUPDATE
        //检测是否需要更新app
        HybridCLRAAAppUpdate.Instance.Enter(gameEntry, serverResRootUrl);
#else

        this.gameEntry.EnterGame();
#endif
    }

    private void onTimeout()
    {
        Debug.Log("serverlist download timeout");
    }

    private void onUpdate(float p)
    {
        Debug.Log("serverlist download progress:" + p);
    }
}
