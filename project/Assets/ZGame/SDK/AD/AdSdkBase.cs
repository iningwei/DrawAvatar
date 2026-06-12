using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.SDK
{
    public enum AdBannerPosition
    {
        BOTTOM,
        TOP
    }

    public class AdSdkBase : MonoBehaviour
    {

        public bool isInit = false;

        public float lastRewardAdTime;
        public float lastInsAdTime;

        public bool bannerShowed = false;
        public virtual void Init()
        {

        }

        public virtual void HideBanner()
        {

        }

        public virtual bool IsRewardedVideoAvailable()
        {
            return false;
        }

        public virtual void LoadBanner(AdBannerPosition pos)
        {

        }

        public virtual void ShowBanner()
        {

        }

        public virtual void LoadInterstitial(Action onLoadFail)
        {

        }

        public virtual bool IsInterstitialReady()
        {
            return false;
        }


        public virtual void ShowInterstitial(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        {

        }

        public virtual void ShowRewardedVideo(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        {

        }


    }
}