using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class BoneNodeData
{
    public string name;
    public string path;
    public Vector3 pos;
    public Vector3 eulerAngle;
    public Vector3 scale;
}


[Serializable]
public class BoneNodePath
{
    public string path;
    public List<BoneNodeData> nodes = new List<BoneNodeData>();
}


public class BonesRef : MonoBehaviour
{
    Transform rootBoneTran;
    public BoneNodePath rootBoneRefPath;
    public BoneNodePath[] bonesRefPath;



    void initBonePath(Transform root, BoneNodePath path)
    {

        for (int i = 0; i < path.nodes.Count; i++)
        {
            var tmpData = path.nodes[i];

            if (root.Find(tmpData.path) == null)
            {
                Transform parentTran = root;
                if (tmpData.path.Contains("/"))
                {
                    var parentPath = tmpData.path.Substring(0, tmpData.path.LastIndexOf("/"));
                    parentTran = root.Find(parentPath);
                }

                var obj = new GameObject();
                obj.name = tmpData.name;
                obj.transform.parent = parentTran;
                obj.transform.localPosition = tmpData.pos;
                obj.transform.localEulerAngles = tmpData.eulerAngle;
                obj.transform.localScale = tmpData.scale;
            }
        }

    }

    private SkinnedMeshRenderer smr;
    void Start()
    {
        smr = GetComponent<SkinnedMeshRenderer>();
        if (smr == null)
        {
            return;
        }
        if (rootBoneTran == null)
        {
            rootBoneTran = transform.parent.Find("root");
        }
        initBonePath(rootBoneTran, rootBoneRefPath);


        string[] bonesPath = new string[bonesRefPath.Length];
        for (int i = 0; i < bonesRefPath.Length; i++)
        {
            initBonePath(rootBoneTran, bonesRefPath[i]);
            bonesPath[i] = bonesRefPath[i].path;
        }


        if (smr.rootBone == null)
        {
            smr.rootBone = rootBoneTran.Find(rootBoneRefPath.path);
        }

        var bones = smr.bones;
        if (bones != null)
        {
            for (int i = 0; i < bones.Length; i++)
            {
                if (bones[i] == null)
                {
                    bones[i] = rootBoneTran.Find(bonesPath[i]);
                }
            }
        }
        smr.bones = bones;
    }
}
