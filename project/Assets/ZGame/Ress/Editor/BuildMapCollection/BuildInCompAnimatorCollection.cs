using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class BuildInCompAnimatorCollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();
        return compInfos;//TODO：这里暂时return

        var animatorChilds = new List<Animator>();
        obj.GetComponentsInChildren<Animator>(true, animatorChilds);

        for (int i = 0; i < animatorChilds.Count; i++)
        {
            Animator child = animatorChilds[i];

            if (child.runtimeAnimatorController == null)
            {
                Debug.LogWarning("warning,AnimatorController not set:" + child.transform.GetHierarchy());
                continue;
            }

            //compInfos add elements
            string acName = child.runtimeAnimatorController.name;
            CompInfo buildInCompAnimatorInfo = new BuildInCompAnimatorInfo(
                child.transform,
                child,
                  acName);
            compInfos.Add(buildInCompAnimatorInfo);

        }

        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();
        return buildMap;//TODO:这里暂时return

        var animatorChilds = new List<Animator>();
        obj.GetComponentsInChildren<Animator>(true, animatorChilds);
        Debug.Log(obj.name + "'s Animator count:" + animatorChilds.Count);


        for (int i = 0; i < animatorChilds.Count; i++)
        {
            Animator child = animatorChilds[i];

            if (child.runtimeAnimatorController == null)
            {
                Debug.LogWarning("warning,AnimatorController not set:" + child.transform.GetHierarchy());
                continue;
            }

            string acName = child.runtimeAnimatorController.name;
            string acPath = AssetDatabase.GetAssetPath(child.runtimeAnimatorController);
            //buildMap add ac element 
            this.AddBundleBuildData(acName, acPath, ABType.AnimatorController, ref buildMap);
        }

        return buildMap;
    }
}
