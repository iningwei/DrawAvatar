using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsEventID
{

    #region 主界面
    //点击升级按钮
    public const string Main_BtnClick_Levelup = "Main_BtnClick_Levelup";

    //点击设置按钮
    public const string Main_BtnClick_Set = "Main_BtnClick_Set";

    //升级Earning，成功后上报
    public const string Main_BtnClick_Earning = "Main_BtnClick_Earning";
    //升级Speed，成功后上报
    public const string Main_BtnClick_Speed = "Main_BtnClick_Speed";
    //升级Worker，成功后上报
    public const string Main_BtnClick_Workers = "Main_BtnClick_Workers";

    //点击皮肤按钮
    public const string Main_BtnClick_Skins = "Main_BtnClick_Skins";
    //点击城市按钮
    public const string Main_BtnClick_Citys = "Main_BtnClick_Citys";
    //点击老虎机按钮
    public const string Main_BtnClick_Slots = "Main_BtnClick_Slots";

    //获得临时工 buffer
    public const string Main_AD_BtnClick_MoreWorkers = "Main_AD_BtnClick_MoreWorkers";
    //获得大力工 buffer
    public const string Main_AD_BtnClick_SuperWorkers = "Main_AD_BtnClick_SuperWorkers";
    //获得5分钟的金币翻倍加成buffer
    public const string Main_AD_BtnClick_GoldDouble_5m = "Main_AD_BtnClick_GoldDouble_5m";
    //获得15分钟的金币翻倍加成buffer
    public const string Main_AD_BtnClick_GoldDouble_15m = "Main_AD_BtnClick_GoldDouble_15m";
    //获得3分钟加速buffer
    public const string Main_AD_BtnClick_Speedup = "Main_AD_BtnClick_Speedup";
    #endregion

    #region 升级界面
    //升级界面 点击关闭按钮
    public const string Levelup_BtnClick_Close = "Levelup_BtnClick_Close";
    //升级界面 获得广告奖励
    public const string Levelup_AD_BtnClick_Double = "Levelup_AD_BtnClick_Double";

    #endregion

    #region 离线收益界面

    //离线收益界面 点击关闭按钮
    public const string Offline_BtnClick_Close = "Offline_BtnClick_Close";
    //离线收益界面 获得广告奖励
    public const string Offline_AD_BtnClick_Double = "Offline_AD_BtnClick_Double";
    #endregion

    #region 场景
    //第一次进入一个城市时
    public const string Start_Area = "Start_Area";
    //完成某个城市
    public const string Finish_Area = "Finish_Area";
    #endregion

    #region 垃圾处理玩法
    public const string GC_Start = "GC_Start";
    public const string GC_Finsish = "GC_Finsish";
    #endregion
    #region 老虎机
    //花费钻石Spin按钮点击
    public const string Slots_BtnClick_Spin = "Slots_BtnClick_Spin";
    //免费Spin成功
    public const string Slots_AD_BtnClick_FreeSpin = "Slots_AD_BtnClick_FreeSpin";
    //关闭Slots界面
    public const string Slots_BtnClick_Close = "Slots_BtnClick_Close";
    #endregion
    #region 皮肤
    //点击Get按钮
    public const string Skin_BtnClick_Get = "Skin_BtnClick_Get";
    //关闭按钮
    public const string Skin_BtnClick_Close = "Skin_BtnClick_Close";
    #endregion
    #region 城市界面
    //点击GetReward按钮
    public const string City_BtnClick_GetRewards = "City_BtnClick_GetRewards";
    //关闭按钮
    public const string City_BtnClick_Close = "City_BtnClick_Close";
    #endregion

    #region 限时奖励

    //点击Gift按钮
    public const string Gift_BtnClick_Open = "Gift_BtnClick_Open";
  
    //获得金币奖励
    public const string Gift_BtnClick_GetGold = "Gift_BtnClick_GetGold";
  
    //获得钻石奖励
    public const string Gift_BtnClick_GetGem = "Gift_BtnClick_GetGem";
    
    //点击关闭按钮
    public const string Gift_BtnClick_Close = "Gift_BtnClick_Close";

    #endregion
    
    #region 广告通用
    //插屏 触发展示  
    public const string GameInst_ShowChance = "GameInst_ShowChance";
    //插屏 成功展示  
    public const string GameInst_ShowSuccess = "GameInst_ShowSuccess";
    //插屏 展示失败 
    public const string GameInst_ShowFail = "GameInst_ShowFail";
    //插屏 用户点击广告界面
    public const string GameInst_Click = "GameInst_Click";

    //激励视频 触发展示
    public const string RewardVideo_ShowChance = "RewardVideo_ShowChance";
    //激励视频  成功展示
    public const string RewardVideo_ShowSuccess = "RewardVideo_ShowSuccess";
    //激励视频 展示失败
    public const string RewardVideo_ShowFail = "RewardVideo_ShowFail";
    //激励视频 开始加载
    public const string RewardVideo_VideoLoad = "RewardVideo_VideoLoad";
    //激励视频 加载失败
    public const string RewardVideo_VideoLoadFail = "RewardVideo_VideoLoadFail";
    //激励视频 加载成功
    public const string RewardVideo_VideoLoadSucc = "RewardVideo_VideoLoadSucc";
    //激励视频 用户点击广告界面
    public const string RewardVideo_Click = "RewardVideo_Click";
    #endregion

}
