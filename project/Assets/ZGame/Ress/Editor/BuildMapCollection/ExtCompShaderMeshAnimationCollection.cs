 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class ExtCompShaderMeshAnimationCollection : CompResCollection, IRefResCollection
{
    //META NEW TODO:zhouhui
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
         List<CompInfo> compInfos = new List<CompInfo>();

        //////    var animatorList = new List<ShaderMeshAnimator>();
        //////    obj.GetComponentsInChildren<ShaderMeshAnimator>(true, animatorList);

        //////    for (int i = 0; i < animatorList.Count; i++)
        //////    {
        //////        ShaderMeshAnimator animator = animatorList[i];

        //////        if (animator.defaultMeshAnimation == null || animator.meshAnimations == null || animator.meshAnimations.Length == 0)//异常情况。
        //////        {
        //////            Debug.LogWarning("warning,ShaderMeshAnimator:" + animator.transform.GetHierarchy() + "  animation not set");
        //////            continue;
        //////        }

        //////        List<string> animations = new List<string>();
        //////        for (int j = 0; j < animator.meshAnimations.Length; j++)
        //////        {
        //////            animations.Add(animator.meshAnimations[j].name);
        //////        }
        //////        CompInfo extCompShaderMeshAnimationInfo = new ExtCompShaderMeshAnimationInfo(
        //////                animator.transform,
        //////                animator,
        //////                animator.defaultMeshAnimation.name, animations);
        //////        compInfos.Add(extCompShaderMeshAnimationInfo);

        //////    }

      return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
         Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();

        //////    var animatorChilds = new List<ShaderMeshAnimator>();
        //////    obj.GetComponentsInChildren<ShaderMeshAnimator>(true, animatorChilds);
        //////    Debug.Log(obj.name + "'s ShaderMeshAnimation count:" + animatorChilds.Count);


        //////    for (int i = 0; i < animatorChilds.Count; i++)
        //////    {
        //////        ShaderMeshAnimator animator = animatorChilds[i];

        //////        if (animator.defaultMeshAnimation == null || animator.meshAnimations == null || animator.meshAnimations.Length == 0)//异常情况。
        //////        {
        //////            Debug.LogWarning("warning,ShaderMeshAnimator:" + animator.transform.GetHierarchy() + "  animation not set");
        //////            continue;
        //////        }


        //////        //default animation
        //////        string defaultAnimName = animator.defaultMeshAnimation.name;
        //////        string defaultAnimPath = AssetDatabase.GetAssetPath(animator.defaultMeshAnimation);
        //////        //buildMap add element 
        //////        this.AddBundleBuildData(defaultAnimName, defaultAnimPath, ABType.Object, ref buildMap);

        //////        for (int j = 0; j < animator.meshAnimations.Length; j++)
        //////        {
        //////            string animName = animator.meshAnimations[j].name;
        //////            string aniPath = AssetDatabase.GetAssetPath(animator.meshAnimations[j]);

        //////            //buildMap add element 
        //////            this.AddBundleBuildData(animName, aniPath, ABType.Object, ref buildMap);
        //////        }
        //////    }
     return buildMap;
    }
}
