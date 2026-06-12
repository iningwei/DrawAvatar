using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.SDK
{
    public class IronSourceAdSdkManager : AdSdkBase
    {

        //////        public string rewardId = "DefaultRewardedVideo";
        //////        public string interstitialId = "DefaultInterstitial";
        //////        public string bannerID = "DefaultBanner";

        //////        public string ANDROID_APP_KEY = "de9d4f69";
        //////        public string IOS_APP_KEY = "666666";


        //////        Action onRewardVideoShowSuccess = null;
        //////        Action onRewardVideoShowFail = null;



        //////        Action onInterstitialVideoLoadFail = null;
        //////        Action onInterstitialVideoShowSuccess = null;
        //////        Action onInterstitialVideoShowFail = null;




        //////        bool bannerLoaded = false;
        //////        int bannerLoadRetryCount = 0;

        //////        public override void Init()
        //////        {
        //////            DebugExt.Log("do IronSource ad init........");
        //////            if (isInit)
        //////            {
        //////                return;
        //////            }
        //////            isInit = true;
        //////            AppManager.Instance.UnRegisterAppPauseAct(onApplicationPause);
        //////            AppManager.Instance.RegisterAppPauseAct(onApplicationPause);
        //////            //IronSourceConfig.Instance.setClientSideCallbacks(true);
        //////            //IronSource.Agent.shouldTrackNetworkState(true);
        //////            IronSource.Agent.validateIntegration();

        //////#if UNITY_ANDROID
        //////            IronSource.Agent.init(ANDROID_APP_KEY);
        //////#elif UNITY_IOS
        //////         IronSource.Agent.init(IOS_APP_KEY);
        //////#endif

        //////            //IronSourceEvents.onRewardedVideoAdOpenedEvent += onRewardedVideoAdOpened;            
        //////            //IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += onRewardedVideoAvailabilityChanged;             
        //////            IronSourceEvents.onRewardedVideoAdEndedEvent += onRewardedVideoAdEnded;
        //////            IronSourceEvents.onRewardedVideoAdRewardedEvent += onRewardedVideoAdRewarded;
        //////            IronSourceEvents.onRewardedVideoAdShowFailedEvent += onRewardedVideoAdShowFailed;
        //////            //实测，视频开始，并不会触发该事件onRewardedVideoAdStartedEvent
        //////            IronSourceEvents.onRewardedVideoAdStartedEvent += onRewardedVideoAdStarted;
        //////            IronSourceEvents.onRewardedVideoAdClickedEvent += onRewardedVideoAdClicked;
        //////            IronSourceEvents.onRewardedVideoAdClosedEvent += onRewardedVideoAdClosed;


        //////            //IronSourceEvents.onInterstitialAdReadyEvent += onInterstitialAdReady;

        //////            IronSourceEvents.onInterstitialAdLoadFailedEvent += onInterstitialAdLoadFailed;
        //////            IronSourceEvents.onInterstitialAdOpenedEvent += onInterstitialAdOpened;
        //////            IronSourceEvents.onInterstitialAdShowSucceededEvent += onInterstitialAdShowSucceeded;
        //////            IronSourceEvents.onInterstitialAdShowFailedEvent += onInterstitialAdShowFailed;
        //////            IronSourceEvents.onInterstitialAdClickedEvent += onInterstitialAdClicked;
        //////            IronSourceEvents.onInterstitialAdClosedEvent += onInterstitialAdClosed;


        //////            IronSourceEvents.onBannerAdLoadedEvent += onBannerAdLoaded;
        //////            IronSourceEvents.onBannerAdLoadFailedEvent += onBannerAdLoadFailed;


        //////            TimerTween.TimerTween.Delay(60, () =>
        //////            {
        //////                this.LoadBanner();
        //////                this.ShowBanner();
        //////            }).Start(); ;
        //////        }







        //////        #region RewardedVideo

        //////        public override bool IsRewardedVideoAvailable()
        //////        {
        //////            var ret = IronSource.Agent.isRewardedVideoAvailable();
        //////            //Debug.LogError("reward ad available..." + ret);
        //////            return ret;
        //////        }

        //////        public override void ShowRewardedVideo(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        //////        {

        //////            onRewardVideoShowSuccess = onShowSuccess;
        //////            onRewardVideoShowFail = onShowFail;



        //////            if (IsRewardedVideoAvailable())
        //////            {
        //////                EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.RewardVideo_ShowChance);
        //////                IronSource.Agent.showRewardedVideo(rewardId);
        //////            }
        //////            else
        //////            {

        //////                onNotAvailable?.Invoke();
        //////            }
        //////        }
        //////        private void onRewardedVideoAdEnded()
        //////        {
        //////            DebugExt.Log("onRewardedVideoAdEnded");
        //////        }


        //////        private void onRewardedVideoAdClicked(IronSourcePlacement obj)
        //////        {
        //////            DebugExt.Log("onRewardedVideoAdClicked");
        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.RewardVideo_Click);
        //////        }

        //////        private void onRewardedVideoAdStarted()
        //////        {
        //////            DebugExt.Log("onRewardedVideoAdStarted");

        //////        }



        //////        void onRewardedVideoAdClosed()
        //////        {
        //////            DebugExt.Log("onRewardedVideoAdClosed--------------->");

        //////        }

        //////        void onRewardedVideoAdRewarded(IronSourcePlacement ssp)
        //////        {
        //////            DebugExt.Log("onRewardedVideoAdRewarded--->");
        //////            this.lastRewardAdTime = Time.time;
        //////            this.lastInsAdTime = Time.time;//本项目中，在激励视频播放成功后，需要把插屏的时间也重置
        //////            onRewardVideoShowSuccess?.Invoke();
        //////            onRewardVideoShowSuccess = null;

        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.RewardVideo_ShowSuccess);
        //////        }

        //////        void onRewardedVideoAdShowFailed(IronSourceError error)
        //////        {
        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.RewardVideo_ShowFail, error.getErrorCode());
        //////            onRewardVideoShowFail?.Invoke();
        //////            onRewardVideoShowFail = null;
        //////        }
        //////        #endregion


        //////        #region InterstitialVideo


        //////        public override bool IsInterstitialReady()
        //////        {
        //////            return IronSource.Agent.isInterstitialReady();
        //////        }
        //////        public override void LoadInterstitial(Action onLoadFail)
        //////        {
        //////            this.onInterstitialVideoLoadFail = onLoadFail;
        //////            IronSource.Agent.loadInterstitial();
        //////        }


        //////        public override void ShowInterstitial(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        //////        {
        //////            this.onInterstitialVideoShowSuccess = onShowSuccess;
        //////            this.onInterstitialVideoShowFail = onShowFail;
        //////            if (this.IsInterstitialReady() == false)
        //////            {
        //////                DebugExt.Log("Interstitial ad not ready");
        //////                onNotAvailable?.Invoke();
        //////                this.LoadInterstitial(null);
        //////            }
        //////            else
        //////            {
        //////                EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.GameInst_ShowChance);
        //////                IronSource.Agent.showInterstitial(interstitialId);
        //////            }
        //////        }
        //////        private void onInterstitialAdOpened()
        //////        {
        //////            DebugExt.Log("onInterstitialAdOpened ");

        //////        }
        //////        void onInterstitialAdLoadFailed(IronSourceError error)
        //////        {
        //////            DebugExt.Log("onInterstitialAdLoadFailed");
        //////            onInterstitialVideoLoadFail?.Invoke();
        //////            onInterstitialVideoLoadFail = null;

        //////        }

        //////        void onInterstitialAdShowSucceeded()
        //////        {
        //////            DebugExt.Log("onInterstitialAdShowSucceeded");
        //////            this.lastInsAdTime = Time.time;

        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.GameInst_ShowSuccess);
        //////            onInterstitialVideoShowSuccess?.Invoke();
        //////            onInterstitialVideoShowSuccess = null;
        //////        }

        //////        private void onInterstitialAdClicked()
        //////        {
        //////            DebugExt.Log("onInterstitialAdClicked");
        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.GameInst_Click);
        //////        }

        //////        void onInterstitialAdShowFailed(IronSourceError error)
        //////        {
        //////            DebugExt.Log("onInterstitialAdOpenedEvent");

        //////            EventDispatcher.Instance.DispatchEvent(AnalyticsEventID.GameInst_ShowFail, error.getErrorCode());
        //////            onInterstitialVideoShowFail?.Invoke();
        //////            onInterstitialVideoShowFail = null;
        //////        }

        //////        void onInterstitialAdClosed()
        //////        {
        //////            DebugExt.Log("onInterstitialAdClosed");

        //////        }
        //////        #endregion


        //////        #region BannerVideo


        //////        public override void LoadBanner(AdBannerPosition pos = AdBannerPosition.BOTTOM)
        //////        {
        //////            bannerLoadRetryCount++;
        //////            if (pos == AdBannerPosition.BOTTOM)
        //////            {
        //////                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM, bannerID);
        //////            }
        //////            else if (pos == AdBannerPosition.TOP)
        //////            {
        //////                IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.TOP, bannerID);
        //////            }

        //////        }

        //////        private void onBannerAdLoadFailed(IronSourceError error)
        //////        {
        //////            Debug.LogError("banner ad load fail:" + error.ToString() + ", retryCount:" + bannerLoadRetryCount);
        //////            bannerLoaded = false;
        //////            //等待10秒重新加载
        //////            if (bannerLoadRetryCount < 5)
        //////            {
        //////                TimerTween.TimerTween.Delay(60, () =>
        //////                {
        //////                    LoadBanner();
        //////                }).Start();
        //////            }

        //////        }



        //////        void showBanner()
        //////        {
        //////            IronSource.Agent.displayBanner();
        //////            EventDispatcher.Instance.DispatchEvent(EventID.OnBannerAdShowed);
        //////            this.bannerShowed = true;
        //////        }

        //////        void hideBanner()
        //////        {
        //////            IronSource.Agent.hideBanner();
        //////        }
        //////        private void onBannerAdLoaded()
        //////        {
        //////            DebugExt.Log("banner ad load success");
        //////            bannerLoaded = true;
        //////            if (bannerCanShow && bannerLoaded)
        //////            {
        //////                this.showBanner();
        //////            }
        //////            else
        //////            {
        //////                this.hideBanner();
        //////            }
        //////        }
        //////        bool bannerCanShow = false;
        //////        public override void ShowBanner()
        //////        {
        //////            bannerCanShow = true;
        //////            if (bannerLoaded)
        //////            {
        //////                this.showBanner();
        //////            }
        //////        }

        //////        public override void HideBanner()
        //////        {
        //////            bannerCanShow = false;
        //////            IronSource.Agent.hideBanner();
        //////        }
        //////        #endregion

        //////        void onApplicationPause(bool isPaused)
        //////        {
        //////            IronSource.Agent.onApplicationPause(isPaused);
        //////        }

    }
}