using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZGame
{
    public class Coordinate
    {
        /// <summary>
        /// 获得相对坐标
        /// 获得srcObj相对targetObj本地坐标系下的坐标
        /// </summary>
        /// <param name="srcObj"></param>
        /// <param name="targetObj"></param>
        /// <returns></returns>
        public static Vector3 GetRelativeCoord(GameObject srcObj, GameObject targetObj)
        {
            Vector3 srcPos = srcObj.transform.position;
            return targetObj.transform.InverseTransformPoint(srcPos);
        }
    }
}