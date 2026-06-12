using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZGame;

public class IOAndroidLoader : Singleton<IOAndroidLoader>
{
    AndroidJavaClass readAsset = new AndroidJavaClass("com.zgame.sdk.ReadAsset");
    public IOAndroidLoader()
    {
        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            object jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
            readAsset.CallStatic("init", jo);
        }
    }

    public bool IsFileExist(string path)//path为StreamingAssets文件夹内的全路径
    {
        return readAsset.CallStatic<bool>("isFileExists", path);
    }

    public byte[] GetBytes(string path)
    {
        //Debug.Log("begin call android LoadFile, path:" + path);
        return readAsset.CallStatic<byte[]>("readFile", path);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="path">为StreaminAssets目录下的具体路径</param>
    /// <returns></returns>
    public string GetText(string path)
    {
        byte[] bytes = GetBytes(path);
        if (bytes == null)
        {
            Debug.LogError("error,LoadAndroidFileText failed, path:" + path);
        }
        return System.Text.Encoding.UTF8.GetString(bytes);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="originPath">为StreaminAssets目录下的具体路径</param>
    /// <param name="destPath">详细的目标路径，要保证该路径可写</param>
    public void CopyFile(string originPath, string destPath)
    {
        var datas = GetBytes(originPath);
        File.WriteAllBytes(destPath, datas);
    }

}
