/* 该代码对应老版本Sign in with Apple SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SIWA
using UnityEngine.SignInWithApple;
#endif
using ZGame;
using ZGame.Event;
using ZGame.SDK;

public class AppleSDKMgr : Singleton<AppleSDKMgr>
{
#if SIWA
    public GameObject LoginAppleObj = null;
  
    public SignInWithApple SIWA = null;


    /// <summary>
    /// iOS设备系统版本号 
    /// </summary>
    public string iosDeviceOSVersion;
    /// <summary>
    /// iOS设备系统大版本号;iOS设备大版本低于13则无需开启Sign in with Apple
    /// </summary>
    public int iosDeviceOSBigVersion;
    public AppleSDKMgr()
    {
#if UNITY_IOS && !UNITY_EDITOR
        iosDeviceOSVersion = UnityEngine.iOS.Device.systemVersion;
        DebugExt.Log("cur ios version：" + iosDeviceOSVersion);
        iosDeviceOSBigVersion = int.Parse(iosDeviceOSVersion.Split('.')[0]);
        DebugExt.Log("cur ios big version：" + iosDeviceOSBigVersion);
#endif
    }

    public void Login()
    {
        if (AppleSDKMgr.Instance.LoginAppleObj == null)
        {
            Debug.Log("init SignInWithApple");
            LoginAppleObj = new GameObject();
            LoginAppleObj.name = "LoginAppleObj";
            SIWA = LoginAppleObj.AddComponent<SignInWithApple>();
        }
        else
        {
            SIWA = LoginAppleObj.GetComponent<SignInWithApple>();
        }

        SIWA.Login(onAppleLoginCallback);
    }

    private void onAppleLoginCallback(SignInWithApple.CallbackArgs args)
    {
        DebugExt.Log("onAppleLoginCallback!");
        if (args.error == null)
        {
            DebugExt.LogE("appleLogin success");
            UserInfo userInfo = args.userInfo;
            string displayName = userInfo.displayName;
            string email = userInfo.email;
            string idToken = userInfo.idToken;
            string userId = userInfo.userId;
            string userDetectionStatus = userInfo.userDetectionStatus.ToString();
            DebugExt.LogE("displayName:" + displayName + ", email:" + email + ", idToken:" + idToken + ", userId:" + userId + ", userDetectionStatus:" + userDetectionStatus);

            //Storage.SetAuthChannel(SDKAuthChannelID.APPLE);//调整为延迟设置（SDKLoginCMP消息中，以及游戏内绑定绑定成功后再设置）

            string nick_name = displayName;
            string gender = "";
            string location = "";
            string pictureUrl = "";
            EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginCMP, AuthChannelID.APPLE + "|" + userId + "|" + idToken + "|" +
            nick_name + "|" + gender + "|" + email + "|" + pictureUrl);
        }
        else
        {
            DebugExt.LogE("appleLogin fail，error:" + args.error);
            EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail,args.error);
        }

    }
#endif
}
*/