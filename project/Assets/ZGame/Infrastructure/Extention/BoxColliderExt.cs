using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoxColliderExt
{
    //获得boxcollider的八个顶点个世界坐标
    public static Vector3[] GetBoxColliderVertexPositions(this BoxCollider boxcollider)
    {
        var vertices = new Vector3[8];
        //下面4个点
        vertices[0] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[1] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[2] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        vertices[3] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, -boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        //上面4个点
        vertices[4] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[5] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, boxcollider.size.z) * 0.5f);
        vertices[6] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(-boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        vertices[7] = boxcollider.transform.TransformPoint(boxcollider.center + new Vector3(boxcollider.size.x, boxcollider.size.y, -boxcollider.size.z) * 0.5f);
        return vertices;
    }


    public static bool IsPointInside(this BoxCollider boxCollider, Vector3 worldPoint)
    {
        var flag = boxCollider.bounds.Contains(worldPoint);
        if (flag == true)
        {
            Debug.LogError("in collider:" + boxCollider.transform.name);
        }
        return flag;


        //以下代码有bug

        // 将世界坐标点转换为BoxCollider的本地坐标
        Vector3 localPoint = boxCollider.transform.InverseTransformPoint(worldPoint);

        // 获取BoxCollider的中心和尺寸
        Vector3 center = boxCollider.center;
        Vector3 size = boxCollider.size * 0.5f; // 半尺寸

        // 获取GameObject的缩放
        Vector3 scale = boxCollider.transform.lossyScale;

        // 检查零缩放
        if (Mathf.Abs(scale.x) < Mathf.Epsilon ||
            Mathf.Abs(scale.y) < Mathf.Epsilon ||
            Mathf.Abs(scale.z) < Mathf.Epsilon)
        {
            Debug.LogWarning("BoxCollider has zero or near-zero scale, cannot determine bounds.");
            return false;
        }

        // 调整尺寸以考虑缩放
        Vector3 scaledSize = new Vector3(
            size.x / Mathf.Abs(scale.x),
            size.y / Mathf.Abs(scale.y),
            size.z / Mathf.Abs(scale.z)
        );


        // 计算边界框的min和max
        Vector3 min = center - scaledSize;
        Vector3 max = center + scaledSize;

        // 调试信息
        Debug.LogError($"{boxCollider.transform.name}, Checking point: world={worldPoint}, local={localPoint}\n" +
                  $"BoxCollider: center={center}, size={boxCollider.size}, scale={scale}\n" +
                  $"Bounds: min={min}, max={max}");

        // 判断点是否在边界框内
        bool isInside = localPoint.x >= min.x && localPoint.x <= max.x &&
                        localPoint.y >= min.y && localPoint.y <= max.y &&
                        localPoint.z >= min.z && localPoint.z <= max.z;

        Debug.LogError($"Point is {(isInside ? "inside" : "outside")} the BoxCollider:{boxCollider.transform.name}");
        return isInside;
    }
}
