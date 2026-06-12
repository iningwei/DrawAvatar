using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZGame.Ress.AB
{
    /// <summary>
    /// Lua scripts builded to ab with no prefix. 
    /// </summary>
    public class ABLogic
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">no suffix, born no prefix</param>
        /// <returns></returns>
        public static void LoadAll(string name, Action<TextAsset[]> callback)
        {
            AB.Load(name, ABType.Null, (objs) =>
          {
              int count = objs.Length;
              TextAsset[] texts = new TextAsset[count];
              for (int i = 0; i < count; i++)
              {
                  texts[i] = objs[i] as TextAsset;
              }
              callback?.Invoke(texts);
          }, true);

        }
    }
}