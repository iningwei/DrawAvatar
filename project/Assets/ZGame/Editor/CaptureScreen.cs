using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CaptureScreen : MonoBehaviour
{
    [MenuItem("工具/截屏")]
    public static void Capture()
    {
        var now = System.DateTime.Now;
        var year = now.Year;
        var month = now.Month;
        var day = now.Day;
        var hour = now.Hour;
        var minute = now.Minute;
        var second = now.Second;
        var name = year.ToString() + month.ToString() + day.ToString() + hour.ToString() + minute.ToString() + second.ToString() + ".png";
        var path = Application.dataPath + "/../" + name;
        ScreenCapture.CaptureScreenshot(path);
        Debug.LogError("capture finished:" + path);
        AssetDatabase.Refresh();
    }
}
