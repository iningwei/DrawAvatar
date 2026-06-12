using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    /// <summary>
    /// 多语言
    /// </summary>
    public class Multilingual
    {
        public static string FormatStr(long refId, string targetStr, params object[] paras)
        {
            if (string.IsNullOrEmpty(targetStr))
            {
                return "";
            }

            string r = targetStr;
            if (paras != null)
            {
                try
                {
                    r = string.Format(targetStr, paras);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError(targetStr + " format failed,  refId:" + refId);
                }
            }


            return r;
        }
    }
}