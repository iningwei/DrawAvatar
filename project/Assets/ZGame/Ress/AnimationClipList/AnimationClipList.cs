using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame.Res
{
    public class AnimationClipList : MonoBehaviour
    {
        public AnimationClip[] clips;
#if UNITY_EDITOR
        void OnValidate()
        {
            //过滤掉重复项
            if (clips != null && clips.Length > 0)
            {
                Dictionary<string, AnimationClip> dic = new Dictionary<string, AnimationClip>();
                for (int i = 0; i < clips.Length; i++)
                {
                    dic[clips[i].name] = clips[i];
                }

                List<AnimationClip> st = new List<AnimationClip>();

                foreach (var v in dic)
                {
                    st.Add(v.Value);
                }

                clips = st.ToArray();
            }
        }
#endif
    }
}