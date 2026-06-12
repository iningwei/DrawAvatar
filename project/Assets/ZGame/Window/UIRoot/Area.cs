using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;

namespace ZGame.Window
{
    /// <summary>
    /// Area is the inner partial area of a window
    /// Used to help window to handle with complex logic
    /// </summary>
    public class Area : UIRoot
    {
        public Area(GameObject obj, UIRoot parentUIRoot,  params object[] paras) : base(obj, parentUIRoot,  paras)
        {
           
        }
    }
}