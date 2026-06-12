using System;
using UnityEngine;



//https://titanwolf.org/Network/Articles/Article?AID=cc30918d-cd91-4cbc-9fdb-c9eb88e882f7
//https://answers.unity.com/questions/1362906/change-brightness-of-display-on-android.html

//PS:have not tested on devices!!!
//
// temple cancel use on ios
/*
#if !UNITY_EDITOR && UNITY_IOS
 using System.Runtime.InteropServices;
#endif
*/
namespace ZGame.SDK
{
    public class AppBrightness
    {
        /*
#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern void setBrightness(float brightness);

        [DllImport("__Internal")]
        private static extern float getBrightness();
#endif
        */





        /// <summary>
        /// 
        /// </summary>
        /// <param name="brightness"></param>
        public static void Set(float brightness)
        {

#if UNITY_EDITOR

#elif UNITY_IOS
/*
                        setBrightness(brightness.Value);
                        */
#elif UNITY_ANDROID
//这里设置的是当前运行App的亮度，并不会影响到其它App和系统的亮度，切到其它界面后，会恢复到系统的亮度
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                var window = activity.Call<AndroidJavaObject>("getWindow");
                var lp = window.Call<AndroidJavaObject>("getAttributes");
                lp.Set("screenBrightness", brightness);
                window.Call("setAttributes", lp);
            }));

#endif

        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static float Get()
        {
            float brightness = 1f;
#if UNITY_EDITOR
#elif UNITY_IOS
/*
            brightness = getBrightness();
            */
#elif UNITY_ANDROID
//需要注意的是这里获得的是屏幕亮度，即系统级的亮度。安卓中这部分操作不需要权限
            var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var system = new AndroidJavaClass("android.provider.Settings.System");
            var contentResolver = activity.Call<AndroidJavaObject>("getContentResolver");
            var b = system.CallStatic<int>("getInt", contentResolver, system.CallStatic<string>("SCREEN_BRIGHTNESS"));
            brightness = b / 255f;
#endif
            return brightness;
        }
    }
}
