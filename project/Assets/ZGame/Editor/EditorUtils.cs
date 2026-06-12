using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class EditorUtils
{
    //work while right click folder.
    //Selection.activeObject;  
    public static bool GetCurSelectedFolderPath(out List<string> paths)
    {
        List<string> pathList = new List<string>();
        foreach (var obj in Selection.objects)
        {
            string tmpPath = AssetDatabase.GetAssetPath(obj);
            Debug.Log(tmpPath);
            if (AssetDatabase.IsValidFolder(tmpPath))
            {
                pathList.Add(tmpPath);
            }
        }
        paths = pathList;
        return pathList.Count > 0;
    }
}
