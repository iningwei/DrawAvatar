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

public class ABScene
{
    static ABScene()
    {
        //SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        //SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
    }

    //private static void SceneManager_sceneUnloaded(Scene arg0)
    //{
    //    Debug.LogError($"scene:{arg0.name} unloaded");
    //}

    //private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    //{
    //    Debug.LogError($"scene:{arg0.name} loaded, mode:{arg1.ToSafeString()}");
    //}

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
        removeLightmaps(name, null);
    }



    static Action<string, LoadSceneMode, Action, bool> loadFinishHandle = (name, loadMode, lightmapAttachedCallback, sync) =>
    {
        Debug.Log(name + " ,scene load finish handle");
        Scene curScene = SceneManager.GetSceneByName(name);
        GameObject[] rootObjs = curScene.GetRootGameObjects();
        //find node "Root",get it's ref then   load them.
        int count = rootObjs.Length;
        GameObject targetRootNode = null;
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
                ABScene.LoadSceneLightmaps(name, loadMode, holder.lightmapInfo, lightmapAttachedCallback, sync);
            }
            else
            {
                lightmapAttachedCallback?.Invoke();
            }


            ABManager.Instance.AddRootCompInfoHolderDestroyNotice(targetRootNode);
        }
        else
        {
            Debug.LogError("scene:" + name + ", have no Root node or no Root node with component RootCompInfoHolder");
        }
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

    public static void Load(string name, LoadSceneMode loadSceneMode, Action<UnityEngine.Object> callback, Action lightmapAttachedCallback, bool sync)
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
                    loadFinishHandle(name, loadSceneMode, lightmapAttachedCallback, sync);
                    callback?.Invoke(null);
                    Debug.Log("unload scene ab:" + name);
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
                CoroutineManager.Instance.AddCoroutine(loadSceneEnumerator(name, loadSceneMode, (obj) =>
                {
                    if (callback != null)
                    {
                        callback(obj);
                    }
                    Debug.Log("scene loaded ,unload it's ab");
                    ab.Unload(false);
                }, lightmapAttachedCallback));
            }, false);
        }
    }



    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="loadSceneMode"></param>
    /// <param name="callback"></param>
    /// <param name="lightmapAttachedCallback"></param>
    /// <returns></returns>
    static IEnumerator loadSceneEnumerator(string name, LoadSceneMode loadSceneMode, Action<UnityEngine.Object> callback, Action lightmapAttachedCallback)
    {
        AsyncOperation asy = SceneManager.LoadSceneAsync(name, loadSceneMode);

        yield return asy;


        loadFinishHandle(name, loadSceneMode, lightmapAttachedCallback, false);


        if (callback != null)
        {
            callback.Invoke(null);
        }
    }


    static List<SceneLightmapMsg> sceneLightmapMsgList = new List<SceneLightmapMsg>();

    static void removeLightmaps(string sceneName, Action lightmapAttachedCallback)
    {
        int targetIndex = -1;
        for (int i = 0; i < sceneLightmapMsgList.Count; i++)
        {
            if (sceneLightmapMsgList[i].sceneName == sceneName)
            {
                targetIndex = i;
            }

        }

        if (targetIndex != -1)
        {
            totalLightmapCount -= sceneLightmapMsgList[targetIndex].count;
            sceneLightmapMsgList.RemoveAt(targetIndex);

            refreshLightmaps(lightmapAttachedCallback);
        }
    }


    static int totalLightmapCount = 0;
    static void addLightmaps(string sceneName, LoadSceneMode loadMode, LightmapData[] datas, List<LightmapRenderInfo> renderInfos, Action lightmapAttachedCallback)
    {

        if (loadMode == LoadSceneMode.Single)
        {
            sceneLightmapMsgList.Clear();
            totalLightmapCount = 0;
        }


        if (datas != null && datas.Length > 0)
        {
            SceneLightmapMsg msg = new SceneLightmapMsg();

            msg.sceneName = sceneName;
            msg.count = datas.Length;

            msg.datas = datas;
            msg.renderInfos = renderInfos;

            totalLightmapCount += datas.Length;

            sceneLightmapMsgList.Add(msg);
        }


        refreshLightmaps(lightmapAttachedCallback);
    }


    static void refreshLightmaps(Action lightmapAttachedCallback)
    {
        if (sceneLightmapMsgList != null && sceneLightmapMsgList.Count > 0)
        {
            LightmapData[] destination = new LightmapData[totalLightmapCount];

            int offset = 0;
            for (int i = 0; i < sceneLightmapMsgList.Count; i++)
            {
                var tmpDatas = sceneLightmapMsgList[i].datas;
                if (tmpDatas != null && tmpDatas.Length > 0)
                {
                    tmpDatas.CopyTo(destination, offset);

                    //reset  renderInfo's index
                    var tmpRenderInfos = sceneLightmapMsgList[i].renderInfos;
                    if (tmpRenderInfos != null)
                    {
                        for (int j = 0; j < tmpRenderInfos.Count; j++)
                        {
                            var renderInfo = tmpRenderInfos[j];

                            renderInfo.curLightmapIndex = renderInfo.originLightmapIndex + offset;
                        }
                    }

                    //set offset
                    offset += tmpDatas.Length;
                }
            }

            //assign lightmap
            //LightmapSettings.lightmapsMode = LightmapsMode.NonDirectional;
            LightmapSettings.lightmaps = destination;

            Debug.Log("set lightmaps, count:" + destination.Length + ", time:" + UnityEngine.Time.time);
            //reassign renderInfo
            for (int k = 0; k < sceneLightmapMsgList.Count; k++)
            {
                fillRenderers(sceneLightmapMsgList[k].renderInfos);
            }

            lightmapAttachedCallback?.Invoke();
        }

    }

    public static void LoadSceneLightmaps(string sceneName, LoadSceneMode loadMode, LightmapInfo info, Action lightmapAttachedCallback, bool sync = true)
    {

        Debug.Log("begin LoadSceneLightmaps, sync:" + sync);
        int count = info.lightmapColors.Count;
        LightmapData[] datas = new LightmapData[count];


        int loadedCount = 0;
        for (int i = 0; i < count; i++)
        {
            int index = i;

            string colorName = info.lightmapColors[i];
            //Debug.Log("begin load lightmap index:" + index + ", time:" + Time.time);
            ABManager.Instance.LoadTexture(colorName, (t) =>
            {
                loadedCount++;
                datas[index] = new LightmapData();
                datas[index].lightmapColor = t as Texture2D;
                //Debug.Log("finish load lightmap index:" + index + ", lightmapColor name:" + t.name + ", time:" + Time.time);
                if (loadedCount == count)
                {
                    Debug.Log($"all lightmap texture loaded,count:{loadedCount},time:{Time.time},to set lightmaps ----->");

                    //LightmapSettings.lightmaps = datas;
                    addLightmaps(sceneName, loadMode, datas, info.lightmapRenders, lightmapAttachedCallback);
                }

            }, sync);
        }

    }

    static void fillRenderers(List<LightmapRenderInfo> renderInfos)
    {
        int count = renderInfos.Count;
        Debug.Log("fill lightmap renderers,count:" + count);
        LightmapRenderInfo info;
        MeshRenderer render;
        for (int i = 0; i < count; i++)
        {
            info = renderInfos[i];
            if (info.tran == null)
            {
                Debug.LogError("something error occur!!!please check and fix it");
                continue;
            }

            render = info.tran.GetComponent<MeshRenderer>();

            render.lightmapIndex = info.curLightmapIndex;
            //render.lightmapScaleOffset = info.lightmapScaleOffset;

            //TODO:
            //WARNING:The renderer SM_GlassWall is a part of a static batch. Setting the lightmap scale and offset will not affect the rendering. The scale and offset is already burnt into the lightmapping UVs in the static batch.
        }
    }
}
