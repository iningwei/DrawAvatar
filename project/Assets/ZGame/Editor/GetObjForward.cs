using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class GetObjForward
{
    [MenuItem("GameObject/GetObjForward", false, 13)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        Debug.LogError(obj.name + ",   forward:" + obj.transform.forward.normalized);
    }




    [MenuItem("GameObject/GetObjEulerAngles", false, 14)]
    public static void GetEuler()
    {
        GameObject obj = Selection.activeObject as GameObject;
        Vector3 euler = obj.transform.eulerAngles;
        Debug.LogError(obj.name + ",   euler x:" + euler.x + ", y:" + euler.y + ", z:" + euler.z);
    }
}
