using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Res
{
    public class ShaderList : MonoBehaviour
    {
        public Shader[] Shaders;
#if UNITY_EDITOR
        void OnValidate()
        {
            //过滤掉重复项
            if (Shaders != null && Shaders.Length > 0)
            {
                Dictionary<string, Shader> dic = new Dictionary<string, Shader>();
                for (int i = 0; i < Shaders.Length; i++)
                {
                    dic[Shaders[i].name] = Shaders[i];
                }

                List<Shader> st = new List<Shader>();

                foreach (var v in dic)
                {
                    st.Add(v.Value);
                }

                Shaders = st.ToArray();
            }
        }
#endif
    }
}