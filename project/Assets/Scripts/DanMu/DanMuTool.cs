 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.DanMu;
using ZGame.Window;

public class DanMuTool
{
    public delegate void chatHandler(DanMuChatData data);
    public event chatHandler onChat;

    public delegate void likeHandler(DanMuLikeData likeData);
    public event likeHandler onLike;

    public delegate void giftHandler(DanMuGiftData giftData);
    public event giftHandler onGift;


    Queue<DanMuBaseData> allDanMuDatas = new Queue<DanMuBaseData>();

    public void Process()
    {
        var danMuDatas = DanMuInput.GetDanMuList();
        if (danMuDatas.Count > 0)
        {
            for (int i = 0; i < danMuDatas.Count; i++)
            {
#if NET_DATA_PB
                //掉线状态下非礼物数据则不加入
                if (SocketManager.Instance.IsConnected() == false)
                {
                    if (danMuDatas[i] is DanMuGiftData)
                    {
                        allDanMuDatas.Enqueue(danMuDatas[i]);
                    }
                }
                else
                {
                    allDanMuDatas.Enqueue(danMuDatas[i]);
                }
#else
                allDanMuDatas.Enqueue(danMuDatas[i]);
#endif

            }
        }

#if NET_DATA_PB
        //如果当前处于掉线状态则不处理弹幕数据
        if (SocketManager.Instance.IsConnected() == false)
        {
            return;
        }
        //如果当前S2CRoomLogin没有返回成功，则不处理弹幕
        if (ServiceFetch.roomService.isRcvS2CRoomLoginResult1 == false)
        {
            return;
        }
        //加载缓存礼物数据
        if (isCacheGiftDataLoaded == false && string.IsNullOrEmpty(ServiceFetch.roomService.selfUid) == false)
        {
            this.LoadGiftDataFromCache(ServiceFetch.roomService.selfUid);
        }
#endif

        if (allDanMuDatas.Count > 0)
        {
            var singleData = allDanMuDatas.Dequeue();
            handleDanMuData(singleData);
        }
    }

    void handleDanMuData(DanMuBaseData data)
    {
        if (data is DanMuChatData)
        {
            DanMuChatData cData = data as DanMuChatData;
            if (cData != null)
            {
                onChat(cData);
            }
        }
        if (data is DanMuLikeData)
        {
            DanMuLikeData lData = data as DanMuLikeData;
            if (onLike != null)
            {
                onLike(lData);
            }
        }
        if (data is DanMuGiftData)
        {
            DanMuGiftData gData = data as DanMuGiftData;
            if (onGift != null)
            {
                onGift(gData);
            }
        }
    }


    bool isCacheGiftDataLoaded = false;
    string giftDataCacheFileName = "GiftCacheData";
    public void LoadGiftDataFromCache(string zuboUid)
    {
        try
        {
            string fileName = CryptoTool.DesEncryptStrWithHex(giftDataCacheFileName + zuboUid, ConfigUtility.Data.ABResNameCryptoKey);
            string txtContent = IOTools.ReadStringFromSettingDir($"{fileName}.txt");
            if (string.IsNullOrEmpty(txtContent))
            {
                isCacheGiftDataLoaded = true;
                return;
            }

            var jsonData = CryptoTool.DesDecryptStrFromHex(txtContent, ConfigUtility.Data.ABResNameCryptoKey);
            var datas = JsonUtility.FromJson<List<DanMuGiftData>>(jsonData);
            
            if (datas != null && datas.Count > 0)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    DanMuInput.AddDanMuData(datas[i]);
                }
            }
            else
            {
                Debug.LogError("danmu gift data null");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("LoadGiftDataFromCache error, ex:" + ex.ToString());
        }
        isCacheGiftDataLoaded = true;
    }

    public void SaveGiftDataWhileAppQuit(string zuboUid)
    {
        try
        {
            List<DanMuBaseData> list = new List<DanMuBaseData>(allDanMuDatas);
            List<DanMuGiftData> giftList = new List<DanMuGiftData>();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] is DanMuGiftData gift)
                {
                    giftList.Add(gift);
                }
            }

            string jsonData = "";
            string fileName = CryptoTool.DesEncryptStrWithHex(giftDataCacheFileName + zuboUid, ConfigUtility.Data.ABResNameCryptoKey);
            if (giftList.Count > 0)
            {
                jsonData = JsonUtility.ToJson(giftList);
                
            }
#if UNITY_EDITOR
            Debug.LogError("begin write DanMuGiftJsonData:" + jsonData + " , to " + fileName);
#endif
            jsonData = CryptoTool.DesEncryptStrWithHex(jsonData, ConfigUtility.Data.ABResNameCryptoKey);
            IOTools.WriteStringToSettingDir($"{fileName}.txt", jsonData);
        }
        catch (System.Exception ex)
        {
            Debug.LogError("SaveGiftDataWhileAppQuit error, " + ex.ToString());
        }

    }
}
