using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniJSON;

namespace ZGame
{

    public static class MiniJsonTool
    {
        public static List<string> ToListString(this List<object> old)
        {
            List<string> target = new List<string>();
            if (old.Count > 0)
            {
                for (int i = 0; i < old.Count; i++)
                {
                    target.Add((string)old[i]);
                }
            }
            return target;
        }

        //public static float ToFloat(this string str)
        //{

        //    float f = 0f;
        //    if (str.IndexOf('.') == -1 && str.IndexOf('E') == -1 && str.IndexOf('e') == -1)
        //    {
        //        f = float.Parse(str);
        //    }
        //    else
        //    {
        //      f= (float)(double)str;
        //    }

        //}

        public static string KVsToJsonStr(List<KeyValuePair<string, string>> kvs)
        {
            if (kvs == null || kvs.Count == 0)
            {
                Debug.LogError("error,kvs is not valid");
                return "{}";
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < kvs.Count; i++)
            {
                var key = kvs[i].Key;
                var value = kvs[i].Value;
                dic[key] = value;
            }
            return Json.Serialize(dic);
        }

        public static string KVsToJsonStrBase64(List<KeyValuePair<string, string>> kvs)
        {
            string jsonStr = KVsToJsonStr(kvs);

            return jsonStr.ToBase64();
        }
    }
}