using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    public class BuildSprite : BuildImg
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Sprite);

            
            return base.Build(obj);
        }
    }
}