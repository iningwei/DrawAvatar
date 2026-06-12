using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.RessEditor;
namespace ZGame.RessEditor
{
    public class BuildObject : BuildBase
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Object);
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

            string path = AssetDatabase.GetAssetPath(obj);
            AssetBundleBuild[] buildMap = new AssetBundleBuild[1];
            buildMap[0].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
            buildMap[0].assetNames = new string[] { path };
            BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, buildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);
            DebugExt.Log("-------->build bundle:" + obj.name + ", finished");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
    }
}