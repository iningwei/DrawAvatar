using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection; 
using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame;
using ZGame.Ress;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;
using ZGame.TimerTween;

public class ABLargeScene
{
    //unity api now only support async unload scene.
    public static void Unload(string name, Action callback)
    {
        if (isSceneExist(name) == false)
        {
            Debug.LogError($"no scene with name: {name}, can not unload");
            return;
        }
        Debug.LogError("start unload:" + Time.time);


        CoroutineManager.Instance.AddCoroutine(unloadSceneEnumerator(name, callback));

    }

    static IEnumerator unloadSceneEnumerator(string name, Action callback)
    {
        AsyncOperation op = SceneManager.UnloadSceneAsync(name);
        while (op.isDone == false)
        {
            yield return null;
        }
        callback?.Invoke();

    }



    static System.Action<string, LoadSceneMode, Vector3, System.Action, bool> loadFinishHandle = (name, loadMode, initPos, initChunksLoadedCallback, sync) =>
    {
        Scene curScene = SceneManager.GetSceneByName(name);
        GameObject[] rootObjs = curScene.GetRootGameObjects();
        //find node "Root",get it's ref then   load them.
        int count = rootObjs.Length;
        GameObject targetRootNode = null;
        //设置Directional Light的强度//TODO:后面删除
        for (int i = 0; i < count; i++)
        {
            if (rootObjs[i].name == "Directional Light")
            {
                rootObjs[i].GetComponent<Light>().intensity = 1f;
                break;
            }
        }

        for (int i = 0; i < count; i++)
        {
            if (rootObjs[i].name == "Root")
            {
                if (rootObjs[i].GetComponent<RootCompInfoHolder>())
                {
                    targetRootNode = rootObjs[i];
                    break;
                }
                else
                {
                    Debug.LogError("Root node have no component RootCompInfoHolder");
                }
            }
        }
        if (targetRootNode != null)
        {
            ABManager.Instance.FillReferenceOfRootCompInfoHolder(targetRootNode, sync);

            var holder = targetRootNode.GetComponent<RootCompInfoHolder>();
            if (holder.lightmapInfo != null && holder.lightmapInfo.lightmapColors != null && holder.lightmapInfo.lightmapColors.Count > 0 && holder.lightmapInfo.lightmapRenders != null && holder.lightmapInfo.lightmapRenders.Count > 0)
            {
                //基础场景就不烘焙了。
            }
            ABManager.Instance.AddRootCompInfoHolderDestroyNotice(targetRootNode);
        }
        else
        {
            Debug.LogError("scene:" + name + ", have no Root node or no Root node with component RootCompInfoHolder");
        }

        //MapLoader.Instance.Init(name, 60);
        //MapLoader.Instance.PreLoad(initPos, initChunksLoadedCallback);
    };

    static bool isSceneExist(string name)
    {
        int count = SceneManager.sceneCount;
        for (int i = 0; i < count; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            if (s.name == name)
            {
                return true;
            }
        }
        return false;
    }

    public static void Load(string name, LoadSceneMode loadSceneMode, Vector3 initPos, Action<UnityEngine.Object> callback, Action initChunksLoadedCallback, bool sync)
    {

        //check whether scene is loaded
        //be careful not use SceneManager.GetSceneByName(name) to identify whether the target scene is loaded.
        //for this api will return a unvalid scene if the scene not exist.
        if (isSceneExist(name))
        {
            Debug.LogError($"scene:{name} already loaded");
            return;
        }

        if (sync)
        {
            Debug.LogError("begin load large scene, sync:" + sync.ToString() + ", initPos:" + initPos);
            AB.Load(name, ABType.Scene, (objs) =>
            {
                AssetBundle ab = objs[0] as AssetBundle;
                //now scene is in RAM,we can use SceneManager to load.
                SceneManager.LoadScene(name, loadSceneMode);

                //场景加载后必须延迟至少一帧，否则场景无法准备好
                //导致的问题诸如：
                //1，生成角色，场景中无法显示，但是不报错；
                //2，loadFinishHandle方法中查找场景中的Root节点，是找不到的 
                Action delayCall = () =>
                {
                    loadFinishHandle(name, loadSceneMode, initPos, initChunksLoadedCallback, sync);
                    callback?.Invoke(null);
                    Debug.Log("unload scene ab");
                    ab.Unload(false);
                };

                CoroutineManager.Instance.DelayOneFrameCall(delayCall);

            }, true);
        }
        else
        {
            AB.Load(name, ABType.Scene, (objs) =>
            {
                AssetBundle ab = objs[0] as AssetBundle;

                //------------------->异步方式加载
                //now scene is in RAM,we can use SceneManager to load.
                CoroutineManager.Instance.AddCoroutine(loadSceneEnumerator(name, loadSceneMode, initPos, (obj) =>
                {
                    if (callback != null)
                    {
                        callback(obj);
                    }
                    Debug.Log("scene loaded ,unload it's ab");
                    ab.Unload(false);
                }, initChunksLoadedCallback));
            }, false);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="loadSceneMode"></param>
    /// <param name="callback"></param>
    /// <param name="initChunksLoadedCallback"></param>
    /// <returns></returns>
    static IEnumerator loadSceneEnumerator(string name, LoadSceneMode loadSceneMode, Vector3 initPos, Action<UnityEngine.Object> callback, Action initChunksLoadedCallback)
    {
        AsyncOperation asy = SceneManager.LoadSceneAsync(name, loadSceneMode);

        yield return asy;


        loadFinishHandle(name, loadSceneMode, initPos, initChunksLoadedCallback, false);


        if (callback != null)
        {
            callback.Invoke(null);
        }
    }




}
