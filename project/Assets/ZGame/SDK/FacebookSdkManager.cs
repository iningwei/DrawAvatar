using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using MiniJSON;
using System.IO;
#if Facebook
using Facebook.Unity;
#endif

namespace ZGame.SDK
{
    public class FacebookSdkManager : Singleton<FacebookSdkManager>
    {
#if Facebook
        bool autoCallLoginWhileInited = false;
        public void Init()
        {

            DebugExt.Log("Do FB Init");
            FB.Init(() =>
            {
                DebugExt.Log("FB Init success");
                FB.Mobile.SetAutoLogAppEventsEnabled(true);
                FB.ActivateApp();
                if (autoCallLoginWhileInited)
                {
                    Login();
                }
            });

            AppManager.Instance.RegisterAppPauseAct(b =>
            {
                if (!b)
                {
                    if (FB.IsInitialized)
                    {
                        FB.ActivateApp();
                    }
                    else
                    {
                        FB.Init(() =>
                        {
                            FB.Mobile.SetAutoLogAppEventsEnabled(true);
                            FB.ActivateApp();
                        });
                    }
                }
            });
        }


        public void Login(bool autoCallLoginWhileInitSuccess = false)
        {
            Debug.Log("call fb login, autoCallLoginWhileInitSuccess:" + autoCallLoginWhileInitSuccess);
            if (!FB.IsInitialized)
            {
                DebugExt.LogE("fb can not login，for it is not initialized, autoCallLoginWhileInitSuccess:" + autoCallLoginWhileInitSuccess);
                autoCallLoginWhileInited = autoCallLoginWhileInitSuccess;
                return;
            }

            DebugExt.Log("do fb login--->");
            //var perms = new List<string>() { "public_profile", "email" };
            //使用gaming_user_picture权限，在游戏类型app中才能获得fb帐号的头像; 否则public_profile权限获得的是fb游戏账户的头像
            //https://developers.facebook.com/docs/games/social-and-retention/features/login-for-gaming
            var perms = new List<string>() { "gaming_user_picture", "email" };

            FB.LogInWithReadPermissions(perms, authCallback);

        }

        public void Logout()
        {
            FB.LogOut();
        }

        Action shareSuccessCallback;
        Action shareFailCallback;
        public void Share(string contentURL, Action successCallback, Action failCallback)
        {
            shareSuccessCallback = successCallback;
            shareFailCallback = failCallback;
            FB.ShareLink(new Uri(contentURL), callback: shareCallback);
        }

        private void shareCallback(IShareResult result)
        {
            if (result.Cancelled)
            {
                DebugExt.LogE("share cancelled");
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }

            }
            else if (!String.IsNullOrEmpty(result.Error))
            {
                DebugExt.LogE("share error:" + result.Error);
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }
            }
            else if (!String.IsNullOrEmpty(result.PostId))
            {
                DebugExt.LogE("share error, postId:" + result.PostId);
                if (shareFailCallback != null)
                {
                    shareFailCallback();
                    shareFailCallback = null;
                }
            }
            else
            {
                DebugExt.LogE("share success!");
                if (shareSuccessCallback != null)
                {
                    shareSuccessCallback();
                    shareSuccessCallback = null;
                }

            }
        }

        string userId;
        string sdkToken;

        private void authCallback(IResult result)
        {
            if (result == null)
            {
                DebugExt.LogE("fb authCallback Null response");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Facebook auth have no response");
                return;
            }

            if (!string.IsNullOrEmpty(result.Error))
            {
                DebugExt.LogE("fb login fail:" + result.Error);
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, result.Error);
            }
            else if (result.Cancelled)
            {
                DebugExt.Log("user cancelled fb login");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "User Cancelled facebook login");
            }
            else if (!string.IsNullOrEmpty(result.RawResult))
            {
                DebugExt.Log("fb login success");
                //DebugExt.Log("RawResult:" + result.RawResult);

                var dic = result.ResultDictionary;
                foreach (KeyValuePair<string, object> item in dic)
                {
                    DebugExt.LogE("11111 key:" + item.Key + ",  value:" + item.Value.ToString());
                    //经过打印信息，发现这里无name和picture等信息
                }
                //ResultDictionary 存储的就是RawResult中的键值对




                dic.TryGetValue("user_id", out userId);
                dic.TryGetValue("access_token", out sdkToken);
                DebugExt.Log("user_id:" + userId);
                DebugExt.Log("access_token:" + sdkToken);


                //二次请求获得信息
                DebugExt.Log("try get more message:@@@--->");
                FB.API("/me?fields=first_name,last_name,gender,email,picture", HttpMethod.GET, graphCallback);

            }
            else
            {
                DebugExt.LogE("fb empty response");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Facebook response have no data");
            }
        }


        private void graphCallback(IGraphResult result)
        {
            string firstname = "";
            string lastname = "";
            string gender = "";
            string email = "";
            //string location = "";
            object pictureData = null;
            string pictureUrl = "";
            if (result == null)
            {
                DebugExt.LogE("fb graphCallback Null response");
                EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "GraphCallback null");
            }
            else
            {
                if (!string.IsNullOrEmpty(result.Error))
                {
                    DebugExt.LogE("fb got user message error:" + result.Error);
                    EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginFail, "Get user message failed");
                }
                else
                {
                    //fb gender, location需要额外申请权限??

                    result.ResultDictionary.TryGetValue("first_name", out firstname);
                    result.ResultDictionary.TryGetValue("last_name", out lastname);
                    result.ResultDictionary.TryGetValue("gender", out gender);
                    result.ResultDictionary.TryGetValue("email", out email);
                    //result.ResultDictionary.TryGetValue("location", out location);

                    result.ResultDictionary.TryGetValue("picture", out pictureData);
                    object pictureData2 = null;
                    (pictureData as Dictionary<string, object>).TryGetValue("data", out pictureData2);
                    (pictureData2 as Dictionary<string, object>).TryGetValue("url", out pictureUrl);
                    //DebugExt.LogE("pictureUrl:" + pictureUrl);
                    DebugExt.Log($"firstname:{firstname},lastname:{lastname},gender:{gender}");

                    string nick_name = (firstname + " " + lastname).Trim();

                    EventDispatcher.Instance.DispatchEvent(EventID.SDKLoginCMP, AuthChannelID.FB + "|" + userId + "|" + sdkToken + "|" +
                        nick_name + "|" + gender + "|" + email + "|" + pictureUrl);
                }
            }
        }

        private void onHideUnity(bool isUnityShown)
        {
            if (!isUnityShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        private void initCallback()
        {
            DebugExt.Log("fb sdk init finished");
            // Signal an app activation App Event
            //FB.Mobile.SetAutoLogAppEventsEnabled(true);
            FB.ActivateApp();
        }




        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////    });
        //////    }

        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2, string parameterName3, object parameterValue3)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////        { parameterName3, parameterValue3 },
        //////    });
        //////    }
        //////    public void LogEvent(string name, string parameterName1, object parameterValue1, string parameterName2, object parameterValue2, string parameterName3, object parameterValue3, string parameterName4, object parameterValue4)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName1, parameterValue1 },
        //////        { parameterName2, parameterValue2 },
        //////        { parameterName3, parameterValue3 },
        //////        { parameterName4, parameterValue4 },
        //////    });
        //////    }
        //////    public void LogEvent(string name, string parameterName, object parameterValue)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, new Dictionary<string, object>()
        //////    {
        //////        { parameterName, parameterValue }
        //////    });
        //////    }

        //////    public void LogEvent(string name)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        FB.LogAppEvent(name, null, null);
        //////    }

        //////    public void LogEvent(string name, params object[] events)
        //////    {
        //////        if (!FB.IsInitialized)
        //////            return;
        //////        int count = events != null ? events.Length / 2 : 0;
        //////        if (count > 0)
        //////        {
        //////            Dictionary<string, object> param = new Dictionary<string, object>();
        //////            for (int i = 0; i < count; i++)
        //////            {
        //////                string key = events[i * 2] as string;
        //////                if (string.IsNullOrEmpty(key)) { DebugExt.LogE(" fetal error: " + name + ": key is not string:" + events[i * count]); continue; }
        //////                object value = events[i * 2 + 1];
        //////                param[key] = value;
        //////            }

        //////            FB.LogAppEvent(name, null, param);
        //////        }
        //////        else
        //////        {
        //////            FB.LogAppEvent(name, null, null);
        //////        }
        //////    }
#endif

    }

}