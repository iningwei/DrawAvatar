using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.DanMu
{
    public class DanMuInput
    {
        public static List<DanMuBaseData> inputList = new List<DanMuBaseData>();

        public static void AddDanMuData(DanMuBaseData data)
        {
            inputList.Add(data);
        }
        public static List<DanMuBaseData> GetDanMuList()
        {
            List<DanMuBaseData> result = new List<DanMuBaseData>();
            for (int i = 0; i < inputList.Count; i++)
            {
                result.Add(inputList[i]);
            }
            inputList.Clear();
            return result;
        }
    }
}
