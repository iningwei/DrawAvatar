using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
 
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    //ugui窗体预制件
    public class BuildWindow : BuildPrefab
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Window);
            return base.Build(obj);
        }
    }
}