using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using ZGame;

//public class CertificateWhore : CertificateHandler
//{
//    protected override bool ValidateCertificate(byte[] certificateData)
//    {
//        return true;
//    }
//}


public class UnityWebRequestMgr : SingletonMonoBehaviour<UnityWebRequestMgr>
{

    /*
    UnityWebRequestMgr uwr = new UnityWebRequestMgr();
    uwr.url = "http://www.mysite.com";
    uwr.method = UnityWebRequestMgr.kHttpVerbGET;   // can be set to any custom method, common constants privided
    uwr.useHttpContinue = false;
    uwr.chunkedTransfer = false;
    uwr.redirectLimit = 0;  // disable redirects
    uwr.timeout = 60;       // don't make this small, web requests do take some time 
    */

    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url"></param>
    /// <param name="action"></param>
    public void Get(string url, Action<UnityWebRequest> actionResult)
    {
        StartCoroutine(_Get(url, actionResult));
    }
    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="downloadFilePathAndName">储存文件的路径和文件名 like 'Application.persistentDataPath+"/unity3d.html"'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    public void DownloadFile(string url, string downloadFilePathAndName, Action<UnityWebRequest> actionResult)
    {
        StartCoroutine(_DownloadFile(url, downloadFilePathAndName, actionResult));
    }
    /// <summary>
    /// 请求图片
    /// </summary>
    /// <param name="url">图片地址,like 'http://www.my-server.com/image.png '</param>
    /// <param name="action">请求发起后处理回调结果的委托,处理请求结果的图片</param>
    /// <returns></returns>
    public void GetTexture(string url, Action<Texture2D> actionResult)
    {
        StartCoroutine(_GetTexture(url, actionResult));
    }
    /// <summary>
    /// 请求AssetBundle
    /// </summary>
    /// <param name="url">AssetBundle地址,like 'http://www.my-server.com/myData.unity3d'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AssetBundle</param>
    /// <returns></returns>
    public void GetAssetBundle(string url, Action<AssetBundle> actionResult)
    {
        StartCoroutine(_GetAssetBundle(url, actionResult));
    }
    /// <summary>
    /// 请求服务器地址上的音效
    /// </summary>
    /// <param name="url">没有音效地址,like 'http://myserver.com/mysound.wav'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AudioClip</param>
    /// <param name="audioType">音效类型</param>
    /// <returns></returns>
    public void GetAudioClip(string url, Action<AudioClip> actionResult, AudioType audioType = AudioType.WAV)
    {
        StartCoroutine(_GetAudioClip(url, actionResult, audioType));
    }


    /// <summary>
    /// 向服务器提交post请求
    /// </summary>
    /// <param name="serverURL">服务器请求目标地址,like "http://www.my-server.com/myform"</param>
    /// <param name="lstformData">form表单参数</param>
    /// <param name="lstformData">处理返回结果的委托,处理请求对象</param>
    /// <returns></returns>
    public void Post(string serverURL, List<IMultipartFormSection> lstformData, Action<UnityWebRequest> actionResult)
    {
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));

        StartCoroutine(_Post(serverURL, lstformData, actionResult));
    }
    /// <summary>
    /// 通过PUT方式将字节流传到服务器
    /// </summary>
    /// <param name="url">服务器目标地址 like 'http://www.my-server.com/upload' </param>
    /// <param name="contentBytes">需要上传的字节流</param>
    /// <param name="resultAction">处理返回结果的委托</param>
    /// <returns></returns>
    public void UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult)
    {
        StartCoroutine(_UploadByPut(url, contentBytes, actionResult, ""));
    }
    /// <summary>
    /// GET请求
    /// </summary>
    /// <param name="url">请求地址,like 'http://www.my-server.com/ '</param>
    /// <param name="action">请求发起后处理回调结果的委托</param>
    /// <returns></returns>
    IEnumerator _Get(string url, Action<UnityWebRequest> actionResult)
    {
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            if (actionResult != null)
            {
                actionResult(uwr);
            }
        }
    }
    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="url">请求地址</param>
    /// <param name="downloadFilePathAndName">储存文件的路径和文件名 like 'Application.persistentDataPath+"/unity3d.html"'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求对象</param>
    /// <returns></returns>
    IEnumerator _DownloadFile(string url, string downloadFilePathAndName, Action<UnityWebRequest> actionResult)
    {
        var uwr = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET);
        uwr.downloadHandler = new DownloadHandlerFile(downloadFilePathAndName);
        yield return uwr.SendWebRequest();
        if (actionResult != null)
        {
            actionResult(uwr);
        }
    }

    //判断是否WebP文件
    bool isWebP(byte[] imageData)
    {
        if (imageData.Length < 12)
        {
            return false;
        }
        byte[] webpHeader = { 0x52, 0x49, 0x46, 0x46 };
        for (int i = 0; i < webpHeader.Length; i++)
        {
            if (imageData.Length <= i || imageData[i] != webpHeader[i])
            {
                return false;
            }
        }

        byte[] webpIdentifier = { 0x57, 0x45, 0x42, 0x50 };
        for (int i = 8; i < 12; i++)
        {
            if (imageData.Length <= i || imageData[i] != webpIdentifier[i - 8])
            {
                return false;
            }
        }
        return true;
    }


    //常见报错Curl error 60: Cert verify failed: UNITYTLS_X509VERIFY_FLAG_USER_  （遇到的是访问外网需要开梯子的图片，梯子不通的时候会报这个错） 
    IEnumerator _GetTexture(string url, Action<Texture2D> actionResult)
    {
        UnityWebRequest uwr = new UnityWebRequest(url);
        //uwr.certificateHandler = new CertificateWhore();
        //解决报错问题--->https://answers.unity.com/questions/1874008/curl-error-60-cert-verify-failed-unitytls-x509veri-1.html

        DownloadHandlerTexture downloadTexture = new DownloadHandlerTexture(true);//true will set texture readable
        uwr.downloadHandler = downloadTexture;
        yield return uwr.SendWebRequest();
        Texture2D t = null;

        if (!(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError))
        {
#if WebP
            var byteDatas = downloadTexture.data;
            if (isWebP(byteDatas))
            {                
                Texture2D texture = WebP.Texture2DExt.CreateTexture2DFromWebP(byteDatas, lMipmaps: true, lLinear: false, lError: out WebP.Error lError, scalingFunction: null, makeNoLongerReadable: false);
                if (lError == WebP.Error.Success)
                {
                    t = texture;
                }
                else
                {
                    Debug.LogError("Webp Load Error : " + lError.ToString());
                }
            }
            else
            {                
                t = downloadTexture.texture;
            }
#else
            t = downloadTexture.texture;
#endif
        }
        else
        {
            string error = uwr.error;
            Debug.LogError("get texture:" + url + " , error:" + error);
        }
        if (actionResult != null)
        {
            actionResult(t);
        }
    }

    /// <summary>
    /// 请求AssetBundle
    /// </summary>
    /// <param name="url">AssetBundle地址,like 'http://www.my-server.com/myData.unity3d'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AssetBundle</param>
    /// <returns></returns>
    IEnumerator _GetAssetBundle(string url, Action<AssetBundle> actionResult)
    {
        UnityWebRequest www = new UnityWebRequest(url);
        DownloadHandlerAssetBundle handler = new DownloadHandlerAssetBundle(www.url, uint.MaxValue);
        www.downloadHandler = handler;
        yield return www.SendWebRequest();
        AssetBundle bundle = null;
        if (!(www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError))
        {
            bundle = handler.assetBundle;
        }
        if (actionResult != null)
        {
            actionResult(bundle);
        }
    }

    /// <summary>
    /// 请求服务器地址上的音效
    /// </summary>
    /// <param name="url">没有音效地址,like 'http://myserver.com/mysound.wav'</param>
    /// <param name="actionResult">请求发起后处理回调结果的委托,处理请求结果的AudioClip</param>
    /// <param name="audioType">音效类型</param>
    /// <returns></returns>
    IEnumerator _GetAudioClip(string url, Action<AudioClip> actionResult, AudioType audioType = AudioType.WAV)
    {
        using (var uwr = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
        {
            yield return uwr.SendWebRequest();
            if (!(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError))
            {
                if (actionResult != null)
                {
                    actionResult(DownloadHandlerAudioClip.GetContent(uwr));
                }
            }
        }
    }

    /// <summary>
    /// 向服务器提交post请求
    /// </summary>
    /// <param name="serverURL">服务器请求目标地址,like "http://www.my-server.com/myform"</param>
    /// <param name="lstformData">form表单参数</param>
    /// <param name="lstformData">处理返回结果的委托</param>
    /// <returns></returns>
    IEnumerator _Post(string serverURL, List<IMultipartFormSection> lstformData, Action<UnityWebRequest> actionResult)
    {
        //List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        //formData.Add(new MultipartFormDataSection("field1=foo&field2=bar"));
        //formData.Add(new MultipartFormFileSection("my file data", "myfile.txt"));
        UnityWebRequest uwr = UnityWebRequest.Post(serverURL, lstformData);
        yield return uwr.SendWebRequest();
        if (actionResult != null)
        {
            actionResult(uwr);
        }
    }


    /// <summary>
    /// 通过PUT方式将字节流传到服务器
    /// </summary>
    /// <param name="url">服务器目标地址 like 'http://www.my-server.com/upload' </param>
    /// <param name="contentBytes">需要上传的字节流</param>
    /// <param name="resultAction">处理返回结果的委托</param>
    /// <param name="resultAction">设置header文件中的Content-Type属性</param>
    /// <returns></returns>
    IEnumerator _UploadByPut(string url, byte[] contentBytes, Action<bool> actionResult, string contentType = "application/octet-stream")
    {
        UnityWebRequest uwr = new UnityWebRequest();
        UploadHandler uploader = new UploadHandlerRaw(contentBytes);

        // Sends header: "Content-Type: custom/content-type";
        uploader.contentType = contentType;

        uwr.uploadHandler = uploader;

        yield return uwr.SendWebRequest();

        bool res = true;
        if (!(uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError))
        {
            res = false;
        }
        if (actionResult != null)
        {
            actionResult(res);
        }
    }



    //如果表单中传入的是中文，那么服务器接收到的是乱码，解决办法Debug.Log(PostWebRequest("http://192.168.1.1/EntryformServlet", "weixinid=测试&pwd=1234567" ));
    //private string PostWebRequest(string postUrl, string paramData)
    //{
    //    // 把字符串转换为bype数组
    //    byte[] bytes = Encoding.GetEncoding("gb2312").GetBytes(paramData);

    //    HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
    //    webReq.Method = "POST";
    //    webReq.ContentType = "application/x-www-form-urlencoded;charset=gb2312";
    //    webReq.ContentLength = bytes.Length;
    //    using (Stream newStream = webReq.GetRequestStream())
    //    {
    //        newStream.Write(bytes, 0, bytes.Length);
    //    }
    //    using (WebResponse res = webReq.GetResponse())
    //    {
    //        //在这里对接收到的页面内容进行处理
    //        Stream responseStream = res.GetResponseStream();
    //        StreamReader streamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
    //        string str = streamReader.ReadToEnd();
    //        streamReader.Close();
    //        responseStream.Close();
    //        //返回：服务器响应流 
    //        return str;
    //    }
    //}


}
