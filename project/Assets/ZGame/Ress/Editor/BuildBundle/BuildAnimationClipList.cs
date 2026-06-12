using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame;
using ZGame.RessEditor;
using ZGame.Res;

public class BuildAnimationClipList : BuildBase
{
    public override bool Build(Object obj)
    {
        abPrefix = ABTypeUtil.GetPreFix(ABType.AnimationClip);
        if (obj.name.ContainChinese())
        {
            DebugExt.LogE("res name should not have Chinese charactor，" + obj.name);
            return false;
        }
        if (obj.name.IsResNameValid() == false)
        {
            Debug.LogError("res file name not valid:" + obj.name);
            return false;
        }
        AnimationClipList clipList = (obj as GameObject).GetComponent<AnimationClipList>();
        if (clipList)
        {
            Dictionary<string, string> suitClipDic = new Dictionary<string, string>();//key:name; value:path
            for (int i = 0; i < clipList.clips.Length; i++)
            {
                var clip = clipList.clips[i];
                string clipName = "";
                string clipPath = "";
                if (clip)
                {
                    clipName = clip.name;
                    clipPath = AssetDatabase.GetAssetPath(clip);

                    //clip资源若是在fbx内，则需要剥离
                    if (clipPath.EndsWith(".fbx") || clipPath.EndsWith(".FBX"))
                    {
                        AssetDatabaseExt.ExportAssetToFBXTmpFolder(clipPath, "anim");
                        clipPath = AssetDatabaseExt.GetAssetPathFromFBXTmpFolder(clipName, "anim");
                    }

                    if (!string.IsNullOrEmpty(clipPath) && suitClipDic.ContainsKey(clipName) == false)
                    {
                        suitClipDic.Add(clipName, clipPath);
                    }
                }
            }


            AssetBundleBuild[] buildMap = new AssetBundleBuild[suitClipDic.Count];
            int index = 0;
            foreach (var item in suitClipDic)
            {
                buildMap[index].assetBundleName = abPrefix + item.Key.ToLower() + IOTools.abSuffix;
                buildMap[index].assetNames = new string[] { item.Value };
                BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, buildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);
                Debug.Log("-------->build animation clip bundle:" + item.Key + ", finished");
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
        else
        {
            Debug.LogError("obj have no AnimationClipList component:" + AssetDatabase.GetAssetPath(obj));
            return false;
        }

    }
}
