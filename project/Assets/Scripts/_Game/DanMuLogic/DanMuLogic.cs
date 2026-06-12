 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZGame;
using ZGame.DanMu;
using ZGame.Event;
using ZGame.ZTable;

public class DanMuLogic : SingletonMonoBehaviour<DanMuLogic>
{
    public DanMuTool danMuTool;

    public void Init()
    {
        danMuTool = new DanMuTool();
        danMuTool.onChat += danMuTool_onChat;
        danMuTool.onLike += danMuTool_onLike;
        danMuTool.onGift += danMuTool_onGift;
    }

    private void danMuTool_onGift(DanMuGiftData giftData)
    {
        var giftBean = BeanManager.Instance.GetGiftDataBySecId(giftData.giftId);

         
#if UNITY_EDITOR//编辑器下，不分平台，都要上传礼物信息
        
#else
 //快手平台礼物是服务端推送的，无需再告诉服务端
        //抖音礼物是本地sdk直推的，需要先告诉服务器
        if (ConfigUtility.Data.GameChannelId == (long)GameChannelId.DouYin)
        {
            
            //向服务器推送  //要先ReqLiveGift，再ReqUserJoin，服务器那边才能处理
            ServiceFetch.roomService.ReqLiveGift(pbLiveGift);
        }
#endif



        //送礼自动加入 
        if (!GameLogic.Instance.daddyManager.Exist(giftData.uid))
        {
            this.UserJoin(giftData.uid,  giftData.nickName, giftData.headerUrl, (uid) =>
            {
                EventDispatcher.Instance.DispatchEvent(EventID.OnDaddySendGift, uid, giftData);
            });
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(EventID.OnDaddySendGift, giftData.uid, giftData);
        }
    }

    private void danMuTool_onLike(DanMuLikeData likeData)
    {
        
        if (ConfigUtility.Data.GameChannelId == (long)GameChannelId.DouYin)
        { 
        }

        //点赞自动加入
        if (!GameLogic.Instance.daddyManager.Exist(likeData.uid))
        {
            //取消点赞加入 20250610
            //if (GameLogic.Instance.daddyManager.IsReachCountLimit() == false)
            //{
            //    if (GameLogic.Instance.daddyManager.Exist(likeData.uid) == false)
            //    {
            //        this.UserJoin(likeData.uid, likeData.nickName, likeData.headerUrl, "", null);
            //    }
            //}
        }
        else
        {
            EventDispatcher.Instance.DispatchEvent(EventID.OnDaddyLike, likeData.uid, likeData.likeCount);
            //////long count = likeData.likeCount / 10;
            //////PlayerGiftEffectInfo giftEffectInfo = new PlayerGiftEffectInfo(likeData.nickName, likeData.uid, "0", count, true, false);
            //////EventDispatcher.Instance.DispatchEvent(EventID.OnPlayerGiftEffect, giftEffectInfo); 
        }
    }
    Dictionary<string, long> labaCdDic = new Dictionary<string, long>();
    Dictionary<string, long> equipHeroCdDic = new Dictionary<string, long>();

    private void danMuTool_onChat(DanMuChatData data)
    {
        //小写化处理
        data.content = data.content.Trim().ToLower();
        

        Debug.Log("danmu chat content:" + data.content);
        //加入指令 
        string realCmd = "";
        if (GameGlobal.Instance.CheckContentSuitCmds(data.content, BeanManager.Instance.DanMuJoinCommands, out realCmd))
        {
            if (GameLogic.Instance.daddyManager.Exist(data.uid) == false)
            {
                //向服务器推送 
                if (ConfigUtility.Data.GameChannelId == (long)GameChannelId.DouYin)
                {
                    
                }
                else if (ConfigUtility.Data.GameChannelId == (long)GameChannelId.KuaiShou)
                {
#if UNITY_EDITOR
                    
#endif
                }
                 
                this.UserJoin(data.uid,  data.nickName, data.headerUrl, null);

            }
        }

        //查询指令
        if (GameGlobal.Instance.CheckContentSuitCmds(data.content, BeanManager.Instance.DanMuQueryCommands, out realCmd))
        {
            string uid = data.uid;
            if (GameLogic.Instance.daddyManager.Exist(uid))
            {
                QueryPreviewManager.Instance.AddPreviewData(uid);
            }
        }

        //喇叭指令
         

        //上阵英雄指令 
       
         

    }

    Dictionary<string, float> joinTimeDic = new Dictionary<string, float>();

    //camp, 1红，2蓝
    private void UserJoin(string uid,  string nickName, string headerUrl, Action<string> joinCallback)
    { 
            //玩家加入成功后，第一时间自动请求玩家信息
            //////GameGlobal.Instance.HandleWithPetData(uid, (pbUserBase) =>
            //////{

            //////    if (joinTimeDic.ContainsKey(uid) == true)
            //////    {
            //////        if (Time.time - joinTimeDic[uid] < 2)  //2秒内不能连续请求加入，否则有可能出现连续点赞的时候触发两次加入,UI 会给出多次提示
            //////        {
            //////            DebugExt.LogE("忽略多次加入");
            //////            return;
            //////        }
            //////    }
            //////    if (joinTimeDic.ContainsKey(uid) == false)
            //////    {
            //////        joinTimeDic.Add(uid, Time.time);
            //////    }
            //////    joinTimeDic[uid] = Time.time;

                 

            //////    //更新daddy列表
            //////    EventDispatcher.Instance.DispatchEvent(EventID.OnDaddyJoin, uid, camp, nickName, headerUrl);
            //////    joinCallback?.Invoke(uid);

            //////    EventDispatcher.Instance.DispatchEvent(EventID.OnScrollMsgTip, uid, "加入了玩法");

            //////    //玩家第一次加入，弹Query提示
            //////    QueryPreviewManager.Instance.AddPreviewData(uid);
            //////}); 
    }


    private void Update()
    {
        if (danMuTool != null)
        {
            danMuTool.Process();
        }
    }

    public override void OnDestroy()
    {
        danMuTool.onChat -= danMuTool_onChat;
        danMuTool.onLike -= danMuTool_onLike;
        danMuTool.onGift -= danMuTool_onGift;
    }
}
