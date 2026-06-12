using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
#if IAP
using ZGame.SDK.IAP;
using UnityEngine.Purchasing;
#endif

using ZGame;
using System.Runtime.InteropServices;

#if Facebook
using Facebook.Unity;
#endif

namespace ZGame.SDK
{

    /// <summary>
    /// 游戏上线渠道
    /// </summary>
    public class GameChannelId
    {
        public static string AppleStore = "99";
        public static string GooglePlay = "100";
        public static string QieZi = "101";
    }

    /// <summary>
    /// 登录渠道
    /// </summary>
    public class AuthChannelID
    {
        public static int NONE = -1;
        /// <summary>
        /// 游客
        /// </summary>
        public static int VISITOR = 0;
        /// <summary>
        /// FB登录
        /// </summary>
        public static int FB = 1;
        /// <summary>
        /// GP登录
        /// </summary>
        public static int GP = 2;
        /// <summary>
        /// Sign in with Apple
        /// </summary>
        public static int APPLE = 3;
    }



    /// <summary>
    /// 支付渠道Id(这里枚举需要和服务端的支付平台类型枚举保持一致)
    /// </summary>
    public class PaymentChannelId
    {
        public static int AppleStore = 1;
        public static int GooglePlay = 2;
    }


    public class SDKTools
    {
        public static void Init()
        {
        
#if Firebase
            FirebaseSdkManager.Instance.Init();
#endif

#if Facebook
            FacebookSdkManager.Instance.Init();
#endif

            //////AnalyticsSdkManager.Instance.Init();
            //////AdSdkManager.Instance.Init();
            /// 
        }




        public static void IAPInit(string[] consumableProductIds, string[] nonConsumableProductIds, string[] subscriptionProductIds)
        {
#if IAP
            IAPMgr.Instance.Init(consumableProductIds, nonConsumableProductIds, subscriptionProductIds);
#endif
        }

        public static string GetOSType()
        {
#if UNITY_EDITOR
            return "unity_editor";
#elif UNITY_ANDROID
        return "android";
#elif UNITY_IOS
        return "ios";
#endif
            return "unknown";
        }


        public static string GetDeviceId()
        {
#if UNITY_EDITOR
            return getMacAddress() + getDeviceUniqueIdentifier();
#elif UNITY_STANDALONE
                                    return getMacAddress() + getDeviceUniqueIdentifier();
#elif UNITY_ANDROID
                                    var uniqueID = new AndroidJavaClass("com.zgame.sdk.DeviceID");
                                    string id;
                                    using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                                    {
                                        object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                                        id = uniqueID.CallStatic<string>("Get", jo, false);
                                    }
                                    id = "1_" + id;
                                    Debug.Log("get device ID：" + id);
                                    return id;
#elif UNITY_IOS
            string sid = SDKExt.GetiOSDeviceUniqueId();
            sid = sid.Replace("-", "");
            sid = sid.Substring(0, 32);
            sid = "2_" + sid;
            Debug.Log("get device ID：" + sid);
            return sid;
#endif
            return "";
        }

        public static void SetAppScreenBrightness(int value)
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
            var androidTools = new AndroidJavaClass("com.zgame.sdk.AndroidTools");

            using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                var jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

                //SetAppScreenBrightness函数直接调用会报错---->：
                //c# exception:UnityEngine.AndroidJavaException: android.view.ViewRootImpl$CalledFromWrongThreadException: Only the original thread that created a view hierarchy can touch its views.
                jo.Call("runOnUiThread", new AndroidJavaRunnable(()=> {
                    androidTools.CallStatic("SetAppScreenBrightness", jo, value);
                }));
            }
#elif UNITY_IOS
#endif

        }
        #region Facebook登录
        public static void LoginFB(bool autoCallLoginWhileInitSuccess)
        {
#if Facebook
            FacebookSdkManager.Instance.Login(autoCallLoginWhileInitSuccess);
#endif
        }
        public static void LogoutFB()
        {
#if Facebook
            FacebookSdkManager.Instance.Logout();
#endif
        }

        public static bool IsFBInited()
        {
#if Facebook
            return FB.IsInitialized;
#endif
            return false;
        }
        #endregion

        #region SignInWithApple
        public static void LoginAPPLE()
        {
#if SIWA
            AppleSDKMgr.Instance.Login();
#endif
        }
        #endregion

        //用户只要不卸载安装，那么guid就是唯一的（第一次设置的值）
        private static string getGUID()
        {
            string guidStr = PlayerPrefs.GetString("guid", "");

            if (guidStr == "")
            {
                guidStr = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString("guid", guidStr);
            }
            return guidStr;
        }

        private static string getDeviceUniqueIdentifier()
        {
            return SystemInfo.deviceUniqueIdentifier.ToLower();
        }
        private static string getMacAddress()
        {
            string physicalAddress = "None";

            NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

            foreach (NetworkInterface adaper in nice)
            {

                Debug.Log(adaper.Description);

                if (adaper.Description == "en0")
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();
                    break;
                }
                else
                {
                    physicalAddress = adaper.GetPhysicalAddress().ToString();

                    if (physicalAddress != "")
                    {
                        break;
                    };
                }
            }

            return physicalAddress.ToLower();
        }



        #region 支付
#if IAP
        public static void PurchaseProduct(string productID, Action onControllerIsNull, Action<PurchaseFailureReason> onFailed, Action<string> onSuccess)
        {

            IAPMgr.Instance.PurchaseProduct(productID, onControllerIsNull, onFailed, onSuccess);

    }
#endif

        public static void AddPayVerifyData(string inner_order_id, string inner_app_id, string receipt_data, int pay_platform)
        {
#if IAP
            PayVerify.Instance.AddPayVerifyData(inner_order_id, inner_app_id, receipt_data, pay_platform);

            PayVerify.Instance.WritePayVerifyDataToLocal();
#endif
        }



        public static void DeleteVerifyData(string inner_order_id)
        {
#if IAP
            PayVerify.Instance.DeleteVerifyData(inner_order_id);
            PayVerify.Instance.WritePayVerifyDataToLocal();
#endif
        }

        public static void ReadPayVerifyDataFromLocal()
        {
#if IAP
            PayVerify.Instance.ReadPayVerifyDataFromLocal();
#endif
        }

#if IAP
        /// <summary>
        /// 没有的话返回null
        /// </summary>
        /// <returns></returns>
        public static PayVerifyData GetFirstPayVerifyDataFromLocal()
        {
            var data = PayVerify.Instance.ReadFirstVerifyDataFromLocal();

            if (data != null)
            {
                Debug.Log("get lost order data, inner_order_id:" + data.inner_order_id);
            }
            else
            {
                Debug.Log("no lost order data！！");
            }
            return data;
        }
#endif

        #endregion
         

    }





}