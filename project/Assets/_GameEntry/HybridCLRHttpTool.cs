using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HybridCLRHttpTool
{
    public static void Get(string url, long id, Action<long, byte[]> onCmp = null, Action<long, string> onError = null, Action<long, long> onUpdate = null, int timeout = 4000)//timeout设置大一点，防止下载大 文件超时
    {

        int maxConnectCount = 3;
        int connectCount = 0;
        HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Get, false, true,
            (HTTPRequest originalRequest, HTTPResponse response) =>
            {
                switch (originalRequest.State)
                {
                    case HTTPRequestStates.Initial:
                        //Debug.Log("   request Initial");
                        break;
                    case HTTPRequestStates.Queued:
                        //Debug.Log("   request Queued");
                        break;
                    case HTTPRequestStates.Processing:
                        //Debug.Log("   request Processing");
                        break;
                    case HTTPRequestStates.Finished://完成(文件不存在404错误等也会进入Finished State)                        
                        int statusCode = response.StatusCode;
                        if (statusCode != 200)
                        {
                            string errorMsg = "4 http request failed, statusCode=" + statusCode;
#if UNITY_EDITOR
                            errorMsg += ", url:" + url;
#endif
                            onError(id, errorMsg);
                        }
                        else
                        {
                            //onCmp(id, response.DataAsText);
                            onCmp(id, response.Data);
                        }
                        break;
                    case HTTPRequestStates.Error:
                        onError(id, originalRequest.Exception.Message);
                        break;
                    case HTTPRequestStates.Aborted://对应网络断开 
                        onError(id, "net unconnect！");
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        connectCount++;
                        if (connectCount <= maxConnectCount)
                        {
                            Debug.Log("connect timeout，continue connect！cur connect times：" + connectCount);
                            originalRequest.Send();
                        }
                        else
                        {
                            onError(id, "connect timeout！！");
                        }
                        break;
                    case HTTPRequestStates.TimedOut://超时
                        Debug.Log("connect timeout，continue connect！！");
                        originalRequest.Send();
                        break;
                    default:
                        break;
                }
            });
        request.ConnectTimeout = TimeSpan.FromSeconds(3);
        request.Timeout = TimeSpan.FromSeconds(timeout);

        request.OnDownloadProgress = (req, downloaded, downloadLength) =>
        {
            if (onUpdate != null)
            {
                onUpdate(downloaded, downloadLength);
            }
        };



        request.Send();

    }

}
