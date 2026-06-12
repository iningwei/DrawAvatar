using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Reflection;
using System;
using ZGame;

public class GetObjHierarchy
{
    [MenuItem("GameObject/GetObjHierarchy", false, 15)]
    public static void Get()
    {
        GameObject obj = Selection.activeObject as GameObject;
        string path = obj.GetHierarchy();
        ClipboardHelper.clipBoard = path;
        Debug.Log("path:" + path + ", has copied to you clipboard!");
    }
}