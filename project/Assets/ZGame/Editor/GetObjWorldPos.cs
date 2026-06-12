using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GetObjWorldPos
{
    [MenuItem("GameObject/GetObjWorldPos", false, 14)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        Debug.LogError(obj.name + ", world pos:" + obj.transform.position);
    }
}
