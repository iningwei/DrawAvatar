using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using ZGame;

public static class StringExt
{

    /// <summary>
    /// 扩展 获取变量名称(字符串)
    /// //PS:C# 6.0可以使用 nameof(变量) 来获得变量的名字
    /// </summary>
    /// <param name="var_name"></param>
    /// <param name="exp"></param>
    /// <returns>return string</returns>
    public static string GetVarName<T>(this T var_name, System.Linq.Expressions.Expression<Func<T>> exp)
    {
        return ((System.Linq.Expressions.MemberExpression)exp.Body).Member.Name;
    }

    /// <summary>
    /// 判断字符串是否包含汉字
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool ContainChinese(this string text)
    {
        bool res = false;
        foreach (char t in text)
        {
            if ((int)t > 127)
                res = true;
        }
        return res;
    }

    /// <summary>
    /// 判断资源名是否合法
    /// 规则：“字母、数字、空格符 、下划线_、中划线-”组成，且开头和结尾只能是字母或者数字
    /// </summary>
    /// <param name="resNameWithoutExt">不带后缀的文件名</param>
    /// <returns></returns>
    public static bool IsResNameValid(this string resNameWithoutExt)
    {

        if (Regex.Match(resNameWithoutExt, @"^[0-9a-zA-Z][a-zA-Z0-9 _\-]+[0-9a-zA-Z]$").Success)
        {
            return true;
        }

        return false;
    }


    //反斜杠\ backslash
    //斜杠/  slash
    /// <summary>
    /// 归一化路径中的斜杠或反斜杠为斜杠
    /// </summary>
    /// <param name="inputStr"></param>
    /// <returns></returns>
    public static string UniformSlash(this string inputStr)
    {
        return inputStr.Replace('\\', '/');
    }


    public static int[] ToIntArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            return new int[] { };
        }
        int[] result = new int[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = int.Parse(strArray[i]);
            }
            catch (Exception ex)
            {
                Debug.LogError("StringArrayToIntArray exception:" + ex.Message);
            }

        }
        return result;
    }

    public static long[] ToLongArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            return new long[] { };
        }
        long[] result = new long[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = long.Parse(strArray[i]);
            }
            catch (Exception ex)
            {

                Debug.LogError("StringArrayToLongArray exception:" + ex.Message);
            }

        }
        return result;
    }

    public static float[] ToFloatArray(this string[] strArray)
    {
        if (strArray == null || strArray.Length == 0)
        {
            return new float[] { };
        }
        float[] result = new float[strArray.Length];
        for (int i = 0; i < strArray.Length; i++)
        {
            try
            {
                result[i] = float.Parse(strArray[i]);
            }
            catch (Exception ex)
            {
                Debug.LogError($"StringArrayToFloatArray exception:{strArray[i]}   , " + ex.Message);
            }

        }
        return result;
    }


    public static Vector2 ToVector2(this string str, char splitChar)
    {
        try
        {
            var datas = str.Split(splitChar);
            return new Vector2(float.Parse(datas[0]), float.Parse(datas[1]));
        }
        catch (Exception ex)
        {
            Debug.LogError("ToVector2 Error, input str:" + str + ", splitChar:" + splitChar + ", ex:" + ex.ToString());

            return Vector2.zero;
        }

    }


    public static string ToBase64(this string str)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
    }

    public static string Base64ToUTF8(this string base64Str)
    {
        var bytes = Convert.FromBase64String(base64Str);
        var utf8Str = Encoding.UTF8.GetString(bytes);
        return utf8Str;
    }

    public static string SubWithPoints(this string str, int len = 5, bool onlySub = false)
    {
        if (str.Length <= len)
        {
            return str;
        }
        else
        {
            if (onlySub)
            {
                return str.Substring(0, len);
            }
            else
            {
                return str.Substring(0, len) + "...";
            }
        }
    }



    /// <summary>
    /// 首字母小写写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToLower(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToLower() + input.Substring(1);
        return str;
    }

    /// <summary>
    /// 首字母大写
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string FirstCharToUpper(this string input)
    {
        if (String.IsNullOrEmpty(input))
            return input;
        string str = input.First().ToString().ToUpper() + input.Substring(1);
        return str;
    }

    /// <summary>
    /// 向下取整指定位数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalLength"></param>
    /// <returns></returns>
    public static string FormatNumberToString(this double value, int decimalLength, bool removeZero)
    {
        double d = Math.Pow(10, decimalLength);
        double v = ((int)Math.Floor(value * d)) / d;
        if (removeZero)
        {
            return v.ToString();
        }
        else
        {
            return v.ToString($"F{decimalLength}");
        }
    }
    /// <summary>
    /// 向下取整指定位数
    /// </summary>
    /// <param name="value"></param>
    /// <param name="decimalLength"></param>
    /// <returns></returns>
    public static string FormatNumberToString(this float value, int decimalLength, bool removeZero)
    {
        var d = (double)value;
        return d.FormatNumberToString(decimalLength, removeZero);
    }


    public static bool VerifyIsValidPhoneNum(this string phoneNumStr)
    {
        string pattern = @"^1[3456789]\d{9}$";
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNumStr, pattern);
    }

    public static bool VerifyIsValidIdentityCardNum(this string idCardStr)
    {
        string pattern = @"^[1-9]\d{5}(18|19|20|21|22)?\d{2}(0[1-9]|1[0-2])(0[1-9]|[12]\d|3[01])\d{3}(\d|[Xx])$";
        return System.Text.RegularExpressions.Regex.IsMatch(idCardStr, pattern);
    }

    public static string RemoveDuplicateCharacters(this string originStr)
    {
        HashSet<char> seen = new HashSet<char>(); // 用于存储已出现的字符
        List<char> resultList = new List<char>();

        foreach (char c in originStr)
        {
            if (seen.Add(c)) // HashSet.Add() 会返回true，如果字符第一次添加
            {
                resultList.Add(c);
            }
        }

        return new string(resultList.ToArray());

        //or use linq, but if originStr is huggge, we suggest use upper!
        // string result = new string(originStr.Distinct().ToArray());
    }

    static string[] units = { "", "万", "亿", "豪" };
    static double[] divisors = { 1, 1e4, 1e8, 1e12 };
    /// <summary>
    /// 
    /// </summary>
    /// <param name="number"></param>
    /// <param name="minNumber">参与大数字处理的最小值，低于此值不做处理直接返回对应字符串</param>
    /// <returns></returns>
    public static string FormatBigNumber(this long number, long minNumber = 10000)
    {
        if (number < minNumber)
        {
            return number.ToString();
        }
        // 确定合适的单位和除数 
        int unitIndex = 0;
        double divisor = 1;
        for (int i = divisors.Length - 1; i >= 0; i--)
        {
            if (Math.Abs(number) >= divisors[i])
            {
                divisor = divisors[i];
                unitIndex = i;
                break;
            }
        }

        double formattedValue = number / divisor;

        // 根据数值大小动态调整小数位 
        if (divisor == 1) // 无单位时保留最多三位有效数字 
        {
            return formattedValue.ToString("0.###");
        }
        else
        {
            if (formattedValue >= 100)
            {
                return $"{formattedValue:F0}{units[unitIndex]}";
            }
            else if (formattedValue >= 10)
            {
                return $"{formattedValue.ToString("0.#")}{units[unitIndex]}";
            }
            else
            {
                return $"{formattedValue.ToString("0.##")}{units[unitIndex]}";
            }
        }
    }
    static string[] units2 = { "", "K", "M", "B", "T" };
    static double[] divisors2 = { 1, 1e3, 1e6, 1e9, 1e12 };
    public static string FormatBigNumber2(this long number, long minNumber = 10000)
    {
        if (number < minNumber)
        {
            return number.ToString();
        }
        // 确定合适的单位和除数 
        int unitIndex = 0;
        double divisor = 1;
        for (int i = divisors2.Length - 1; i >= 0; i--)
        {
            if (Math.Abs(number) >= divisors2[i])
            {
                divisor = divisors2[i];
                unitIndex = i;
                break;
            }
        }

        double formattedValue = number / divisor;

        // 根据数值大小动态调整小数位 
        if (divisor == 1) // 无单位时保留最多三位有效数字 
        {
            return formattedValue.ToString("0.###");
        }
        else
        {
            if (formattedValue >= 100)
            {
                return $"{formattedValue:F0}{units2[unitIndex]}";
            }
            else if (formattedValue >= 10)
            {
                return $"{formattedValue:F1}{units2[unitIndex]}";
            }
            else
            {
                return $"{formattedValue:F2}{units2[unitIndex]}";
            }
        }
    }



    /// <summary>
    /// 将数字格式化为带逗号的千分位格式，例如：39072001 → 39,072,001
    /// </summary>
    public static string FormatWithComma(this long number)
    {
        return number.ToString("N0");
    }

    // 重载：支持 int
    public static string FormatWithComma(this int number)
    {
        return number.ToString("N0");
    }

    // 重载：支持 ulong 
    public static string FormatWithComma(this ulong number)
    {
        return number.ToString("N0");
    }
}
