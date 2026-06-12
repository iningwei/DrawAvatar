using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssignBonesRef
{

    static BoneNodePath getBoneNodeData(Transform root, string path)
    {
        BoneNodePath nodePath = new BoneNodePath();

        List<BoneNodeData> datas = new List<BoneNodeData>();

        string[] pathSeps = path.Split('/');
        for (int i = 0; i < pathSeps.Length; i++)
        {
            string tmpPath = pathSeps[i];
            for (int j = i - 1; j >= 0; j--)
            {
                tmpPath = pathSeps[j] + "/" + tmpPath;
            }
            var tmpTran = root.Find(tmpPath);
            BoneNodeData data = new BoneNodeData();
            data.name = tmpTran.name;
            data.path = tmpPath;
            data.pos = tmpTran.localPosition;
            data.eulerAngle = tmpTran.localEulerAngles;
            data.scale = tmpTran.localScale;

            datas.Add(data);
        }
        nodePath.path = path;
        nodePath.nodes = datas;


        return nodePath;
    }

    [MenuItem("GameObject/SkinnedMeshRenderer/AssignBonesRef")]
    public static void Assign()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj != null)
        {

            var br = obj.GetOrAddComponent<BonesRef>();
           

            var rootTran = obj.transform.parent.Find("root");
            if (rootTran == null)
            {
                Debug.LogError("root node error,please check");
                return;
            }


            var smr = obj.GetComponent<SkinnedMeshRenderer>();

            string rootBonePath = smr.rootBone.GetHierarchy();
            rootBonePath = rootBonePath.Substring(rootBonePath.IndexOf(@"/root/") + 6);
            Debug.Log("rootBonePath:" + rootBonePath);
            br.rootBoneRefPath = getBoneNodeData(rootTran, rootBonePath);


            var bones = smr.bones;
            br.bonesRefPath = new BoneNodePath[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                string tmpPath = bones[i].GetHierarchy();
                tmpPath = tmpPath.Substring(tmpPath.IndexOf(@"/root/") + 6);
                Debug.Log("boneRefPath:" + tmpPath);

                br.bonesRefPath[i] = getBoneNodeData(rootTran, tmpPath);

            }

            Debug.Log("assign finished!");

        }
    }
}
