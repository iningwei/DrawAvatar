using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public static class Vector3Ext
{
    public static Vector3 ZeroZAxis(this Vector3 inputVec)
    {
        return new Vector3(inputVec.x, inputVec.y, 0);
    }

    public static Vector3 MultiplyVector3(this Vector3 inputVec, Vector3 targetVec)
    {
        return new Vector3(inputVec.x * targetVec.x, inputVec.y * targetVec.y, inputVec.z * targetVec.z);
    }

    public static Vector3 SetNewValue(this Vector3 inputVec, Axis axis, float value)
    {
        if (axis == Axis.X)
        {
            return new Vector3(value, inputVec.y, inputVec.z);
        }
        else if (axis == Axis.Y)
        {
            return new Vector3(inputVec.x, value, inputVec.z);
        }
        else if (axis == Axis.Z)
        {
            return new Vector3(inputVec.x, inputVec.y, value);
        }
        else
        {
            Debug.LogError("error ,not support axis:" + axis);
            return Vector3.zero;
        }
    }
    public static Vector3 Divide(this Vector3 inputVec, float f)
    {
        float x = inputVec.x / f;
        float y = inputVec.y / f;
        float z = inputVec.z / f;
        Vector3 v = new Vector3(x, y, z);
        return v;
    }


    public static Vector3 ToUGUIPos(this Vector3 worldPos, Camera camera, Canvas canvas)
    {
        Vector3 viewPos = camera.WorldToViewportPoint(worldPos);
        RectTransform canvasRectTrans = canvas.GetComponent<RectTransform>();
        float x = canvasRectTrans.rect.width * viewPos.x - canvasRectTrans.rect.width * 0.5f;
        float y = canvasRectTrans.rect.height * viewPos.y - canvasRectTrans.rect.height * 0.5f;

        return new Vector3(x, y, 0f);
    }
}
