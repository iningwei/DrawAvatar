using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SetObjRenderVisible : Editor
{
    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Opossite")]
    static void SetObjRenderStatusOpossite()
    {
        var objs = Selection.objects;
        for (int k = 0; k < objs.Length; k++)
        {
            if (objs[k] is GameObject == false)
            {
                continue;
            }
            GameObject obj = objs[k] as GameObject;
            var renders = obj.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
            {
                renders[i].enabled = !renders[i].enabled;
            }
            EditorUtility.SetDirty(obj);
        }

        Debug.Log("finish SetObjAndChildsRenderStatus opossite");
    }

    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Visible")]
    static void SetObjRenderStatusVisible()
    {
        var objs = Selection.objects;
        for (int k = 0; k < objs.Length; k++)
        {
            if (objs[k] is GameObject == false)
            {
                continue;
            }
            GameObject obj = objs[k] as GameObject;
            var renders = obj.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
            {
                renders[i].enabled = true;
            }

            EditorUtility.SetDirty(obj);
        }
        Debug.Log("finish SetObjAndChildsRenderStatus visible");
    }

    [MenuItem("GameObject/SetObjAndChildsRenderStatus/Unvisible")]
    static void SetObjRenderStatusUnvisible()
    {
        var objs = Selection.objects;
        for (int k = 0; k < objs.Length; k++)
        {
            if (objs[k] is GameObject == false)
            {
                continue;
            }
            GameObject obj = objs[k] as GameObject;
            var renders = obj.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < renders.Length; i++)
            {
                renders[i].enabled = false;
            }

            EditorUtility.SetDirty(obj);
        }
        Debug.Log("finish SetObjAndChildsRenderStatus unvisible");
    }
}
