using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace ZGame.Net
{
    /// <summary>
    /// socket连上的方式：直连；重连
    /// </summary>
    public enum SocketConnectedStyle
    {
        Direct,
        ReConnect,
    }
    public class NetTool
    {
        public static SocketConnectedStyle socketConnectedStyle = SocketConnectedStyle.Direct ;



        public static bool IsGoogleReachable = false;
        public static void CheckGoogleReachable(Action reachable, Action unreachable)
        {
            string targetUrl = "https://google.com";
            CoroutineManager.Instance.AddCoroutine(CheckInternetConnection(targetUrl, () =>
            {
                IsGoogleReachable = true;
                reachable?.Invoke();
            }, () =>
            {
                IsGoogleReachable = false;
                unreachable?.Invoke();
            }));
        }



        public static void CheckNetConnectivity(bool overWall, Action successCallback, Action failCallback)
        {
            //////这种方式获得的不准
            //////https://docs.unity3d.com/ScriptReference/Application-internetReachability.html
            /////https://stackoverflow.com/questions/57929106/how-to-properly-check-for-internet-connection-in-unity/57931517#57931517
            /////https://issuetracker.unity3d.com/issues/android-application-dot-internetreachability-returns-notreachable-when-trying-to-receive-unitywebrequest?_ga=2.30583801.770569249.1579078184-493465460.1572247986
            /////公司测试手机遇到第一次Check的时候总是返回false的情况
            //if (Application.internetReachability == NetworkReachability.NotReachable)
            //{
            //    return false;
            //}
            //return true;


            //------------------------------------->
            //////string targetUrl = overWall ? "https://google.com" : "https://www.baidu.com";
            //////CoroutineManager.Singleton.AddCoroutine(CheckInternetConnection(targetUrl, successCallback, failCallback));
            ///取消上述通过url检测当前网络是否连通的方式



            if (successCallback != null)
            {
                successCallback();
            }
        }

        static IEnumerator CheckInternetConnection(string echoServer, Action successCallback, Action failCallback)
        {
            bool result;
            using (var request = UnityWebRequest.Head(echoServer))
            {
                request.timeout = 5;
                yield return request.SendWebRequest();
                //result = !request.isNetworkError && !request.isHttpError && request.responseCode == 200;
                result = (request.result != UnityWebRequest.Result.ConnectionError) && (request.result != UnityWebRequest.Result.ProtocolError) && request.responseCode == 200;
                if (result == false)
                {
                    DebugExt.LogE("---------------------->request.error:" + request.error + "，request.responseCode：" + request.responseCode);
                }
            }
            if (result == true)
            {
                DebugExt.Log("internet connected：" + echoServer);
                if (successCallback != null)
                {
                    successCallback();
                }
            }
            else
            {
                DebugExt.Log("internet not connected：" + echoServer);
                if (failCallback != null)
                {
                    failCallback();
                }
            }
        }

 

    }
}