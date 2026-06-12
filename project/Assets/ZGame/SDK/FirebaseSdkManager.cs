
#if Firebase
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Messaging;


using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ZGame.Event;
using ZGame.Net;

namespace ZGame.SDK
{
    public class FirebaseSdkManager : Singleton<FirebaseSdkManager>
    {
        bool isAvailable = false;

        Dictionary<string, string> remoteCfgCache = new Dictionary<string, string>();



        public void Init()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            AndroidInit();
#endif

#if !UNITY_EDITOR && UNITY_IOS
            IOSInit();
#endif
        }

        void AndroidInit()
        {
            if (IsPlayServicesAvailable() == false)
            {
                DebugExt.Log("gp services not available, quit firebase init");
                return;
            }

            //笔者遇到国内网络不开vpn的话，在第一次请求FCM Token的时候崩溃
            //因此加try catch进行异常捕捉，规避崩溃(部分机型底层崩溃，try catch无法捕捉，依旧崩溃，问题已反馈给firebase官方)
            try
            {
                DebugExt.Log("--->Firebase sdk  begin android init");
                //ContinueWithOnMainThread     or     ContinueWith
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        DebugExt.Log("--->Firebase sdk result:Available");
                        Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                        isAvailable = true;

                        //拉远程配置 
                        //InitializeRemoteConfig();
                        InitializeFCM();
                    }
                    else
                    {
                        DebugExt.LogE(System.String.Format(
                          "Firebase sdk init fail. Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    }
                });
            }
            catch (Exception ex)
            {

                DebugExt.LogE("firebase sdk init error, ex:" + ex.ToString());
            }

        }

        void IOSInit()
        {
            try
            {
                DebugExt.Log("--->Firebase sdk  begin ios init");

                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
                {
                    var dependencyStatus = task.Result;
                    if (dependencyStatus == Firebase.DependencyStatus.Available)
                    {
                        DebugExt.Log("--->Firebase sdk result:Available");
                        Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                        isAvailable = true;


                        InitializeFCM();
                    }
                    else
                    {
                        DebugExt.LogE(System.String.Format(
                          "Firebase sdk init fail. Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    }
                });
            }
            catch (Exception ex)
            {

                DebugExt.LogE("firebase sdk init error, ex:" + ex.ToString());
            }
        }
        private void InitializeRemoteConfig()
        {
            //DebugExt.Log("fetch remote firebase config");
            //Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(new TimeSpan(0));
            //fetchTask.ContinueWith(onFetchRemoteConfig);
        }

        public void InitializeFCM()
        {
            //NetTool.CheckGoogleReachable(() =>
            //{
            //    //由于在Manifest.xml中设置了
            //    //< meta - data android: name = "firebase_messaging_auto_init_enabled" android: value = "false" />       
            //    //< meta - data android: name = "firebase_analytics_collection_enabled" android: value = "false" />
            //    //本以为需要再设置FirebaseMessaging.TokenRegistrationOnInitEnabled = true;，岂料弄巧成拙，这个就会自动开启TokenRegistration的Init，会有一定的概率导致崩溃
            //    //////FirebaseMessaging.TokenRegistrationOnInitEnabled = true;  //不需要开启
            //    ///

            //    DebugExt.Log("Register firebase  token receive!");
            //    Firebase.Messaging.FirebaseMessaging.TokenReceived += FirebaseMessaging_TokenReceived;

            //}, () =>
            //{
            //    DebugExt.Log("google not reachable,so do not do token received call back init");
            //});

            //DebugExt.Log("Register firebase message receive");
            //Firebase.Messaging.FirebaseMessaging.MessageReceived += FirebaseMessaging_MessageReceived;

            //FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);


            //新代码
            //FirebaseMessaging.TokenRegistrationOnInitEnabled = true;//不需要，反而会一定概率导致启动crash
            FirebaseMessaging.GetTokenAsync().ContinueWithOnMainThread(getTokenTask =>
            {
                if (getTokenTask.IsCompleted)
                {
                    string token = getTokenTask.Result;
                    DebugExt.LogE("--->FCM Token is:" + token);

                    Storage.SetFCMToken(token);
                    //--CrazyPet游戏中通过ALogin协议上行FCMToken，但是由于这里获取token是异步的，可能在ALogin之后才获得token，因此增加事件OnFCMTokenReceived，在确定收到token后，触发事件，再通过一个单独的协议把token上报给服务端
                    EventDispatcher.Instance.DispatchEvent(EventID.OnFCMTokenReceived);
                }
            });


            DebugExt.Log("Register firebase message receive!");
            Firebase.Messaging.FirebaseMessaging.MessageReceived += FirebaseMessaging_MessageReceived;
        }

        //https://www.cnblogs.com/sy-liu/p/14596949.html
        public bool IsPlayServicesAvailable()
        {

            const string GoogleApiAvailability_Classname = "com.google.android.gms.common.GoogleApiAvailability";
            AndroidJavaClass clazz = new AndroidJavaClass(GoogleApiAvailability_Classname);
            AndroidJavaObject obj = clazz.CallStatic<AndroidJavaObject>("getInstance");
            var androidJC = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var activity = androidJC.GetStatic<AndroidJavaObject>("currentActivity");
            int value = obj.Call<int>("isGooglePlayServicesAvailable", activity);
            // 0 == success
            // 1 == service_missing
            // 2 == update service required
            // 3 == service disabled
            // 18 == service updating
            // 9 == service invalid
            DebugExt.Log("GALogController Log IsPlayServicesAvailable value:" + value);
            return value == 0;

        }


        public bool CanLogEvent()
        {
            return isAvailable && AppManager.Instance.isLogEventToSDK;
        }

        //////void onFetchRemoteConfig(Task fetchTask)
        //////{
        //////    if (fetchTask.IsCanceled)
        //////    {
        //////        DebugExt.Log("RemoteConfig Fetch canceled.");
        //////    }
        //////    else if (fetchTask.IsFaulted)
        //////    {
        //////        DebugExt.Log("RemoteConfig Fetch encountered an error.");
        //////    }
        //////    else if (fetchTask.IsCompleted)
        //////    {
        //////        DebugExt.Log("RemoteConfig Fetch completed successfully!");
        //////    }

        //////    //////switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchStatus)
        //////    //////{
        //////    //////    case Firebase.RemoteConfig.LastFetchStatus.Success:
        //////    //////        DebugExt.Log("Remote data loaded and ready.");
        //////    //////        Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
        //////    //////        foreach (var k in Firebase.RemoteConfig.FirebaseRemoteConfig.Keys)
        //////    //////        {
        //////    //////            remoteCfgCache[k] = Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue(k).StringValue;
        //////    //////            DebugExt.Log("RemoteData:" + k + " , " + remoteCfgCache[k]);
        //////    //////        }
        //////    //////        break;
        //////    //////    case Firebase.RemoteConfig.LastFetchStatus.Failure:
        //////    //////        switch (Firebase.RemoteConfig.FirebaseRemoteConfig.Info.LastFetchFailureReason)
        //////    //////        {
        //////    //////            case Firebase.RemoteConfig.FetchFailureReason.Error:
        //////    //////                DebugExt.Log("Fetch failed for unknown reason");
        //////    //////                break;
        //////    //////            case Firebase.RemoteConfig.FetchFailureReason.Throttled:
        //////    //////                DebugExt.Log("Fetch throttled until " +
        //////    //////                Firebase.RemoteConfig.FirebaseRemoteConfig.Info.ThrottledEndTime);
        //////    //////                break;
        //////    //////        }
        //////    //////        break;
        //////    //////    case Firebase.RemoteConfig.LastFetchStatus.Pending:
        //////    //////        DebugExt.Log("Latest Fetch call still pending.");
        //////    //////        break;
        //////    //////}
        //////}




        public int GetRemoteConfigPara(string key, int defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return int.Parse(v);
            }
            return defaultValue;
        }

        public long GetRemoteConfigPara(string key, long defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return long.Parse(v);
            }
            return defaultValue;
        }


        public float GetRemoteConfigPara(string key, float defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return float.Parse(v);
            }
            return defaultValue;
        }


        public string GetRemoteConfigPara(string key, string defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return v;
            }
            return defaultValue;
        }

        public bool GetRemoteConfigPara(string key, bool defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return bool.Parse(v);
            }
            return defaultValue;
        }

        public object GetRemoteConfigPara(string key, object defaultValue)
        {
            if (remoteCfgCache.TryGetValue(key, out var v))
            {
                return (object)v;
            }

            return defaultValue;
        }


        public void LogEvent(string name)
        {
            if (CanLogEvent())
            {
                DebugExt.Log("firebase log event, name:" + name);
                FirebaseAnalytics.LogEvent(name);
            }


        }



        public void LogEventWith1StringParam(string name, string paraKey, string paraValue)
        {
            if (CanLogEvent())
            {
                DebugExt.Log("firebase log event, name:" + name + ", para1Key:" + paraKey + ",para1Value:" + paraValue);
                FirebaseAnalytics.LogEvent(name, new Parameter(paraKey, paraValue));
            }
        }
        public void LogEventWith1LongParam(string name, string paraKey, long paraValue)
        {
            if (CanLogEvent())
            {
                DebugExt.Log("firebase log event, name:" + name + ", para1Key:" + paraKey + ",para1Value:" + paraValue);
                FirebaseAnalytics.LogEvent(name, new Parameter(paraKey, paraValue));
            }
        }
        public void LogEventWith1LongParam2LongParam(string name, string para1Key, long para1Value
            , string para2Key, long para2Value)
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value), new Parameter(para2Key, para2Value));
            }
        }
        public void LogEventWith1LongParam2StringParam(string name, string para1Key, long para1Value
    , string para2Key, string para2Value)
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value), new Parameter(para2Key, para2Value));
            }
        }
        public void LogEventWith1StringParam2LongParam(string name, string para1Key, string para1Value
    , string para2Key, long para2Value)
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value), new Parameter(para2Key, para2Value));
            }
        }
        public void LogEventWith1StringParam2StringParam(string name, string para1Key, string para1Value
, string para2Key, string para2Value)
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value), new Parameter(para2Key, para2Value));
            }
        }

        public void LogEventWith1LongParam2LongParam3LongParam4LongParam(string name, string para1Key, long para1Value
            , string para2Key, long para2Value
            , string para3Key, long para3Value
            , string para4Key, long para4Value
            )
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value},para3Key:{para3Key},para3Value:{para3Value},para4Key:{para4Key},para4Value:{para4Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value)
                    , new Parameter(para2Key, para2Value)
                    , new Parameter(para3Key, para3Value)
                    , new Parameter(para4Key, para4Value));
            }
        }

        public void LogEventWith1LongParam2StringParam3StringParam(string name, string para1Key, long para1Value
     , string para2Key, string para2Value
     , string para3Key, string para3Value

     )
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value},para3Key:{para3Key},para3Value:{para3Value} ");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value)
                    , new Parameter(para2Key, para2Value)
                    , new Parameter(para3Key, para3Value));
            }
        }


        public void LogEventWith1LongParam2DoubleParam3StringParam4LongParam5StringParam(string name, string para1Key, long para1Value
    , string para2Key, double para2Value
    , string para3Key, string para3Value
    , string para4Key, long para4Value
            , string para5Key, string para5Value
    )
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value},para3Key:{para3Key},para3Value:{para3Value},para4Key:{para4Key},para4Value:{para4Value},para5Key:{para5Key},para5Value:{para5Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value)
                    , new Parameter(para2Key, para2Value)
                    , new Parameter(para3Key, para3Value)
                    , new Parameter(para4Key, para4Value)
                     , new Parameter(para5Key, para5Value));
            }


        }
        public void LogEventWith1LongParam2StringParam3StringParam4LongParam5LongParam6LongParam(string name, string para1Key, long para1Value
, string para2Key, string para2Value
, string para3Key, string para3Value
, string para4Key, long para4Value
    , string para5Key, long para5Value
             , string para6Key, long para6Value
)
        {
            if (CanLogEvent())
            {
                DebugExt.Log($"firebase log event, name:{name}, para1Key:{para1Key},para1Value:{para1Value},para2Key:{para2Key},para2Value:{para2Value},para3Key:{para3Key},para3Value:{para3Value},para4Key:{para4Key},para4Value:{para4Value},para5Key:{para5Key},para5Value:{para5Value},para6Key:{para6Key},para6Value:{para6Value}");
                FirebaseAnalytics.LogEvent(name, new Parameter(para1Key, para1Value)
                    , new Parameter(para2Key, para2Value)
                    , new Parameter(para3Key, para3Value)
                    , new Parameter(para4Key, para4Value)
                     , new Parameter(para5Key, para5Value)
                     , new Parameter(para6Key, para6Value));
            }
        }




#region tmp unused
        //////public void LogEvent(string name)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name);
        //////    }
        //////}
        //////public void LogEvent(string name, string parameterName, string parameterValue)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        //////    }
        //////}

        //////public void LogEvent(string name, string parameterName1, string parameterValue1, string parameterName2, string parameterValue2)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, new Parameter(parameterName1, parameterValue1), new Parameter(parameterName2, parameterValue2));
        //////    }
        //////}

        //////public void LogEvent(string name, string parameterName, int parameterValue)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        //////    }
        //////}
        //////public void LogEvent(string name, string parameterName, long parameterValue)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        //////    }
        //////}
        //////public void LogEvent(string name, string parameterName, float parameterValue)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        //////    }
        //////}
        //////public void LogEvent(string name, string parameterName, double parameterValue)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameterName, parameterValue);
        //////    }
        //////}

        //////public void LogEvent(string name, params Parameter[] parameters)
        //////{
        //////    if (CanUse())
        //////    {
        //////        FirebaseAnalytics.LogEvent(name, parameters);
        //////    }
        //////}

#endregion

        private void FirebaseMessaging_TokenReceived(object sender, TokenReceivedEventArgs e)
        {
            string token = e.Token;
            DebugExt.LogE("FCM------>Token:" + token);
            Storage.SetFCMToken(token);
        }

        private static void FirebaseMessaging_MessageReceived(object sender, Firebase.Messaging.MessageReceivedEventArgs e)
        {
            try
            {

                DebugExt.Log("FCM-----> Receive message");
                //如果是后台收到通知，点击通知后，app回到前台。这个时候Notification为空：https://firebase.google.com/docs/cloud-messaging/unity/client?authuser=0
                var notification = e.Message.Notification;
                if (notification != null)
                {
                    //游戏运行时收到通知，这个时候不会在手机的消息栏推送。TODO：可以在游戏内popup，展示推送信息
                    //e.Message.NotificationOpened == false也可以用来确定是不是在app运行的时候收到了通知
                    DebugExt.Log("FCM------>From:" + e.Message.From);
                    DebugExt.Log("FCM------>Title:" + e.Message.Notification.Title);
                    DebugExt.Log("FCM------>Body:" + e.Message.Notification.Body);
                }
                else
                {
                    //从后台回到前台，解析数据，进行跳转
                    if (e.Message.Data != null && e.Message.Data.Count > 0)
                    {
                        //收到数据，对数据解析
                        foreach (KeyValuePair<string, string> item in e.Message.Data)
                        {
                            DebugExt.Log("FCM------>Data,key:" + item.Key + ", value:" + item.Value);
                            if (item.Key == "jump")
                            {
                                BattleGlobal.FCMJumpDst = item.Value;
                                DebugExt.Log("cs broadcast OnFCMJump event， fcmJumpDst:" + BattleGlobal.FCMJumpDst);

                                EventDispatcher.Instance.DispatchEvent(EventID.OnFCMJump);
                                break;
                            }
                        }
                    }
                }






                //格式：{"title": "t1", "body": "b1"}
                //string toastStr = "{\"title\": \"" + e.Message.Notification.Title + "\",\"body\": \"" + e.Message.Notification.Body + "\"}";
                //DebugExt.LogE("toastStr:" + toastStr);
                //////BTManager.instance.ToastMessage(toastStr, 0);
            }
            catch (Exception ex)
            {
                DebugExt.LogE("messageReceived exception:" + ex.ToString());
                throw;
            }

        }
    }
}
#endif