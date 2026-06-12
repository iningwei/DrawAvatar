using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class BezierTool
    {
        public static Vector3 InterpVector3(Vector3 startPos, Vector3 endPos, Vector3 controlPoint, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            Vector3 result = (1 - ratio) * (1 - ratio) * startPos + 2 * ratio * (1 - ratio) * controlPoint + ratio * ratio * endPos;
            return result;
        }
    }
}