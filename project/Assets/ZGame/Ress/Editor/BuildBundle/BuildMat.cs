using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.RessEditor
{
    public class BuildMat : BuildBase
    {

        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Material);
            if (obj.name.ContainChinese())
            {
                Debug.LogError("resource name should not contain chinese charactor:" + obj.name);
                return false;
            }
            if (obj.name.IsResNameValid() == false)
            {
                Debug.LogError("res file name not valid:" + obj.name);
                return false;
            }
            string path = AssetDatabase.GetAssetPath(obj);

            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
            buildMap[0].assetNames = new string[] { path };
            BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, buildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);
            Debug.Log("-------->build mat bundle:" + obj.name + ", finished");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
    }
}