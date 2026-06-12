using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace ZGame.Res
{
    public class MatList : MonoBehaviour
    {

        public Material[] Mats;
#if UNITY_EDITOR
        void OnValidate()
        {
            //过滤掉重复项
            if (Mats != null && Mats.Length > 0)
            {
                Dictionary<string, Material> dic = new Dictionary<string, Material>();
                for (int i = 0; i < Mats.Length; i++)
                {
                    dic[Mats[i].name] = Mats[i];
                }

                List<Material> matL = new List<Material>();

                foreach (var v in dic)
                {
                    matL.Add(v.Value);
                }

                Mats = matL.ToArray();
            }
        }
#endif
    }
}