using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.SDK
{
    ////////Iron每请求5次，再请求1次FB

    ////////FB广告每天还有请求上限
    //////public class FBAndIronAdSdkManager : AdSdkBase
    //////{
    //////    public IronSourceAdSdkManager ironSourceMgr;
    //////    public FBAdSdkManager fbADMgr;
    //////    DateTime lastAdTime = new DateTime();

    //////    //本地存储fb某一天播放次数的key
    //////    string fbKey = "";

    //////    //广告播放次数
    //////    int adPlayedTimes = 0;

    //////    public override void Init()
    //////    {

    //////        lastAdTime = DateTime.UtcNow;
    //////        ironSourceMgr.Init();
    //////        fbADMgr.Init();

    //////        int year = DateTime.Now.Year;
    //////        int month = DateTime.Now.Month;
    //////        int day = DateTime.Now.Day;
    //////        fbKey = year.ToString() + month.ToString() + day.ToString();
    //////    }

    //////    bool IsFBTriggerByIronTimes()
    //////    {
    //////        if (adPlayedTimes % 5 == 0)
    //////        {
    //////            return true;
    //////        }
    //////        return false;
    //////    }

    //////    int adType = 0;//0表示没有可用广告，1为fb，2为iron
    //////    public override bool IsRewardedVideoAvailable()
    //////    {
    //////        int count = PlayerPrefs.GetInt(fbKey, 0);

    //////        int fbLimitCount = FirebaseSdkManager.Instance.GetRemoteConfigPara(RemoteConfigUtil.GetConfigStr(RemoteConfig.FbRequestLimit), int.Parse(RemoteConfigUtil.GetConfigValue(RemoteConfig.FbRequestLimit).ToString()));
    //////        if (count < fbLimitCount && IsFBTriggerByIronTimes())
    //////        {
    //////            if (fbADMgr.IsRewardedVideoAvailable())
    //////            {
    //////                DebugExt.Log("use FB ad, usedCount:" + count + ", fbLimitCount:" + fbLimitCount + ",adPlayedTimes:" + adPlayedTimes.ToString());
    //////                PlayerPrefs.SetInt(fbKey, count + 1);
    //////                PlayerPrefs.Save();
    //////                adPlayedTimes++;
    //////                adType = 1;
    //////                return true;
    //////            }
    //////        }

    //////        if (ironSourceMgr.IsRewardedVideoAvailable())
    //////        {
    //////            DebugExt.Log("use iron ad, usedCount:" + count + ", fbLimitCount:" + fbLimitCount + ",adPlayedTimes:" + adPlayedTimes.ToString());
    //////            adPlayedTimes++;
    //////            adType = 2;
    //////            return true;
    //////        }
    //////        adType = 0;
    //////        return false;
    //////    }

    //////    public override void ShowRewardedVideo(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
    //////    {


    //////        if (adType == 1)
    //////        {
    //////            lastAdTime = DateTime.UtcNow;
    //////            fbADMgr.ShowRewardedVideo(onNotAvailable, onShowSuccess, onShowFail);
    //////            return;
    //////        }
    //////        if (adType == 2)
    //////        {
    //////            lastAdTime = DateTime.UtcNow;
    //////            ironSourceMgr.ShowRewardedVideo(onNotAvailable, onShowSuccess, onShowFail);
    //////        }
    //////    }






    //////    public override void ShowBanner()
    //////    {

    //////        ironSourceMgr.ShowBanner();
    //////    }

    //////    public override void HideBanner()
    //////    {
    //////        ironSourceMgr.HideBanner();
    //////    }
    //////}
}