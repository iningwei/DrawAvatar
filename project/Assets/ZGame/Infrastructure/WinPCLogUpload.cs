using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WinPCLogUpload
{
    public static void UploadToServer(string url, Action onSuccess, Action onFail)
    {
        string productName = Application.productName;
        string deviceName = SystemInfo.deviceName;

        string path = Application.persistentDataPath + "/Player.log";
        if (File.Exists(path))
        {
            string copyPath = Application.persistentDataPath + "/Player-Copy.log";
            File.Copy(path, copyPath, true);
            var datas = File.ReadAllBytes(copyPath);//防止报错：IOException: Sharing violation on path，故先copy一份
            string desFileName = productName + "_" + deviceName + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + "_Player" + ".txt";
            Debug.Log("desFileName:" + desFileName);


            HttpTool.UploadFile(url, "file", datas, desFileName, (str) =>
            {
                Debug.Log(str);
                onSuccess?.Invoke();
            }, (str) =>
            {
                Debug.LogError(str);
                onFail?.Invoke();
            });

            //拷贝文件到桌面
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + productName + "_" + deviceName + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + "_Player.log";
            File.Copy(copyPath, desktopPath, true);
        }
        else
        {
            Debug.LogError("not exist file:" + path);
        }
    }
}
