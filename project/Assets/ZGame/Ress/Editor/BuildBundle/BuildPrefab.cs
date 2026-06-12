using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;
using ZGame;

namespace ZGame.RessEditor
{
    public class BuildPrefab : BuildBase
    {
        public override bool Build(Object obj)
        {
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
            if (CheckPrefab(obj as GameObject) == false)
            {
                return false;
            }
            string path = AssetDatabase.GetAssetPath(obj);

            //创建临时预制体，用来操作以及用来打最终的目标AB
            string tmpPath = "Assets/temp_for_prefab/" + obj.name + ".prefab";
            AssetDatabase.DeleteAsset(tmpPath);
            AssetDatabase.CopyAsset(path, tmpPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

            var tmpPrefab = AssetDatabase.LoadAssetAtPath(tmpPath, typeof
                (GameObject)) as GameObject;
            if (tmpPrefab == null)
            {
                Debug.LogError("load temp prefab error, path:" + tmpPath);
                return false;
            }

            //填充 RootCompInfoHolder
            BuildCommand.FillRootCompInfoHolder(tmpPrefab);
            //填充 DynamicCompInfoHolder
            BuildCommand.FillDynamicCompInfoHolder(tmpPrefab);

            //保存Holder信息
            EditorUtility.SetDirty(tmpPrefab);
            AssetDatabase.SaveAssets();


            List<AssetBundleBuild> collectedBuildList = new List<AssetBundleBuild>(BuildCommand.GetGameObjectAssetBundleBuildMap(tmpPrefab).Values);



            //处理真实目标,以及设置依赖关系          
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            AssetBundleBuild[] finalBuildMap = new AssetBundleBuild[2 + collectedBuildList.Count];
            finalBuildMap[0] = BuildCommand.GetCommonMap();//commonResMap
            for (int i = 0; i < collectedBuildList.Count; i++)
            {
                finalBuildMap[i + 1] = collectedBuildList[i];
            }
            finalBuildMap[finalBuildMap.Length - 1].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
            finalBuildMap[finalBuildMap.Length - 1].assetNames = new string[] { tmpPath };//自己


            //打印buildMap
            this.LogBuildMap(finalBuildMap);

            BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, finalBuildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);

            //删除临时资源
            //AssetDatabase.DeleteAsset(tmpPath);

            Debug.Log("-------->build prefab bundle:" + obj.name + ", finished");
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            return true;
        }
    }
}