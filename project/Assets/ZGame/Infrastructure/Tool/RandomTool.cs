using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class RandomTool
    {


        /// <summary>
        /// [v1,v2)
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static int NextInt(int v1, int v2)
        {
            return UnityEngine.Random.Range(v1, v2);
        }

        public static int NextExceptionInt(int v1, int v2, int exceptionValue)
        {
            while (true)
            {
                int v = NextInt(v1, v2);
                if (v != exceptionValue)
                {
                    return v;
                }
            }
        }



        public static float NextFloat(float v1, float v2, uint precision = 0)
        {
            if (v2 <= v1)
            {
                return v1;
            }
            if (precision > 10)
            {
                precision = 10;
                Debug.LogWarning("precision too large, Unity float precision is about 7-10 digits");
            }
            if (precision < 0)
            {
                precision = 0;
            }

            // 1. Unity 官方方法：Range(0f, 1f) 返回 [0, 1) 严格半开区间
            float random01 = UnityEngine.Random.Range(0f, 1f);

            // 2. 映射到目标区间 [v1, v2)
            float result = v1 + random01 * (v2 - v1);

            // 3. 处理精度（四舍五入到指定小数位）
            if (precision == 0)
            {
                return Mathf.Round(result);  // Unity 官方取整函数
            }
            else
            {
                float scale = Mathf.Pow(10f, precision);  // Unity 官方 Pow
                return Mathf.Round(result * scale) / scale;
            }
        }

        //[0, 1) 随机浮点数，保留指定小数位
        public static float NextFloat(uint precision) => NextFloat(0, 1, precision);

        //[v1, v2) 不保留小数（等同于 Mathf.Round后的随机整数）
        public static float NextFloat(float v1, float v2)
       => NextFloat(v1, v2, 0);

        //[v1, v2] 闭区间（包含 v2，常用于 UI 数值显示）
        public static float NextFloatInclusive(float v1, float v2, uint precision = 0)
        => NextFloat(v1, v2 + 0.000001f, precision);  // 微调确保包含 v2


        public static bool Ratio(float r)
        {
            if (r < 0 || r > 1)
            {
                Debug.LogError("error, r should [0,1]");
                return false;
            }
            int v = 0;
            int b = 1;
            if (r >= 0.01 && r <= 1)
            {
                b = 100;
                v = (int)(r * b);
            }
            else if (r >= 0.001 && r < 0.01)
            {
                b = 1000;
                v = (int)(r * b);
            }
            else if (r >= 0.0001 && r < 0.001)
            {
                b = 10000;
                v = (int)(r * b);
            }
            else
            {
                Debug.LogError("r should at least>=0.0001");
            }

            int randomV = NextInt(0, b);
            //DebugExt.LogE("randomV:" + randomV + ", v:" + v);
            if (randomV < v)
            {
                return true;
            }
            return false;
        }


        public static string RandomStr(int len)
        {
            string all = "0,1,2,3,4,5,6,7,8,9,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z";
            string[] allChar = all.Split(',');
            string result = "";
            System.Random rand = new System.Random();
            for (int i = 0; i < len; i++)
            {
                result += allChar[RandomTool.NextInt(0, allChar.Length)];
            }
            return result;
        }
    }
}