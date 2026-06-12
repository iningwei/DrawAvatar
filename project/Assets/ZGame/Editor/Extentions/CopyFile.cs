using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Diagnostics;
using ZGame.Obfuscation;
using System.Text.RegularExpressions;
using ZGame;

public class CopyFile : MonoBehaviour
{

    [MenuItem("工具/拷贝美术工程需要内容到客户端工程")]
    public static void CopyArtProj2ClientProj()
    {
        string pattern = "/project/Assets";
        if (Regex.IsMatch(Application.dataPath, pattern))
        {
            EditorUtility.DisplayDialog("拷贝无法执行", "当前处于项目工程，禁止使用该功能！", "知道了");
            print("当前处于项目工程，不允许使用该功能");
            return;
        }
        if (ConfigUtility.Data.IsABResNameCrypto == false)
        {
            string path = Application.dataPath + "/../BatCmd/CopyArtProj2ClientProj.bat";
            Process.Start(path);
        }
        else
        {
            //从美术工程往项目工程拷贝时对ab资源加密，故取消上述bat命令的方式
            string sourcePath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
            string targetPath = Application.dataPath + "/../../Project/ResEx/" + IOTools.PlatformFolderName;

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
                UnityEngine.Debug.Log("create targetpath:" + targetPath);
            }

            string[] files = Directory.GetFiles(sourcePath);
            int totalCount = 0;

            //二进制文件头增加3字节，同时混淆其名称
            int numberOfBytes = ConfigUtility.Data.ABResByteOffset;
            byte newByte = 0x3;
            for (int i = 0; i < files.Length; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                string fileExt = Path.GetExtension(files[i]);
                if (fileExt == "manifest" || fileName == "android")
                {
                    continue;
                }
                totalCount++;

                //对文件名进行混淆
                //Debug.LogError("oldFileName:" + fileName);
                string newFileName = ConfigUtility.Data.IsABResNameCrypto ? DES.EncryptStrToHex(fileName, ConfigUtility.Data.ABResNameCryptoKey) : fileName;
                //Debug.LogError("newFileName:" + newFileName);

                string targetFile = targetPath + "/" + newFileName + fileExt;
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }

                using (var newFile = new FileStream(targetFile, FileMode.Create, FileAccess.Write))
                {
                    for (int j = 0; j < numberOfBytes; j++)
                    {
                        newFile.WriteByte(newByte);
                    }
                    using (var oldFile = new FileStream(files[i], FileMode.Open, FileAccess.Read))
                    {
                        oldFile.CopyTo(newFile);
                    }
                }
            }

            UnityEngine.Debug.Log("移动文件总数：" + totalCount);
        }

    }

}
