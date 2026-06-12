using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Window;

namespace ZGame.Window
{
    public class Member : UIRoot
    {
        public Member(GameObject obj, UIRoot parentUIRoot, params object[] paras) : base(obj, parentUIRoot, paras)
        {
        }
    }
}