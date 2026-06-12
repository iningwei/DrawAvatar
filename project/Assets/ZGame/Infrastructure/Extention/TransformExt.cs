using System;
using System.Collections.Generic;
using UnityEngine;
using ZGame;

/// <summary>
/// Transform扩展方法
/// </summary>
public static class TransformExt
{
    public static void HideAllChilds(this Transform transform)
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    public static void DestroyAllChilds(this Transform transform, bool destroyImmediate, Action beforeDestroyFunc = null)
    {
        int count = transform.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            if (beforeDestroyFunc != null)
            {
                beforeDestroyFunc();
            }

            if (destroyImmediate)
            {
                GameObject.DestroyImmediate(transform.GetChild(i).gameObject);
            }
            else
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }
    }
    public static string GetHierarchy(this Transform transform)
    {
        Transform parent = transform.parent;
        string hierarchyStr = transform.name;
        while (parent != null)
        {
            hierarchyStr = parent.name + "/" + hierarchyStr;
            parent = parent.parent;
        }
        return hierarchyStr;
    }

    // Breadth First Search
    public static T BFS<T>(this Transform root, string targetName) where T : Component
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            Transform current = queue.Dequeue();
            T t = current.GetComponent<T>();
            if (t && current.name == targetName)
            {
                return t;
            }
            for (int i = 0; i < current.childCount; i++)
            {
                Transform child = current.GetChild(i);
                queue.Enqueue(child);
            }
        }
        return null;
    }

    // Depth First Search
    // unity节点搜索一般建议使用DFS
    // 也可以使用递归的方式实现DFS
    public static T DFS<T>(this Transform root, string targetName) where T : Component
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Transform current = stack.Pop();
            T t = current.GetComponent<T>();
            if (t && current.name == targetName)
            {
                return t;
            }

            for (int i = current.childCount - 1; i >= 0; i--)
            {
                Transform child = current.GetChild(i);
                stack.Push(child);
            }
        }

        return null;
    }

    public static T DFS<T>(this Transform root, string targetName, Func<Transform, bool> condition) where T : Component
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Transform current = stack.Pop();
            T t = current.GetComponent<T>();
            if (t && current.name == targetName)
            {
                return t;
            }

            if (condition(current) == false)
                continue;
            for (int i = current.childCount - 1; i >= 0; i--)
            {
                Transform child = current.GetChild(i);
                stack.Push(child);
            }
        }

        return null;
    }
    public static Component DFS(this Transform root, Type type, string targetName, Func<Transform, bool> condition)
    {
        Stack<Transform> stack = new Stack<Transform>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            Transform current = stack.Pop();
            Component c = current.GetComponent(type);
            if (c && current.name == targetName)
            {
                return c;
            }

            if (condition(current) == false)
                continue;
            for (int i = current.childCount - 1; i >= 0; i--)
            {
                Transform child = current.GetChild(i);
                stack.Push(child);
            }
        }

        return null;
    }

    public static Component FindComponent(this Transform transform, Type type, string name, bool includeInactive)
    {
        Component[] targets = transform.GetComponentsInChildren(type, includeInactive);
        if (targets.Length > 0)
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i].name == name)
                {
                    return targets[i];
                }
            }
        }
        return null;
    }


    public static T FindChildComponent<T>(this Transform transform, string path) where T : Component
    {
        T t = null;
        var child = transform.Find(path);
        if (child != null)
        {
            t = child.GetComponent<T>();
        }
        if (t == null)
        {
            Debug.LogError("error ,FindChild of type:" + t.GetType().Name + " failed, path:" + path);
        }
        return t;
    }
    //public static T GetComponentInParent<T>(this Transform transform) where T : Component
    //{
    //    Transform parent = transform.parent;
    //    T t = null;
    //    while (parent != null)
    //    {
    //        t = parent.GetComponent<T>();
    //        if (t != null)
    //        {
    //            return t;
    //        }
    //        else
    //        {
    //            parent = parent.parent;
    //        }
    //    }
    //    return t;
    //}

    public static void GetCompnentsInChildrenExceptSelf<T>(this Transform transform, bool includeInactive, List<T> results) where T : Component
    {

        transform.GetComponentsInChildren(includeInactive, results);

        foreach (var item in results)
        {
            if (item.transform.Equals(transform))
            {
                results.Remove(item);
                break;
            }
        }
    }


    /// <summary>
    /// 获得自身坐标系下某点相对于目标物体坐标系下的坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="localPos"></param>
    /// <param name="correspondTran"></param>
    /// <returns></returns>
    public static Vector3 GetCorrespondPos(this Transform transform, Vector3 localPos, Transform correspondTran)
    {
        Vector3 correspondPos = Vector3.zero;
        var worldPos = transform.TransformPoint(localPos);
        correspondPos = correspondTran.InverseTransformPoint(worldPos);
        return correspondPos;
    }





    #region position
    //lua侧若调用obj.transform.position=CS.UnityEngine.Vector3(x,y,z);进行赋值的话，是一个很耗的操作
    //通过这种包装，可以大幅度降低消耗

    /// <summary>
    /// 设置X坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    public static void SetPosX(this Transform transform, float x, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
                break;

        }

    }

    /// <summary>
    /// 设置Y坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    public static void SetPosY(this Transform transform, float y, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
                break;
        }

    }

    /// <summary>
    /// 设置Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="z"></param>
    public static void SetPosZ(this Transform transform, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, transform.position.y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
                break;
        }
    }

    public static void SetPosXAccordRefTran(this Transform transform, Transform refTran, float x, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, refTran.position.y, refTran.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, refTran.localPosition.y, refTran.localPosition.z);
                break;
        }
    }
    public static void SetPosYAccordRefTran(this Transform transform, Transform refTran, float y, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(refTran.position.x, y, refTran.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(refTran.localPosition.x, y, refTran.localPosition.z);
                break;
        }
    }
    public static void SetPosZAccordRefTran(this Transform transform, Transform refTran, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(refTran.position.x, refTran.position.y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(refTran.localPosition.x, refTran.localPosition.y, z);
                break;
        }
    }
    /// <summary>
    /// 设置X、Y坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetPosXY(this Transform transform, float x, float y, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, y, transform.position.z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, y, transform.localPosition.z);
                break;

        }

    }

    /// <summary>
    /// 设置Y、Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetPosYZ(this Transform transform, float y, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(transform.position.x, y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(transform.localPosition.x, y, z);
                break;

        }

    }

    /// <summary>
    /// 设置X、Z坐标
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    public static void SetPosXZ(this Transform transform, float x, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, transform.position.y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, transform.localPosition.y, z);
                break;
        }

    }

    public static void SetPosXYZ(this Transform transform, float x, float y, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = new Vector3(x, y, z);
                break;
            case Space.Self:
                transform.localPosition = new Vector3(x, y, z);
                break;

        }

    }

    public static void ResetPosition(this Transform transform, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.position = Vector3.zero;
                break;
            case Space.Self:
                transform.localPosition = Vector3.zero;

                break;

        }
    }

    #endregion

    #region Scale

    public static void ResetScale(this Transform transform)
    {
        transform.localScale = Vector3.one;
    }


    //lossyScale only read,so ignore local or global,only use local.
    public static void SetScale(this Transform transform, float x, float y, float z)
    {

        transform.localScale = new Vector3(x, y, z);
    }
    #endregion


    #region Rotation
    public static void ResetRotation(this Transform transform, Space space)
    {
        if (space == Space.Self)
        {
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
    }

    public static void SetEulerAngles(this Transform transform, float x, float y, float z, Space space)
    {
        switch (space)
        {
            case Space.World:
                transform.eulerAngles = new Vector3(x, y, z);
                break;
            case Space.Self:
                transform.localEulerAngles = new Vector3(x, y, z);
                break;

        }
    }
    #endregion

    /// <summary>
    /// 重置本地position,rotation和scale
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetLocalPRS(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    /// <summary>
    /// 重置世界坐标系下的position,rotation和scale
    /// </summary>
    /// <param name="transform"></param>
    public static void ResetGlobalPRS(this Transform transform)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

}
