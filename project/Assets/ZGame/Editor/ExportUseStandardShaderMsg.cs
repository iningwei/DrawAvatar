using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExportUseStandardShaderMsg : Editor
{

    [MenuItem("GameObject/检测使用了Standard shader的物体", false, 12)]
    public static void Export()
    {
        GameObject root = Selection.activeObject as GameObject;
        Transform[] all = root.GetComponentsInChildren<Transform>(true);
        //Debug.LogError("all count:" + all.Length);
        for (int i = 0; i < all.Length; i++)
        {
            Transform tran = all[i];
            MeshRenderer meshRenderer = tran.GetComponent<MeshRenderer>();
            if (meshRenderer == null)
            {
                continue;
            }

            Material[] mats = meshRenderer.sharedMaterials;
             
            if (mats.Length > 0)
            {
                for (int j = 0; j < mats.Length; j++)
                {
                    Material mat = mats[j];

                    if (mat != null && mat.shader != null && mat.shader.name.Contains("Standard"))
                    {
                        string path = meshRenderer.gameObject.name;
                        Transform tempParent = meshRenderer.transform.parent;
                        while (tempParent != null)
                        {
                            path = tempParent.name + "/" + path;
                            tempParent = tempParent.parent;
                        }

                        Debug.LogError("--------->mat:" + mat.name + ",使用了Standard shader, 相关节点路径：" + path);
                    }
                }
            }
        }
    }
}
