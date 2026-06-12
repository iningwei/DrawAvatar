using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class Altertpsheet 
{
    private static List<string> tpsheetList ;
    private static string pathRoot = "";
    [MenuItem("工具/修改图集的tpsheep文件")]
    public static void AlterTpsheet() {
#if UNITY_EDITOR
        pathRoot = Application.dataPath;
        pathRoot = pathRoot.Substring(0, pathRoot.LastIndexOf("/"));
        tpsheetList = new List<string>();
        JugeIsTpsheet();
        if (tpsheetList == null) return;
        if (tpsheetList.Count < 1) {
            Debug.LogError("没有选中tpsheet文件");
        }
        else
        {
            foreach (string assetsPath in tpsheetList) {
                string path = pathRoot + "/" + assetsPath;

                if (!File.Exists(path))
                {

                    Debug.LogError("路径不存在");
                    return;

                }

                var data = File.ReadAllLines(path);
                StreamWriter s = new StreamWriter(path);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.Length; i++)
                {

                    if (i >= 15)
                    {
                        if (data[i].Contains(".png"))
                        {

                            var strData = data[i].Replace(".png", "");
                            //Debug.LogError(strData);
                            sb.Append(strData + "\r\n");
                        }
                    }
                    else
                    {
                        sb.Append(data[i] + "\r\n");
                    }

                }
                s.Write(sb.ToString());
                s.Flush();
                s.Close();
                AssetDatabase.Refresh();
            }

        }     
#endif
    }

    private static void JugeIsTpsheet()
    {
        if (tpsheetList == null) tpsheetList = new List<string>();
        tpsheetList.Clear();
        //获取选中的对象
        object[] selection = (object[])Selection.objects;
        //判断是否有对象被选中
        if (selection.Length == 0)
            return;
 
        foreach (Object obj in selection)
        {
            string objPath = AssetDatabase.GetAssetPath(obj);
            if (objPath.EndsWith(".tpsheet"))
            {
                tpsheetList.Add(objPath);
            }
        }
    }
}
