using BestHTTP;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using ZGame;
using ZGame.Obfuscation;
using DES = ZGame.Obfuscation.DES;

public class HttpTool
{
    public static void UploadFile(string url, string fieldName, byte[] file, string fileName, Action<string> onComp, Action<string> onError)
    {
        HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, false, true, (HTTPRequest originalReq, HTTPResponse response) =>
        {
            switch (originalReq.State)
            {
                case HTTPRequestStates.Initial:
                    break;
                case HTTPRequestStates.Queued:
                    break;
                case HTTPRequestStates.Processing:
                    break;
                case HTTPRequestStates.Finished:
                    int statusCode = response.StatusCode;
                    if (statusCode != 200)
                    {
                        onError("status code not 200, is:" + statusCode);
                    }
                    else
                    {
                        onComp("upload file success," + fileName);
                    }
                    break;
                case HTTPRequestStates.Error:
                    onError("status error");
                    break;
                case HTTPRequestStates.Aborted://对应网络断开
                    onError("http post  net aborted, url:" + url);
                    break;
                case HTTPRequestStates.ConnectionTimedOut:
                    onError("connect timeout！url:" + url);
                    break;
                case HTTPRequestStates.TimedOut://超时
                    onError("timeout: " + url);
                    break;


            }
        });
        BestHTTP.Forms.HTTPMultiPartForm form = new BestHTTP.Forms.HTTPMultiPartForm();
        form.AddBinaryData(fieldName, file, fileName);
        request.SetForm(form);
        request.ConnectTimeout = TimeSpan.FromSeconds(3);
        request.Timeout = TimeSpan.FromSeconds(3);
        request.Send();
    }


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
                            onError(id, "1 http request failed, statusCode=" + statusCode);
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


    public static void DownloadSubGame(string url, long id, string subgamename, Action<long, string, byte[]> onCmp = null, Action<long, string, string> onError = null, Action<long, string, long, long> onUpdate = null, int timeout = 4000)//timeout设置大点，防止下大文件超时
    {
        HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Get, false, true,
            (HTTPRequest originalRequest, HTTPResponse response) =>
            {
                switch (originalRequest.State)
                {
                    case HTTPRequestStates.Initial:
                        break;
                    case HTTPRequestStates.Queued:
                        break;
                    case HTTPRequestStates.Processing:
                        break;
                    case HTTPRequestStates.Finished:
                        int statusCode = response.StatusCode;
                        if (statusCode != 200)
                        {
                            onError(id, subgamename, "2 http request failed, statusCode=" + statusCode);
                        }
                        else
                        {
                            //onCmp(id, response.DataAsText);
                            onCmp(id, subgamename, response.Data);
                        }
                        break;
                    case HTTPRequestStates.Error:
                        onError(id, subgamename, originalRequest.Exception.Message);
                        break;
                    case HTTPRequestStates.Aborted://对应网络断开                      
                        onError(id, subgamename, "net unconnect！");
                        break;
                    case HTTPRequestStates.ConnectionTimedOut:
                        onError(id, subgamename, "connect timeout！！");
                        break;
                    case HTTPRequestStates.TimedOut://超时                      
                        onError(id, subgamename, "timeout！");
                        break;
                    default:
                        break;
                }
            });
        request.Timeout = TimeSpan.FromSeconds(timeout);
        request.OnDownloadProgress = (req, downloaded, downloadLength) =>
        {
            if (onUpdate != null)
            {
                onUpdate(id, subgamename, downloaded, downloadLength);
            }
        };
        request.Send();
    }


    public static void Post(string url, long id, string data = "", Action<long, byte[]> onCmp = null, Action<long, string> onError = null, int timeout = 6, Dictionary<string, string> headerDic = null)
    {
        byte[] dataByte = System.Text.Encoding.Default.GetBytes(data);
        Post(url, id, dataByte, onCmp, onError, timeout, headerDic);
    }

    public static void Post(string url, long id, byte[] data = null, Action<long, byte[]> onCmp = null, Action<long, string> onError = null, int timeout = 6, Dictionary<string, string> headerDic = null)
    {
        int maxConnectCount = 3;
        int connectCount = 0;
        HTTPRequest request = new HTTPRequest(new Uri(url), HTTPMethods.Post, false, true,
            (HTTPRequest originalRequest, HTTPResponse response) =>
            {
                switch (originalRequest.State)
                {
                    case HTTPRequestStates.Initial:
                        break;
                    case HTTPRequestStates.Queued:
                        break;
                    case HTTPRequestStates.Processing:
                        break;
                    case HTTPRequestStates.Finished:
                        int statusCode = response.StatusCode;
                        if (statusCode != 200)
                        {
                            onError(id, "3 http request failed, statusCode=" + statusCode);
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
                            onError(id, "connect timeout！");
                        }
                        break;
                    case HTTPRequestStates.TimedOut://超时
                        onError(id, "timeout！");
                        //Debug.Log(url + " 超时，继续连");
                        //originalRequest.Send();
                        break;
                    default:
                        Debug.Log("error occur:" + originalRequest.State.ToString());
                        break;
                }
            });
        request.ConnectTimeout = TimeSpan.FromSeconds(10);
        request.Timeout = TimeSpan.FromSeconds(timeout);
        request.RawData = data;


        if (headerDic != null)
        {
            foreach (var item in headerDic)
            {
                request.AddHeader(item.Key, item.Value);
            }

        }

        request.Send();
    }

    /// <summary>
    /// HTTP协议加密方式
    /// 对于上行：
    /// 先生成16位的字符串B，然后对内容进行des加密(取B的前8位作为秘钥)得到字符串A
    /// 组装新字符串C=BA
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    public static string HTTPEncrypt(string content)
    {
        string B = RandomTool.RandomStr(16);
        string C = B + DES.EncryptStrToBase64(content, B.Substring(0, 8));
        return C;
    }

}