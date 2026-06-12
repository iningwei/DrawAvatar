using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using ZGame.SDK;

public class AnalyticsListener
{
//////    bool isInit = false;
//////    public void Init()
//////    {
//////        if (isInit)
//////        {
//////            return;
//////        }
//////        isInit = true;
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Levelup, this.Main_BtnClick_Levelup);

//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Set, this.Main_BtnClick_Set);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Earning, this.Main_BtnClick_Earning);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Speed, this.Main_BtnClick_Speed);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Workers, this.Main_BtnClick_Workers);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Skins,
//////this.Main_BtnClick_Skins);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Citys, this.Main_BtnClick_Citys);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_BtnClick_Slots, this.Main_BtnClick_Slots);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_AD_BtnClick_MoreWorkers, this.Main_AD_BtnClick_MoreWorkers);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_AD_BtnClick_SuperWorkers, this.Main_AD_BtnClick_SuperWorkers);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_AD_BtnClick_GoldDouble_5m, this.Main_AD_BtnClick_GoldDouble_5m);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_AD_BtnClick_GoldDouble_15m,
//////this.Main_AD_BtnClick_GoldDouble_15m);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Main_AD_BtnClick_Speedup,
//////this.Main_AD_BtnClick_Speedup);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Levelup_BtnClick_Close, this.Levelup_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Levelup_AD_BtnClick_Double, this.Levelup_AD_BtnClick_Double);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Offline_BtnClick_Close, this.Offline_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Offline_AD_BtnClick_Double, this.Offline_AD_BtnClick_Double);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Start_Area, this.Start_Area);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Finish_Area, this.Finish_Area);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GC_Start, this.GC_Start);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GC_Finsish, this.GC_Finish);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Slots_BtnClick_Spin, this.Slots_BtnClick_Spin);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Slots_AD_BtnClick_FreeSpin, this.Slots_AD_BtnClick_FreeSpin);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Slots_BtnClick_Close, this.Slots_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Skin_BtnClick_Get, this.Skin_BtnClick_Get);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Skin_BtnClick_Close, this.Skin_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.City_BtnClick_GetRewards, this.City_BtnClick_GetRewards);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.City_BtnClick_Close, this.City_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Gift_BtnClick_Open, this
//////            .Gift_BtnClick_Open);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Gift_BtnClick_GetGold, this
//////            .Gift_BtnClick_GetGold);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Gift_BtnClick_GetGem, this
//////            .Gift_BtnClick_GetGem);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.Gift_BtnClick_Close, this
//////            .Gift_BtnClick_Close);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GameInst_ShowChance, this.GameInst_ShowChance);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GameInst_ShowSuccess, this.GameInst_ShowSuccess);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GameInst_ShowFail, this.GameInst_ShowFail);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.GameInst_Click, this.GameInst_Click);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_ShowChance, this.RewardVideo_ShowChance);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_ShowSuccess, this.RewardVideo_ShowSuccess);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_ShowFail, this.RewardVideo_ShowFail);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_VideoLoad, this.RewardVideo_VideoLoad);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_VideoLoadFail, this.RewardVideo_VideoLoadFail);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_VideoLoadSucc, this.RewardVideo_VideoLoadSucc);
//////        EventDispatcher.Instance.AddListener(AnalyticsEventID.RewardVideo_Click, this.RewardVideo_Click);
//////    }

//////    private void RewardVideo_Click(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_VideoLoadSucc(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_VideoLoadFail(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_VideoLoad(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_ShowFail(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_ShowSuccess(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void RewardVideo_ShowChance(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void GameInst_Click(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void GameInst_ShowFail(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void GameInst_ShowSuccess(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void GameInst_ShowChance(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Gift_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }
   
//////    private void Gift_BtnClick_GetGem(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }
    
//////    private void Gift_BtnClick_GetGold(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }
  
//////    private void Gift_BtnClick_Open(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }
  
//////    private void City_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void City_BtnClick_GetRewards(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "amount", (int) paras[0]);
//////    }

//////    private void Skin_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "carId", (int)paras[0], "cleanerId", (int) paras[1]);
//////    }

//////    private void Skin_BtnClick_Get(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "type", (int)paras[0], "id", (int) paras[1]);
//////    }

//////    private void Slots_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Slots_AD_BtnClick_FreeSpin(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "result", paras[0].ToString());
//////    }

//////    private void Slots_BtnClick_Spin(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "result", paras[0].ToString());
//////    }

//////    private void GC_Finish(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "GCId", (int)  paras[0]);
//////    }
//////    private void GC_Start(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "GCId", (int)  paras[0]);
//////    }

//////    private void Finish_Area(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", paras[0].ToString(), "time", paras[1].ToString());
//////    }

//////    private void Start_Area(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", paras[0].ToString(), "time", paras[1].ToString());
//////    }

//////    private void Offline_AD_BtnClick_Double(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }

//////    private void Offline_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "CityId", (int) paras[0]);
//////    }

//////    private void Levelup_AD_BtnClick_Double(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "amount", (int) paras[0]);
//////    }

//////    private void Levelup_BtnClick_Close(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "amount", (int) paras[0]);
//////    }

//////    private void Main_AD_BtnClick_Speedup(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }

//////    private void Main_AD_BtnClick_GoldDouble_15m(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }

//////    private void Main_AD_BtnClick_GoldDouble_5m(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }

//////    private void Main_AD_BtnClick_SuperWorkers(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }

//////    private void Main_AD_BtnClick_MoreWorkers(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }

//////    private void Main_BtnClick_Slots(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Main_BtnClick_Citys(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Main_BtnClick_Skins(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Main_BtnClick_Workers(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "W_Level", (int) paras[0], "SceneId", (int) paras[1]);
//////    }

//////    private void Main_BtnClick_Speed(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "S_Level", (int) paras[0], "SceneId", (int) paras[1]);
//////    }

//////    private void Main_BtnClick_Earning(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "E_Level", (int) paras[0], "SceneId", (int) paras[1]);
//////    }

//////    private void Main_BtnClick_Set(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId);
//////    }

//////    private void Main_BtnClick_Levelup(string evtId, object[] paras)
//////    {
//////        AnalyticsSdkManager.Instance.LogEvent(evtId, "Level", (int)paras[0]);
//////    }
}
