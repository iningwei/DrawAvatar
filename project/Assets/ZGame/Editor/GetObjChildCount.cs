using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GetObjChildCount
{
    [MenuItem("GameObject/GetChildCount", false, 13)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        int activeCount = 0;
        for (int i = 0; i < obj.transform.childCount; i++)
        {
            var child = obj.transform.GetChild(i);
            if (child.gameObject.activeSelf)
            {
                activeCount++;
            }
        }

        Debug.LogError(obj.name + ", child count:" + obj.transform.childCount + ", activeCount:" + activeCount);
    }
}
