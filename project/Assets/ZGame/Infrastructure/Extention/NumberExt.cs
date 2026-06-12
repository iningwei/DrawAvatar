using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class NumberExt
{
    public static Vector2 ToVector2(this float[] floatArray)
    {
        if (floatArray == null || floatArray.Length < 2)
        {
            Debug.LogError("floatArray can not change to Vector2" + floatArray.ToString());
            return Vector2.zero;
        }
        return new Vector2(floatArray[0], floatArray[1]);
    }
    public static Vector3 ToVector3(this float[] floatArray)
    {
        if (floatArray == null || floatArray.Length < 3)
        {
            Debug.LogError("floatArray can not change to Vector3" + floatArray.ToString());
            return Vector3.zero;
        }
        return new Vector3(floatArray[0], floatArray[1], floatArray[2]);
    }


    public static string FormatKM(this int value, int precise = 1)
    {
        string preciseStr = "f" + precise;
        string formatStr;
        if (value < 1000)
        {
            formatStr = value.ToString();
        }
        else if (value >= 1000 && value < 1000000)
        {
            formatStr = (value / 1000f).ToString(preciseStr) + "K";
        }
        else
        {
            formatStr = (value / 1000000f).ToString(preciseStr) + "M";
        }

        return formatStr;
    }


    public static string FormatQianWan(this int value, int precise = 2)
    {
        string preciseStr = "f" + precise;
        string formatStr;
        if (value < 1000)
        {
            formatStr = value.ToString();
        }
        else if (value >= 1000 && value < 10000)
        {
            formatStr = (value / 1000f).ToString(preciseStr) + "千";
        }
        else
        {
            formatStr = (value / 10000f).ToString(preciseStr) + "万";
        }

        return formatStr;
    }

    public static void ExtractDHMS(this long seconds, out long d, out long h, out long m, out long s)
    {
        d = seconds / (3600 * 24);
        h = seconds % (3600 * 24) / 3600;
        m = seconds % 3600 / 60;
        s = seconds % 60;
    }

    public static void ExtractDHMS(this int seconds, out int d, out int h, out int m, out int s)
    {
        d = seconds / (3600 * 24);
        h = seconds % (3600 * 24) / 3600;
        m = seconds % 3600 / 60;
        s = seconds % 60;
    }

    public static string FormatMMSS(this long seconds)
    {
        string ms = "00:00";
        ms = $"{seconds % 3600 / 60:00}:{seconds % 60:00}";
        return ms;
    }

    public static string FormatHHMMSS(this long seconds)
    {
        string hms = "00:00:00";
        hms = $"{seconds / 3600:00}:{seconds % 3600 / 60:00}:{seconds % 60:00}";
        return hms;
    }
    public static string FormatHHMMSS(this int seconds)
    {
        string hms = "00:00:00";
        hms = $"{seconds / 3600:00}:{seconds % 3600 / 60:00}:{seconds % 60:00}";
        return hms;
    }

    public static string FormatMMSS(this int seconds)
    {
        string hms = "00:00";
        hms = $"{seconds / 60:00}:{seconds % 60:00}";
        return hms;
    }

    public static string FormatSimple(this long seconds)
    {
        if (seconds >= 86400) // ≥ 1天
        {
            long days = seconds / 86400;
            return $"{days}天";
        }
        else if (seconds >= 3600) // ≥ 1小时
        {
            long hours = seconds / 3600;
            return $"{hours}时";
        }
        else if (seconds >= 60) // ≥ 1分钟
        {
            long minutes = seconds / 60;
            return $"{minutes}分";
        }
        else
        {
            return $"{seconds}秒";
        }
    }

}
