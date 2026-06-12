using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



public class ExportMissingMeshOrMatMsg : Editor
{

    [MenuItem("GameObject/检测丢失Mesh或者Mat的物体", false, 13)]
    public static void Export()
    {
        GameObject root = Selection.activeObject as GameObject;
        Transform[] all = root.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < all.Length; i++)
        {
            Transform tran = all[i];

            MeshFilter mf = tran.GetComponent<MeshFilter>();
            if (mf != null)
            {
                if (mf.sharedMesh == null)
                {
                    string path = mf.gameObject.name;
                    Transform tempParent = mf.transform.parent;
                    while (tempParent != null)
                    {
                        path = tempParent.name + "/" + path;
                        tempParent = tempParent.parent;
                    }


                    Debug.LogError("MeshFilter:" + path + "  , have no mesh!!");
                }
            }



            MeshRenderer meshRenderer = tran.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                Material[] mats = meshRenderer.sharedMaterials;

                if (mats.Length > 0)
                {
                    for (int j = 0; j < mats.Length; j++)
                    {
                        Material mat = mats[j];

                        if (mat == null)
                        {
                            string path = meshRenderer.gameObject.name;
                            Transform tempParent = meshRenderer.transform.parent;
                            while (tempParent != null)
                            {
                                path = tempParent.name + "/" + path;
                                tempParent = tempParent.parent;
                            }

                            Debug.LogError("--------->mesh renderer:" + path + ", have materials with no data");
                        }
                    }
                }
            }


        }

        Debug.Log("检测完毕");
    }
}