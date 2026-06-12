using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Obfuscation;
using ZGame.Ress.AB.Holder;

namespace ZGame.RessEditor
{
    public class BuildCommandWithLog
    {
        static List<ABBuildLogData> abBuildLogList = new List<ABBuildLogData>();
        [MenuItem("工具/打包/打ab包/打批量ab带log/对所有窗体打AB包")]
        public static void BuildAllWindow()
        {
            BuildConfig.Init();
            if (!BuildCommand.CheckCommonRes())
            {
                return;
            }

            readABBuildLog();
            List<ABBuildLogData> abBuildLogNew = new List<ABBuildLogData>();

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/ArtResources" });
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string guid = prefabGuids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("temp_for_prefab"))
                {
                    continue;
                }
                if (!path.EndsWith("Window.prefab"))
                {
                    continue;
                }

                string fullPath = Application.dataPath + path.Replace("Assets", "");
                string fileName = Path.GetFileNameWithoutExtension(fullPath);

                GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (asset != null)
                {
                    var holder = asset.GetComponent<RootCompInfoHolder>();
                    if (holder != null)
                    {
                        string md5Value;
                        if (isMD5TheSame(fullPath, path, out md5Value))
                        {
                            Debug.Log($"{path} is newest, no need buildAB");
                            continue;
                        }

                        Debug.Log("begin build ab for:" + fullPath);
                        var buildFunc = BuildConfig.getBuildFunc(path, asset);
                        if (buildFunc != null)
                        {
                            bool r = buildFunc(asset);
                            if (r)
                            {
                                abBuildLogNew.Add(new ABBuildLogData(path, md5Value));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("no RootCompInfoHolder attached to:" + path);
                    }
                }
            }

            BuildCommand.DeleteUselessAfterBuildAB();

            //更新日志
            updateABBuildLog(abBuildLogNew);

            Debug.Log("打所有窗体AB包完毕");
        }


        [MenuItem("工具/打包/打ab包/打批量ab带log/对所有非窗体预制件打AB包")]
        public static void BuildAllNonWindowPrefab()
        {
            BuildConfig.Init();
            if (!BuildCommand.CheckCommonRes())
            {
                return;
            }

            readABBuildLog();
            List<ABBuildLogData> abBuildLogNew = new List<ABBuildLogData>();

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/ArtResources" });
            for (int i = 0; i < prefabGuids.Length; i++)
            {
                string guid = prefabGuids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);

                if (path.EndsWith("Window.prefab") || path.Contains("ArtResources/Window")
                    || path.EndsWith("ShaderList.prefab"))
                {
                    continue;
                }

                string fullPath = Application.dataPath + path.Replace("Assets", "");
                string fileName = Path.GetFileNameWithoutExtension(fullPath);


                GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                if (asset != null)
                {
                    var holder = asset.GetComponent<RootCompInfoHolder>();
                    if (holder != null)
                    {
                        string md5Value;
                        if (isMD5TheSame(fullPath, path, out md5Value))
                        {
                            Debug.Log($"{path} is newest, no need buildAB");
                            continue;
                        }

                        Debug.Log("begin build ab for:" + fullPath);
                        var buildFunc = BuildConfig.getBuildFunc(path, asset);
                        if (buildFunc != null)
                        {
                            bool r = buildFunc(asset);
                            if (r)
                            {
                                abBuildLogNew.Add(new ABBuildLogData(path, md5Value));
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError("no RootCompInfoHolder attached to:" + path);
                    }
                }
            }

            BuildCommand.DeleteUselessAfterBuildAB();

            //更新日志
            updateABBuildLog(abBuildLogNew);
            Debug.Log("打所有非窗体预制件AB包完毕");
        }

        [MenuItem("工具/打包/打ab包/打批量ab带log/对所有Texture打AB包")]
        public static void BuildAllTexture()
        {
            readABBuildLog();
            List<ABBuildLogData> abBuildLogNew = new List<ABBuildLogData>();

            var textureGuids = AssetDatabase.FindAssets("t:Texture", new[] { "Assets/ArtResources/Texture" });
            for (int i = 0; i < textureGuids.Length; i++)
            {
                string guid = textureGuids[i];
                string path = AssetDatabase.GUIDToAssetPath(guid);

                string allPath = Application.dataPath + path.Replace("Assets", "");
                string fileName = Path.GetFileNameWithoutExtension(allPath);

                Texture asset = AssetDatabase.LoadAssetAtPath<Texture>(path);


                string md5Value;
                if (isMD5TheSame(allPath, path, out md5Value))
                {
                    Debug.Log($"{path} is newest, no need buildAB");
                    continue;
                }
                var buildFunc = BuildConfig.getBuildFunc(path, asset);
                if (buildFunc != null)
                {
                    bool r = buildFunc(asset);
                    if (r)
                    {
                        abBuildLogNew.Add(new ABBuildLogData(path, md5Value));
                    }
                }
            }

            BuildCommand.DeleteUselessAfterBuildAB();
            //更新日志
            updateABBuildLog(abBuildLogNew);
            Debug.Log("对所有texture打ab完成");
        }
        private static void updateABBuildLog(List<ABBuildLogData> abBuildLogNew)
        {
            if (abBuildLogNew.Count > 0)
            {
                List<ABBuildLogData> addTarget = new List<ABBuildLogData>();
                for (int i = 0; i < abBuildLogNew.Count; i++)
                {
                    ABBuildLogData newData = abBuildLogNew[i];

                    for (int j = 0; j < abBuildLogList.Count; j++)
                    {
                        ABBuildLogData tmp = abBuildLogList[j];
                        if (tmp.originResPath == newData.originResPath)
                        {
                            tmp.md5Value = newData.md5Value;
                            tmp.timeDes = newData.timeDes;
                            break;
                        }
                    }

                    addTarget.Add(newData);
                }

                abBuildLogList.AddRange(addTarget);
            }

            string logFilePath = Application.dataPath + "/ZGame/Ress/ABBuildLogs/" +
         IOTools.PlatformFolderName + "/ablog.txt";
            IOTools.CreateFileSafe(logFilePath);

            string contents = "";
            for (int i = 0; i < abBuildLogList.Count; i++)
            {
                contents = contents + abBuildLogList[i].originResPath + "@";
                contents = contents + abBuildLogList[i].md5Value + "@";
                contents = contents + abBuildLogList[i].timeDes + "\r\n";
            }
            IOTools.WriteString(logFilePath, contents);
        }

        static bool isMD5TheSame(string fullPath, string resPath, out string md5Value)
        {
            ABBuildLogData targetLogData = null;
            for (int i = 0; i < abBuildLogList.Count; i++)
            {
                if (abBuildLogList[i].originResPath == resPath)
                {
                    targetLogData = abBuildLogList[i];
                    break;
                }
            }
            var bytes = File.ReadAllBytes(fullPath);
            string md5 = MD5.Get(bytes);
            md5Value = md5;
            if (targetLogData != null)
            {
                if (targetLogData.md5Value == md5)
                {
                    return true;
                }
            }
            return false;
        }
        static void readABBuildLog()
        {
            abBuildLogList.Clear();

            string logFilePath = Application.dataPath + "/ZGame/Ress/ABBuildLogs/" +
          IOTools.PlatformFolderName + "/ablog.txt";
            if (File.Exists(logFilePath))
            {
                var lines = File.ReadAllLines(logFilePath);
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line) && line.Contains("@"))
                    {
                        var strs = line.Split('@');
                        string resPath = strs[0];
                        string md5Value = strs[1];
                        abBuildLogList.Add(new ABBuildLogData(resPath, md5Value));
                    }
                }
            }
        }
    }
}