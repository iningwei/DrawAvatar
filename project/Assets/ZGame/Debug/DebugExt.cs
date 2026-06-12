using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using UnityEngine;

namespace ZGame
{
    public class DebugExt
    {
        static int maxCount = 200;

        static Queue<string> queues = new Queue<string>();
        static string productName;
        static string deviceName;
        static bool isInit = false;
        static string pPath = Application.persistentDataPath;
        static string debugFilePath;
        static void Init()
        {
            productName = Application.productName;
            deviceName = SystemInfo.deviceName;
            debugFilePath = pPath + "/" + productName + "_" + deviceName + ".txt";


            if (File.Exists(debugFilePath))
            {
                string[] lines = File.ReadAllLines(debugFilePath);
                for (int i = 0; i < lines.Length; i++)
                {
                    queues.Enqueue(lines[i]);
                }
            }
            isInit = true;
        }
        static string formatContent(string content)
        {
            content = content + ">stamp:" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now, true) + ",frame:" + Time.frameCount;
            return content;
        }
        public static void Log(string content)
        {
            content = formatContent(content);
            Debug.Log(content);
            TraceDebug(content);
        }

        public static void LogW(string content)
        {
            content = formatContent(content);
            Debug.LogWarning(content);
            TraceDebug(content);
        }

        public static void LogE(string content, UnityEngine.Object context = null)
        {
            content = formatContent(content);
            Debug.LogError(content, context);
            TraceDebug(content);
        }

        public static void UploadDebugToServer(string url, string fileNameExt, Action onSuccess, Action onFail)
        {
            string contents = "";
            foreach (var item in queues)
            {
                contents += item + "\r\n";
            }

            IOTools.WriteString(debugFilePath, contents);
            byte[] data = IOTools.ReadFile(debugFilePath);
            //和后端协议的表单字段为 file
            HttpTool.UploadFile(url, "file", data, productName + "_" + deviceName + "_" + fileNameExt + "_" + TimeTool.GetyyyyMMddHHmmssfff(DateTime.Now) + ".txt", (str) =>
            {
                Log(str);
                onSuccess?.Invoke();
            }, (str) =>
            {
                LogE(str);
                onFail?.Invoke();
            });
        }


        static void TraceDebug(string content)
        {
            if (isInit == false)
            {
                Init();
            }

            if (ConfigUtility.Data.IsEnableLogTrace)
            {
                if (queues.Count == maxCount)
                {
                    queues.Dequeue();
                }
                queues.Enqueue(content);

                if (ConfigUtility.Data.IsEnableLogRealtimeWriteToLocal)
                {
                    string contents = "";
                    foreach (var item in queues)
                    {
                        contents += item + "\r\n";
                    }
                    string path = pPath + "/" + productName + ".txt";
                    IOTools.WriteString(path, contents);
                }
            }
        }
    }
}