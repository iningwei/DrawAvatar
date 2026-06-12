using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GetBlendShapesData
{
    [MenuItem("GameObject/SkinnedMeshRenderer/GetBlendShapesData")]
    static void Get()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {
            var smr = obj.GetComponent<SkinnedMeshRenderer>();
            for (int i = 0; i < 4; i++)
            {
                var w = smr.GetBlendShapeWeight(i);
                Debug.Log($"index:{i}, weight:{w}");
            }
        }
    }
}
