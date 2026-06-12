using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class OpenByNotepadPP
{
    //http://docs.unity3d.com/ScriptReference/MenuItem.html
    [MenuItem("Assets/open with Notepad++(win) or 文本编辑(mac)", false, 1)]
    static void openByNotepadPP()
    {
        if (Selection.objects.Length != 1)
        {
            UnityEngine.Debug.LogError("you can only open one file per time");
            return;
        }
        UnityEngine.Object obj = Selection.objects[0];
        string path = Application.dataPath + AssetDatabase.GetAssetPath(obj).Substring(6);
        if (!(path.EndsWith(".cs") || path.EndsWith(".txt")
            || path.EndsWith(".bin") || path.EndsWith(".xml")
            || path.EndsWith(".shader")
            || path.EndsWith(".jslib")
            || path.EndsWith(".bin")
            || path.EndsWith(".bytes")
             || path.EndsWith(".json")
             || path.EndsWith(".m")
             || path.EndsWith(".mm")
             || path.EndsWith(".h")
             || path.EndsWith(".html")
             || path.EndsWith(".atlas")
             || path.EndsWith(".md")))
        {
            UnityEngine.Debug.LogError("not support current file:" + path);
            return;
        }
        UnityEngine.Debug.Log(path + ",opened");
#if UNITY_EDITOR_WIN
        //Process.Start(path);//使用默认打开方式打开文本文件
        Process.Start("notepad++.exe", path);//使用notepad++打开，需要机器安装了该软件
#elif UNITY_EDITOR_OSX
//使用mac上的文本编辑打开文件
        Application.OpenURL("file://" + path);
#endif 
    }
}
