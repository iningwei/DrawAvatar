using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;

namespace ZGame.RessEditor
{
    //场景确保涉及到动态资源的节点都在Root节点下
    //其它诸如后处理、光照等节点可以挂载外部
    //通过对Root节点挂载RootCompInfoHolder来实现通用的依赖加载
    public class BuildScene : BuildBase
    {
        public override bool Build(Object obj)
        {
            abPrefix = ABTypeUtil.GetPreFix(ABType.Scene);
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


            GameObject rootObj;
            if (CheckScene(obj, out rootObj) == false)
            {
                return false;
            }
            if (rootObj != null)
            {

                string objPath = AssetDatabase.GetAssetOrScenePath(obj);

                //------------->这后面的流程和对预制件的处理是一样的
                //填充 RootCompInfoHolder
                BuildCommand.FillRootCompInfoHolder(rootObj);
                //填充 DynamicCompInfoHolder
                BuildCommand.FillDynamicCompInfoHolder(rootObj);


                //保存Holder信息
                //---->
                //TAKE CARE:
                //If rootObj is a prefab.
                //SetDirty to it,then call SaveAssets().
                //But you can still find a * in your editor.
                //After build scene assetbundle,you can find rootObj's RootCompInfoHolder's attached message lost.

                //Tested many methods:
                //keep rootObj is not a prefab,and then SetDirty() to rootObj and scene,but still have chance accur
                //<----
                EditorUtility.SetDirty(rootObj);
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();
                EditorSceneManager.SaveOpenScenes();

                List<AssetBundleBuild> collectedBuildList = new List<AssetBundleBuild>(BuildCommand.GetGameObjectAssetBundleBuildMap(rootObj).Values);


                //处理真实目标,以及设置依赖关系
                AssetBundleBuild[] finalBuildMap = new AssetBundleBuild[2 + collectedBuildList.Count];
                finalBuildMap[0] = BuildCommand.GetCommonMap();
                for (int i = 0; i < collectedBuildList.Count; i++)
                {
                    finalBuildMap[i + 1] = collectedBuildList[i];
                }
                finalBuildMap[finalBuildMap.Length - 1].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
                finalBuildMap[finalBuildMap.Length - 1].assetNames = new string[] { objPath };

                //打印buildMap
                LogBuildMap(finalBuildMap);

                BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, finalBuildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);
            }

            Debug.Log("-------->build scene bundle:" + obj.name + ", finished");
            AssetDatabase.Refresh();
            return true;
        }


        public void GetSceneRootCompInfo(Object obj)
        {
            string abPrefix = ABTypeUtil.GetPreFix(ABType.Scene);
            if (obj.name.ContainChinese())
            {
                Debug.LogError("resource name should not contain chinese charactor:" + obj.name);
                return;
            }
            if (obj.name.IsResNameValid() == false)
            {
                Debug.LogError("res file name not valid:" + obj.name);
                return;
            }


            GameObject rootObj;
            if (CheckScene(obj, out rootObj) == false)
            {
                return;
            }
            if (rootObj != null)
            {

                string objPath = AssetDatabase.GetAssetOrScenePath(obj);

                //------------->这后面的流程和对预制件的处理是一样的
                //填充 RootCompInfoHolder
                BuildCommand.FillRootCompInfoHolder(rootObj);
                //填充 DynamicCompInfoHolder
                BuildCommand.FillDynamicCompInfoHolder(rootObj);



                EditorUtility.SetDirty(rootObj);
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                Debug.Log("get scene root comp info finished");
            }
        }


        public void BuildSceneAB(Object obj)
        {
            string abPrefix = ABTypeUtil.GetPreFix(ABType.Scene);



            GameObject rootObj;
            if (CheckScene(obj, out rootObj) == false)
            {
                return;
            }
            if (rootObj != null)
            {

                string objPath = AssetDatabase.GetAssetOrScenePath(obj);


                List<AssetBundleBuild> collectedBuildList = new List<AssetBundleBuild>(BuildCommand.GetGameObjectAssetBundleBuildMap(rootObj).Values);


                //处理真实目标,以及设置依赖关系           
                AssetBundleBuild[] finalBuildMap = new AssetBundleBuild[2 + collectedBuildList.Count];
                finalBuildMap[0] = BuildCommand.GetCommonMap();
                for (int i = 0; i < collectedBuildList.Count; i++)
                {
                    finalBuildMap[i + 1] = collectedBuildList[i];
                }
                finalBuildMap[finalBuildMap.Length - 1].assetBundleName = abPrefix + obj.name.ToLower() + IOTools.abSuffix;
                finalBuildMap[finalBuildMap.Length - 1].assetNames = new string[] { objPath };

                BuildPipeline.BuildAssetBundles(BuildConfig.outputPath, finalBuildMap, BuildConfig.options, EditorUserBuildSettings.activeBuildTarget);
            }

            AssetDatabase.Refresh();
            DebugExt.Log("-------->build scene bundle:" + obj.name + ", finished");

            BuildCommand.DeleteUselessAfterBuildAB();
        }
    }
}
