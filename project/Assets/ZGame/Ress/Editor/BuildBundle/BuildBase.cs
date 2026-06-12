using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System;
using System.Threading;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.AB;
using ZGame.UGUIExtention;
using ZGame.Ress.Info;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace ZGame.RessEditor
{
    /// <summary>
    /// 打包基类
    /// </summary>
    public class BuildBase
    {
        public string abPrefix;
        public virtual bool Build(UnityEngine.Object obj)
        {
            Debug.LogError("can not build ab:" + AssetDatabase.GetAssetOrScenePath(obj));
            return false;
        }

        /// <summary>
        /// 对预制件进行统一的检测
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns>有错误则返回false</returns>
        public bool CheckPrefab(GameObject prefab)
        {
            string path = AssetDatabase.GetAssetPath(prefab);
            if (!path.EndsWith(".prefab"))
            {
                DebugExt.LogE("target not a prefab , path:" + path);
                return false;
            }



            var allChild = prefab.GetComponentsInChildren(typeof(Transform), true);
            Component tmpChild = null;
            for (int i = 0; i < allChild.Length; i++)
            {
                tmpChild = allChild[i];

                //检测是否在子物体上误添加了 RootCompInfoHolder
                if (tmpChild is Transform)
                {
                    var tran = tmpChild as Transform;
                    if (tran != prefab.transform)
                    {
                        if (tran.GetComponent<RootCompInfoHolder>() != null)
                        {
                            DebugExt.LogE("duplicate RootCompInfoHolder, path:" + tran.GetHierarchy());
                            return false;
                        }
                    }
                }


                if (BuildCommand.CheckValidOfU2D(tmpChild.gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfRenderer(tmpChild.gameObject) == false)
                {
                    return false;
                }
                if (BuildCommand.CheckValidOfUGUI(tmpChild.gameObject) == false)
                {
                    return false;
                }
            }

            return true;
        }


        public bool CheckScene(UnityEngine.Object sceneObj, out GameObject rootObj)
        {
            rootObj = null;
            string path = AssetDatabase.GetAssetPath(sceneObj);
            if (!path.EndsWith(".unity"))
            {
                DebugExt.LogE("target not a scene , path:" + path);
                return false;
            }

            //当前打开的场景就是目标场景
            Scene targetScene;
            var activeScene = EditorSceneManager.GetActiveScene();
            if (activeScene != null && activeScene.name == sceneObj.name)
            {
                Debug.Log("now opend scene is target scene");
                targetScene = activeScene;
            }
            else
            {
                EditorSceneManager.SaveOpenScenes();//先保存当前场景

                targetScene = EditorSceneManager.OpenScene(path);//打开目标场景
            }




            var rootObjs = targetScene.GetRootGameObjects();//获得根节点
            //从根节点中找名称为Root的节点
            GameObject theRootObj = null;
            for (int i = 0; i < rootObjs.Length; i++)
            {
                var tmpObj = rootObjs[i];
                if (tmpObj.name == "Root")
                {
                    theRootObj = tmpObj;
                    rootObj = theRootObj;
                    break;
                }
            }

            if (theRootObj == null)
            {
                DebugExt.LogE("error, scene have no Root node");
                return false;
            }
            RootCompInfoHolder rcih = theRootObj.transform.GetComponent<RootCompInfoHolder>();
            if (rcih == null)
            {
                DebugExt.LogE("error, scene's Root node has not attach Component:RootCompInfoHolder");
                return false;
            }


            //遍历根节点，检测是否在非根节点Root外的节点上添加了RootCompInfoHolder
            for (int j = 0; j < rootObjs.Length; j++)
            {
                var tmpObj = rootObjs[j];

                var allChild = tmpObj.GetComponentsInChildren(typeof(Transform), true);
                Component tmpChild = null;
                for (int i = 0; i < allChild.Length; i++)
                {
                    tmpChild = allChild[i];

                    //检测是否在子物体上误添加了 RootCompInfoHolder
                    if (tmpChild is Transform)
                    {
                        var tran = tmpChild as Transform;
                        if (tran != theRootObj.transform)
                        {
                            if (tran.GetComponent<RootCompInfoHolder>() != null)
                            {
                                DebugExt.LogE("duplicate RootCompInfoHolder, path:" + tran.GetHierarchy());
                                return false;
                            }
                        }
                    }


                    //场景中的非Root节点就不检查了。只需要针对Root节点检查
                    ////if (BuildCommand.CheckValidOfU2D(tmpChild.gameObject) == false)
                    ////{
                    ////    return false;
                    ////}
                    ////if (BuildCommand.CheckValidOfRenderer(tmpChild.gameObject) == false)
                    ////{
                    ////    return false;
                    ////}
                    ////if (BuildCommand.CheckValidOfUGUI(tmpChild.gameObject) == false)
                    ////{
                    ////    return false;
                    ////}
                }
            }

            if (BuildCommand.CheckValidOfU2D(theRootObj) == false)
            {
                return false;
            }
            if (BuildCommand.CheckValidOfRenderer(theRootObj) == false)
            {
                return false;
            }
            if (BuildCommand.CheckValidOfUGUI(theRootObj) == false)
            {
                return false;
            }

            return true;
        }

        public void LogBuildMap(AssetBundleBuild[] finalBuildMap)
        {
            Debug.Log("------------>");
            for (int i = 0; i < finalBuildMap.Length; i++)
            {
                Debug.Log("buildMap " + i + ", name:" + finalBuildMap[i].assetBundleName + ", path:" + finalBuildMap[i].assetNames[0]);
            }
        }
    }
}