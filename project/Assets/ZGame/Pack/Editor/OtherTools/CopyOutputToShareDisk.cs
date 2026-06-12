using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEditor.PackageManager;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class CopyToShareDisk
{

    public static void DoCopyUseWebClient(string sourceFolderPath, string remoteDiskPath, string userName, string password)
    {
        /// 创建WebClient实例  
        System.Net.WebClient client = new WebClient();
        client.Credentials = new NetworkCredential(userName, password);//指定用户名和密码
        client.Credentials = CredentialCache.DefaultCredentials;


        // 获取文件夹中的所有文件
        string[] files = Directory.GetFiles(sourceFolderPath);
        // 遍历文件并上传
        foreach (string file in files)
        {
            // 获取文件名
            string fileName = Path.GetFileName(file);

            // 构造上传的完整 URL
            string uploadUrl = remoteDiskPath + "/" + fileName;

            try
            {
                // 上传文件
                client.UploadFile(uploadUrl, "POST", file);
                Debug.Log("上传文件 " + fileName + " 成功");
            }
            catch (WebException e)
            {
                Debug.LogError("上传文件 " + fileName + " 失败：" + e.Message);
            }
        }
    }


    //在Unity中上传文件到局域网的SMB（Server Message Block）协议共享文件夹
    //可以使用System.IO命名空间下的File.Copy方法来实现。这种方法可以将本地文件复制到共享文件夹中。
    public static void DoCopy2SMB(string sourceFolderPath, string remoteDiskPath)
    {
        string[] files = Directory.GetFiles(sourceFolderPath);
        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            if (!Directory.Exists(remoteDiskPath))
            {
                Directory.CreateDirectory(remoteDiskPath);
            }
            // 构造上传的完整 URL
            string uploadUrl = remoteDiskPath + "/" + fileName;
            try
            {
                File.Copy(file, uploadUrl, true);
                Debug.Log("上传文件 " + fileName + " ,到SMB服务器 成功");
            }
            catch (Exception e)
            {
                Debug.LogError("上传文件 " + fileName + ",到SMB服务器 失败：" + e.Message);
            }
        }
    }
}
