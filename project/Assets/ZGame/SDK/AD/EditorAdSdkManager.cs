using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.SDK
{
    public class EditorAdSdkManager : AdSdkBase
    {
        public override void Init()
        {
            DebugExt.Log("EditorAdSdk init....");
        }
        public override void ShowInterstitial(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        {
            onShowSuccess?.Invoke();
        }
        public override void ShowRewardedVideo(Action onNotAvailable, Action onShowSuccess, Action onShowFail)
        {
            if (this.IsRewardedVideoAvailable())
            {
                onShowSuccess?.Invoke();
            }
            else
            {
                onNotAvailable?.Invoke();
            }
        }

        public override bool IsRewardedVideoAvailable()
        {
            return true;
        }

    }
}