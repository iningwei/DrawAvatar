using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if SIWA
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Extensions;
using AppleAuth.Interfaces;
using AppleAuth.Native;
#endif
using ZGame;
using ZGame.Event;
using ZGame.SDK;
using System.Text;

public class AppleSDKMgr : SingletonMonoBehaviour<AppleSDKMgr>
{
#if SIWA

    private IAppleAuthManager _appleAuthManager;

    /// <summary>
    /// iOS设备系统版本号 
    /// </summary>
    public string iosDeviceOSVersion
    {
        get
        {
#if UNITY_IOS && !UNITY_EDITOR
            return UnityEngine.iOS.Device.systemVersion;
#else
            return "1.0.0";
#endif

        }
    }
    /// <summary>
    /// iOS设备系统大版本号;iOS设备大版本低于13则无需开启Sign in with Apple
    /// </summary>
    public int iosDeviceOSBigVersion
    {
        get
        {
            return int.Parse(iosDeviceOSVersion.Split('.')[0]);
        }
    }

    bool isInit = false;

    public void Login()
    {
        DebugExt.Log("cur ios version：" + iosDeviceOSVersion);
        DebugExt.Log("cur ios big version：" + iosDeviceOSBigVersion);

        var deserializer = new PayloadDeserializer();
        this._appleAuthManager = new AppleAuthManager(deserializer);


        this.SignInWithApple();
    }
    private void SignInWithApple()
    {
        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail | LoginOptions.IncludeFullName);

        this._appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                // If a sign in with apple succeeds, we should have obtained the credential with the user id, name, and email, save it
                string userId = credential.User;
                var appleIdCredential = credential as IAppleIDCredential;
                string identityToken = "";
                string email = "";
                string nick_name = "";
                if (appleIdCredential.IdentityToken != null)
                {
                    identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken, 0, appleIdCredential.IdentityToken.Length);
                }
                if (appleIdCredential.Email != null)
                {
                    email = appleIdCredential.Email;
                }
                if (appleIdCredential.FullName != null)
                {
                    nick_name = appleIdCredential.FullName.ToLocalizedString();
                }

                string gender = "";
                string pictureUrl = "";

                DebugExt.LogE("APPLE login finish--->nick_name:" + nick_name + ", email:" + email + ", idToken:" + identityToken + ", userId:" + userId);


                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginCMP, AuthChannelID.APPLE + "|" + userId + "|" + identityToken + "|" +
           nick_name + "|" + gender + "|" + email + "|" + pictureUrl);
            },
            error =>
            {
                var authorizationErrorCode = error.GetAuthorizationErrorCode();
                DebugExt.LogE("Sign in with Apple failed: " + authorizationErrorCode.ToString() + ", error:" + error.ToString());
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, authorizationErrorCode.ToString());
            });
    }

    private void Update()
    {
        // Updates the AppleAuthManager instance to execute
        // pending callbacks inside Unity's execution loop
        if (this._appleAuthManager != null)
        {
            this._appleAuthManager.Update();
        }

    }

#endif
}
