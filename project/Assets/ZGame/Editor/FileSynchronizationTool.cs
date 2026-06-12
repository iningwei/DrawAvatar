
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

namespace ZGame.RessEditor
{
    public class FileSynchronizationTool
    {

        [MenuItem("Assets/ZGame/双客户端文件同步/全量同步")]
        public static void Build()
        {
            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            if (assets.Length < 0)
            {
                EditorUtility.DisplayDialog("对象错误", "当前未选择任何文件或文件夹！！！", "确认");
                return;
            }

            if (EditorUtility.DisplayDialog("提示", " 是否确定同步--->是否继续??？", "OK", "Cancel"))
            {
                for (int i = 0; i < assets.Length; i++)
                {
                    string fileDir = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets")) + AssetDatabase.GetAssetPath(assets[i]);
                    Debug.Log(fileDir);
                    if (Directory.Exists(fileDir))
                    {
                        LsFolder(fileDir);
                    }
                    else if (File.Exists(fileDir))
                    {
                        FileSynchronization(fileDir);
                    }
                    else
                    {
                        Debug.LogError($"路径错误！---{fileDir}");
                    }

                    if (File.Exists(fileDir + ".meta"))
                    {
                        FileSynchronization(fileDir + ".meta");
                    }
                }
            }


        }

        private static void FileSynchronization(string path)
        {
            string basepath = Path.GetDirectoryName(path);
            // basepath.Replace(@"\\","/");
            // Debug.LogError(basepath);
            if (basepath.Contains("\\art_project\\"))
            {
                basepath = basepath.Replace("\\art_project\\", "\\project\\");
            }
            else
            {
                basepath = basepath.Replace("\\project\\", "\\art_project\\");
            }
            try
            {
                if (!Directory.Exists(basepath))
                {
                    Directory.CreateDirectory(basepath);
                }

            }
            catch (IOException e)
            {
                Debug.LogError($"文件夹{basepath}创建失败:" + e);
                return;
            }
            string targetPath = basepath + "\\" + Path.GetFileName(path);

            try
            {

                Debug.Log($"文件路径：\n原文件--{path}\n现文件--{targetPath} ");
                File.Copy(path, targetPath, true);
                Debug.Log($"文件{path}复制成功！");
            }
            catch (IOException e)
            {
                Debug.LogError($"文件{path}复制失败：" + e);
            }

        }

        private static void LsFolder(string path)
        {
            string TargetPath = "";
            if (path.Contains("/art_project/"))
            {
                TargetPath = path.Replace("/art_project/", "/project/");
            }
            else
            {
                TargetPath = path.Replace("/project/", "/art_project/");
            }
            Debug.LogError("文件夹名称：" + TargetPath);
            try
            {
                if (!Directory.Exists(TargetPath))
                {
                    Directory.CreateDirectory(TargetPath);
                }
            }
            catch (IOException e)
            {
                Debug.LogError("文件夹创建失败；---" + e);
                return;
            }
            DirectoryInfo direction = new DirectoryInfo(path);
            FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
            foreach (var item in files)
            {
                FileSynchronization(item.FullName);

            }
            DirectoryInfo[] folders = direction.GetDirectories("*", SearchOption.TopDirectoryOnly);

            foreach (var item in folders)
            {
                try
                {
                    if (!Directory.Exists(item.FullName))
                    {
                        Directory.CreateDirectory(item.FullName);
                    }
                }
                catch (IOException e)
                {
                    Debug.LogError("文件夹创建失败；---" + e + "\nitem.FullName");
                    continue;
                }

            }
        }
    }
}
