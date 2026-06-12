using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Event
{
    public class EventBaseID
    {
        //---------------->游戏框架相关 1-10000
        public const string OnBeginDownloadResFiles = "1";
        public const string OnResFileDownloading = "2";
        public const string OnResFileDownloaded = "3";

        public const string SDKLoginSuccess = "10";
        public const string SDKLoginFail = "11";
        public const string SDKLoginCMP = "12";

        public const string OnFCMTokenReceived = "20";
        public const string OnSoundSliderValueChange = "30";//声音大小，控制bgm和audio
        public const string OnBgmSliderValueChange = "31";
        public const string OnAudioSliderValueChange = "32";
        public const string OnLanguageCodeChange = "40";

        public const string OnBeginDownloadHotResFiles = "50";
        public const string OnHotResFileDownloaded = "51";
        public const string OnHotResFileDownloadFail = "52";
        public const string OnHotResFileDownloading = "53";

        public const string OnABResLoaded = "100";
        public const string OnRootCompInfoHolderObjDestroy = "101";
        public const string OnCompInfoHolderChildObjDestroy = "102";

        public const string OnDynamicCompInfoHolderObjInstantiate = "110";

        

        public const string OnFCMJump = "410";
    }
}