using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ZGame;
using ZGame.DanMu;
using ZGame.Event;
using ZGame.Window;
using ZGame.ZTable;


/// <summary>
/// 机器人快捷键功能
/// </summary>
public class RobotShortcut : SingletonMonoBehaviour<RobotShortcut>
{
    /// <summary>
    /// true为使用中  false为未使用
    /// </summary>
    public Dictionary<string, bool> robots = new Dictionary<string, bool>();

    /// <summary>
    /// 在线机器人数量
    /// </summary>
    long robotOnLineNumber { get { return robots.Count(x => x.Value); } }

    bool initFlag = false;
    public void Init()
    {
        if (initFlag) return;
        initFlag = true;

        var map = RobotDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            robots.Add(item.Value.ID, false);
        }

        EventDispatcher.Instance.AddListener(EventID.OnKickDaddy, this.onKickDaddy);
        EventDispatcher.Instance.AddListener(EventID.OnDaddyJoin, this.onDaddyJoin);
        Debug.LogError("init robotShortcut");
    }

    private void onKickDaddy(string evtId, object[] paras)
    {
        string uid = paras[0].ToString();

        if (robots.ContainsKey(uid))
        {
            robots[uid] = false;
            Debug.LogError("移除预设机器人：" + uid + ", 剩余预设机器人总数：" + robotOnLineNumber);
        }

    }

    private void onDaddyJoin(string evtId, object[] paras)
    {
        string uid = paras[0].ToString();

        if (robots.ContainsKey(uid) == false)
        {
            WindowUtil.ShowTip($"加入的机器人 {uid} 不在配置表中");
            return;
        }
        Debug.Log($"daddy {uid} joined,更新robot信息");
        robots[uid] = true;
    }

    public string getValidRobotUId(long minIndex = 0, long maxIndex = 1000)
    {
        List<string> list = new List<string>();
        var keys = robots.Keys.ToList();
        for (int i = 0; i < keys.Count; i++)
        {
            string key = keys[i];
            if (i >= minIndex && i <= maxIndex && robots[key] == false)
            {
                list.Add(key);
            }
        }
        if (list.Count == 0)
        {
            WindowUtil.ShowTip($"{minIndex} - {maxIndex}区间机器人ID已用尽！");
            return "";
        }
        int index = RandomTool.NextInt(0, list.Count);
        string id = list[index];
        return id;
    }


    public void AddRobot(string uid)
    {
        string cmd = BeanManager.Instance.GetJoinCMDStr();

        this.addJoinDanMuData(uid, cmd);
    }



    void addRandomRobot()
    {
        string targetUId = this.getValidRobotUId();
        if (targetUId == "")
        {
            return;
        }
        this.AddRobot(targetUId);
    }

    void addJoinDanMuData(string targetUId, string cmd)
    {
        RobotData robotData = BeanManager.Instance.GetRobotDataByUID(targetUId);
        DanMuInput.AddDanMuData(new DanMuChatData(targetUId.ToString(), robotData.nickName, robotData.avatarUrl, TimeTool.GetNowStamp(), cmd, true));
    }



    void robotSendTargetGift(long campType, int summonDataId, int giftCount)
    {
        List<string> specificCmapDaddyIds = new List<string>();
        var daddyDic = GameLogic.Instance.daddyManager.daddyDic;
        foreach (var item in daddyDic)
        {
            if (item.Value.campType == campType)
            {
                specificCmapDaddyIds.Add(item.Key);
            }
        }

        //随机选一个
        int count = specificCmapDaddyIds.Count;
        if (count > 0)
        {
            var v = RandomTool.NextInt(0, count);
            var uid = specificCmapDaddyIds[v];
            this.SendGift(uid, summonDataId, giftCount);
        }
        else
        {
            Debug.LogError($"阵营{campType}无robot，无法送礼");
        }
    }

    public void SendGift(string uid, int summonDataId, long giftCount)
    {


        var data = BeanManager.Instance.GetRobotDataByUID(uid);
        if (data == null)
        {
            Debug.LogError("送礼对象不是机器人，id：" + uid);
            return;
        }
        var summonData = BeanManager.Instance.GetSummonData(summonDataId);
        var giftData = BeanManager.Instance.GetGiftData(summonData.giftId);
        var danmuGiftData = new DanMuGiftData(uid, data.nickName, data.avatarUrl, TimeTool.GetNowStamp(), giftData.sec_gift_id, giftCount, (int)giftData.gift_value * giftCount, true);
        DanMuInput.AddDanMuData(danmuGiftData);
    }

    public void SendLike(string uid, long likeCount)
    {
        if (!GameLogic.Instance.daddyManager.Exist(uid))
        {
            WindowUtil.ShowTip(uid + "未加入玩法");
            return;
        }

        var data = BeanManager.Instance.GetRobotDataByUID(uid);
        if (data == null)
        {
            Debug.LogError("送礼对象不是机器人，id：" + uid);
            return;
        }
        var danmuLikeData = new DanMuLikeData(uid, data.nickName, data.avatarUrl, TimeTool.GetNowStamp(), likeCount, true);
        DanMuInput.AddDanMuData(danmuLikeData);
    }

    public void SendEquipHero(string uid, long unitId)
    {
        if (!GameLogic.Instance.daddyManager.Exist(uid))
        {
            WindowUtil.ShowTip(uid + "未加入玩法");
            return;
        }
        var data = BeanManager.Instance.GetRobotDataByUID(uid);
        if (data == null)
        {
            Debug.LogError("送礼对象不是机器人，id：" + uid);
            return;
        }

        string cmd = BeanManager.Instance.GetEquipHeroCMDStr();
        var danmuChatData = new DanMuChatData(uid, data.nickName, data.avatarUrl, TimeTool.GetNowStamp(), cmd + unitId, true);
        DanMuInput.AddDanMuData(danmuChatData);
    }
    void Update()
    {
        if (!initFlag)
        {
            return;
        }

        if (WindowManager.Instance.IsWindowOpen(WindowNames.LoginWindow) || WindowManager.Instance.IsWindowOpen(WindowNames.LoadingWindow))
        {
            return;
        }


        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                //if (WindowManager.Instance.IsWindowOpen(WindowNames.GMWindow) == false)
                //{
                //    WindowManager.Instance.ShowWindow(WindowNames.GMWindow, WindowLayer.Hud2, false, false, true, null);
                //}
                //else
                //{
                //    WindowManager.Instance.CloseWindow(WindowNames.GMWindow);
                //}
            }



            ////////加入 
            //////if (Input.GetKeyDown(KeyCode.J))
            //////{
            //////    this.addRandomRobot(CampType.Red);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.K))
            //////{
            //////    this.addRandomRobot(CampType.Blue);
            //////}


            ////////送礼
            //////if (Input.GetKeyDown(KeyCode.Alpha1))
            //////{
            //////    //for (int i = 0; i < 250; i++)
            //////    {
            //////        analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha1);
            //////    }
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Alpha2))
            //////{
            //////    analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha2);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Alpha3))
            //////{
            //////    analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha3);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Alpha4))
            //////{
            //////    analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha4);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Alpha5))
            //////{
            //////    analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha5);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Alpha6))
            //////{
            //////    analysisSendGiftShortcut(CampType.Red, KeyCode.Alpha6);
            //////}

            //////if (Input.GetKeyDown(KeyCode.Q))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.Q);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.W))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.W);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.E))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.E);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.R))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.R);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.T))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.T);
            //////}
            //////else if (Input.GetKeyDown(KeyCode.Y))
            //////{
            //////    analysisSendGiftShortcut(CampType.Blue, KeyCode.Y);
            //////}
        }
    }
    void analysisSendGiftShortcut(long campType, KeyCode keyCode)
    {
        int codeASCII = (int)keyCode;
        var data = BeanManager.Instance.GetSummonDataByKeyShortcut(codeASCII);
        if (data != null)
        {
            robotSendTargetGift(campType, data.ID, 1);
        }
    }

    public override void OnDestroy()
    {
        initFlag = false;
    }
}
