using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.SDK
{
    public class FBAdSdkManager : AdSdkBase
    {
        //////string fbRewardVideoId = "323353782055274_323359478721371";
        ////////string fbBannerADId = "323353782055274_335693814154604";



        //////public static bool isDoPreloadingRewardedVideo = false;
        //////public static bool isRewardedVideoLoaded = false;
        //////public static RewardedVideoAd rewardedVideoAd = null;

        //////FBRewardedVideoListener fbRewardedVideoListener = new FBRewardedVideoListener();




        //////public override void Init()
        //////{
        //////    DebugExt.Log("FBManager ad Init");
        //////    if (isInit)
        //////    {
        //////        return;
        //////    }

        //////    AudienceNetworkAds.Initialize();

        //////    //10秒后开始预加载
        //////    StartCoroutine(DelayPreload());
        //////}
        //////IEnumerator DelayPreload()
        //////{
        //////    yield return new WaitForSeconds(10);
        //////    PreLoadRewardedVideo();
        //////}

        //////void PreLoadRewardedVideo()
        //////{
        //////    if (AudienceNetworkAds.IsInitialized() == false)
        //////    {
        //////        Debug.LogError("FB AD not init success,can not do preload");
        //////        return;
        //////    }
        //////    if (isDoPreloadingRewardedVideo || isRewardedVideoLoaded)
        //////    {
        //////        DebugExt.Log("do not preload, for isDoPreloadingRewardedVideo:" + isDoPreloadingRewardedVideo + ", isRewardedVideoLoaded:" + isRewardedVideoLoaded);
        //////        return;
        //////    }
        //////    DebugExt.Log("begin preload fb reward video");

        //////    isDoPreloadingRewardedVideo = true;
        //////    rewardedVideoAd = new RewardedVideoAd(fbRewardVideoId);
        //////    rewardedVideoAd.Register(this.gameObject);

        //////    //各回调详细说明见
        //////    //https://developers.facebook.com/docs/audience-network/guides/ad-formats/rewarded-video/android#server-side-reward-validation
        //////    //视频预加载成功
        //////    rewardedVideoAd.RewardedVideoAdDidLoad = fbRewardedVideoListener.OnRewardedVideoLoadedEvent;
        //////    //预加载失败
        //////    rewardedVideoAd.RewardedVideoAdDidFailWithError = fbRewardedVideoListener.OnRewardedVideoFailedEvent;
        //////    //视频播放完，关闭
        //////    rewardedVideoAd.RewardedVideoAdDidClose = () =>
        //////    {
        //////        DebugExt.Log("fb reward vedio play finish");
        //////        fbRewardedVideoListener.OnRewardedVideoAdClosed();
        //////        StartCoroutine(DelayPreload());
        //////    };

        //////    rewardedVideoAd.LoadAd();
        //////}

        //////public override bool IsRewardedVideoAvailable()
        //////{
        //////    if (isRewardedVideoLoaded && isDoPreloadingRewardedVideo == false)
        //////    {
        //////        //return rewardedVideoAd.IsValid();
        //////        return true;
        //////    }
        //////    return false;
        //////}
        //////void PlayRewardVideo()
        //////{
        //////    Time.timeScale = 0;
        //////    rewardedVideoAd.Show();
        //////    isRewardedVideoLoaded = false;
        //////    isDoPreloadingRewardedVideo = false;
        //////}


        //////public override void ShowRewardedVideo(Action onNotAvailable, Action onSuccess = null, Action onFail = null)
        //////{
        //////    if (this.IsRewardedVideoAvailable())
        //////    {
        //////        fbRewardedVideoListener.onReward = () =>
        //////        {

        //////            if (onSuccess != null)
        //////            {
        //////                onSuccess();
        //////            }

        //////            Time.timeScale = 1;
        //////        };
        //////        fbRewardedVideoListener.onRewardError = () =>
        //////        {
        //////            if (onFail != null)
        //////            {
        //////                onFail();
        //////            }

        //////            Time.timeScale = 1;
        //////        };

        //////        PlayRewardVideo();
        //////    }
        //////    else
        //////    {
        //////        onNotAvailable?.Invoke();
        //////    }

        //////}
    }

    class FBRewardedVideoListener
    {
        //////public Action onReward = null;
        //////public Action onRewardClose = null;
        //////public Action onRewardError = null;

        //////public void OnRewardedVideoLoadedEvent()
        //////{
        //////    Time.timeScale = 1;
        //////    FBAdSdkManager.isDoPreloadingRewardedVideo = false;
        //////    FBAdSdkManager.isRewardedVideoLoaded = true;

        //////    Debug.LogError("fb RewardedVideo preload success  ");
        //////}
        //////public void OnRewardedVideoFailedEvent(string errorMsg)
        //////{
        //////    FBAdSdkManager.isDoPreloadingRewardedVideo = false;
        //////    FBAdSdkManager.isRewardedVideoLoaded = false;
        //////    Time.timeScale = 1;
        //////    Debug.LogError("fb RewardedVideo preload fail  , msg:" + errorMsg);
        //////}

        //////public void OnRewardedVideoExpiredEvent(string adUnitId)
        //////{
        //////    Debug.LogError("RewardedVideo load Expired:" + adUnitId);


        //////    var act = onRewardError;
        //////    if (act != null)
        //////    {
        //////        act();
        //////    }
        //////    onRewardError = null;
        //////}


        //////public void OnRewardedVideoAdShowFailed(string adUnitId, string errorMsg)
        //////{
        //////    Debug.LogError("RewardedVideo play failed :" + adUnitId + ", msg:" + errorMsg);

        //////    var act = onRewardError;
        //////    if (act != null)
        //////    {
        //////        act();
        //////    }
        //////    onRewardError = null;
        //////}

        //////public void OnRewardedVideoAdClicked(string scene)
        //////{

        //////}
        //////public void OnRewardedVideoAdClosed()
        //////{
        //////    DebugExt.Log("fb RewardedVideo closed ");

        //////    if (FBAdSdkManager.rewardedVideoAd != null)
        //////    {
        //////        FBAdSdkManager.rewardedVideoAd.Dispose();
        //////        FBAdSdkManager.rewardedVideoAd = null;
        //////    }


        //////    var act = onReward;
        //////    var act1 = onRewardClose;

        //////    if (act != null)
        //////    {
        //////        act();
        //////    }
        //////    if (act1 != null)
        //////    {
        //////        act1();
        //////    }
        //////    onRewardClose = null;
        //////    onRewardError = null;
        //////    onReward = null;
        //////    Time.timeScale = 1;
        //////}
    }
}