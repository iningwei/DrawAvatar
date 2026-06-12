using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class SDKExt
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string getIPv6(string mHost, string mPort);
#endif

    public static string GetIPv6ByCS(string mHost, string mPort)
    {
#if UNITY_IOS && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
#else
        return mHost + "&&ipv4";
#endif
    }



#if !UNITY_EDITOR && UNITY_IOS
        [DllImport("__Internal")]
        private static extern string DeviceUniqueId();
#endif
    public static string GetiOSDeviceUniqueId()
    {
#if UNITY_IOS && !UNITY_EDITOR
        return DeviceUniqueId();
#endif
        return "";
    }



#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void TryRequestPermissionIfNeeded();
#endif
    public static void TryWebRequest()
    {
#if UNITY_IOS && !UNITY_EDITOR
		TryRequestPermissionIfNeeded();
#endif
    }




    //安卓gp商店 格式为：   "market://details?id=" + appId
    //ios商店 格式为： "itms-apps://itunes.apple.com/app/id" + appId
    public static void openStore(string key)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("Jump to google play store!");
        //Application.OpenURL(url);//这样对于装了多个商店的机器，有可能会打开其他商店
        openGPMarketShop(key,"com.android.vending");
#elif UNITY_IOS && !UNITY_EDITOR
        Debug.Log("Jump to apple store"); 
        openAppStore(key);
#else
        Debug.Log("Can not open store in editor");
#endif
    }

    public static void openAppStore(string key)
    {
        string url = "itms-apps://itunes.apple.com/app/id" + key;
        Application.OpenURL(url);
    }
    public static void openAndroidDownloadPage(string url)
    {
        Application.OpenURL(url);
    }

    /// <summary>
    /// 打开安卓gp商店
    /// </summary>
    /// <param name="appPackage">应用市场中目标app包名</param>
    /// <param name="marketPackage">应用市场包名</param>
    public static void openGPMarketShop(string appPackage, string marketPackage)
    {
        if (!Application.isEditor)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_VIEW"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "market://details?id=" + appPackage);
            intentObject.Call<AndroidJavaObject>("setData", uriObject);
            intentObject.Call<AndroidJavaObject>("setPackage", marketPackage);
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("startActivity", intentObject);
        }
    }

    #region WEBGL USED
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void getUserTicket();
#endif
    public static void GetUserTicket()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		getUserTicket();
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void openURLSelf(string url);
#endif
    public static void OpenURLSelf(string url)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		openURLSelf(url);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void copyText(string content);
#endif
    public static void CopyText(string content)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		copyText(content);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void copyText1(string content);
#endif
    public static void CopyText1(string content)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		copyText1(content);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void copyText2(string content);
#endif
    public static void CopyText2(string content)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		copyText2(content);
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void copyText3(string content);
#endif
    public static void CopyText3(string content)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
		copyText3(content);
#endif
    }
    #endregion
}
