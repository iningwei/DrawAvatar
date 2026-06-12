using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZGame.DanMu;

public class GameLogic : SingletonMonoBehaviour<GameLogic>
{
    public DaddyManager daddyManager;

    public bool isUseWaiGua = false;
    public void Init()
    {
        daddyManager = new DaddyManager();


        DanMuLogic.Instance.Init();
        if (BeanManager.Instance.GetConfigBool(100000))
        {
            RobotShortcut.Instance.Init();
        }
        else
        {
#if UNITY_EDITOR
            RobotShortcut.Instance.Init();
#else
  if (ConfigUtility.Data.LoginType == LoginType.DEV)
            {
                RobotShortcut.Instance.Init();
            }
#endif 
        }

        //抖音弹幕直推
        if (ConfigUtility.Data.GameChannelId == (int)GameChannelId.DouYin)
        {
            //DouYinDanMuByteDirectManager.Instance.onRcvChatMessage += Instance_onRcvChatMessage;
            //DouYinDanMuByteDirectManager.Instance.onRcvLikeMessage += Instance_onRcvLikeMessage;
            //DouYinDanMuByteDirectManager.Instance.onRcvGiftMessage += Instance_onRcvGiftMessage;
            //DouYinDanMuByteDirectManager.Instance.StartByteDirect(BeanManager.Instance.GetConfigStr(3001));
        }
    }
    private void Instance_onRcvGiftMessage(string id, string nick, string headUrl, long timeStamp, string secGiftId, long giftNum, long giftTotalValue, bool isTest)
    {
        DanMuInput.AddDanMuData(new DanMuGiftData(id, nick, headUrl, timeStamp, secGiftId, giftNum, giftTotalValue, isTest));
    }

    private void Instance_onRcvLikeMessage(string id, string nick, string headUrl, long timeStamp, long likeCount, bool isTest)
    {
        DanMuInput.AddDanMuData(new DanMuLikeData(id, nick, headUrl, timeStamp, likeCount, isTest));
    }

    private void Instance_onRcvChatMessage(string id, string nick, string headUrl, long timeStamp, string content, bool isTest)
    {
        DanMuInput.AddDanMuData(new DanMuChatData(id, nick, headUrl, timeStamp, content, isTest));
    }


    private void Update()
    {
        this.daddyManager.Update();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        this.daddyManager.Dispose();
    }
}
