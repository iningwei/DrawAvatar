using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HolderNodeObjPoolStats : MonoBehaviour
{
    [MenuItem("GameObject/统计HolderNodeObjPool", false, 13)]
    public static void Do()
    {
        GameObject obj = Selection.activeObject as GameObject;
        Dictionary<string, int> resultDic = new Dictionary<string, int>();
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            string name = obj.transform.GetChild(i).name;
            if (resultDic.ContainsKey(name))
            {
                resultDic[name] += 1;
            }
            else
            {
                resultDic.Add(name, 1);
            }
        }

        Debug.LogError("统计结果：");
        foreach (var item in resultDic)
        {
            Debug.LogError($"{item.Key}--->{item.Value}");
        }
    }
}
