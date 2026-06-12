using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetBonesData
{
    [MenuItem("GameObject/SkinnedMeshRenderer/GetBonesData")]
    public static void Get()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            var smr = obj.GetComponent<SkinnedMeshRenderer>();
            var bones = smr.bones;
            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i] != null)
                {
                    string path = bones[i].GetHierarchy();
                    Debug.Log("used bone:" + path);
                }
                else
                {
                    Debug.LogError($"bones[{i}] is null");
                }

            }
        }
    }
}
