using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{
    public static class RectTransformExt
    {
        public static void RebuildLayout(this RectTransform trans)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
        }
    }
}