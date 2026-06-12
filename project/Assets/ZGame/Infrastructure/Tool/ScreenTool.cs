using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenTool
{
    /// <summary>
    /// 坐标是否在当前屏幕内
    /// </summary>
    /// <param name="worldPos"></param>
    /// <returns></returns>
    public static bool IsOnScreen(Camera mainCam, Vector3 worldPos)
    {
        // 将对象位置转换为视口坐标（0-1 范围）
        Vector3 screenPoint = mainCam.WorldToViewportPoint(worldPos);
        // 检查是否在屏幕范围内
        bool onScreen = screenPoint.z > 0 && // 在摄像机前方
                        screenPoint.x > 0 && screenPoint.x < 1 &&
                        screenPoint.y > 0 && screenPoint.y < 1;

        return onScreen;
    }
}
