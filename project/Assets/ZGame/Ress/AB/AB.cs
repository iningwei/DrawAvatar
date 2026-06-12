using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ZGame.Obfuscation;


namespace ZGame.Ress.AB
{

    public enum ABLoadingState
    {
        Unload,
        Loading,
        Loaded,
    }


    public class AsyncLoadingMsg
    {
        public long asyncId = 0;
        public AssetBundleCreateRequest req;
        public Action<UnityEngine.Object[]> callbacks;
    }

    /// <summary>
    /// AssetBundle资源的基类
    /// </summary>
    public class AB
    {
        /// <summary>
        /// async loading dic,to solve async load the same res
        /// </summary>
        static Dictionary<string, AsyncLoadingMsg> asyncLoadingDic = new Dictionary<string, AsyncLoadingMsg>();



        static string getABRealNameFromABPath(string fullPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            if (ConfigUtility.Data.IsABResNameCrypto)
            {
                fileName = DES.DecryptStrFromHex(fileName, ConfigUtility.Data.ABResNameCryptoKey);
            }


            return fileName;
        }

        public static void Load(string name, ABType abType, Action<UnityEngine.Object[]> callback, bool isSync, bool keepAB = false)
        {
            ABOperator.Instance.Add(new ABRes(name, abType, callback, isSync, keepAB));
        }


        #region not used
        ///////// <summary>
        /////////name不带后缀，不带前缀
        ///////// </summary>
        ///////// <param name="name"></param>
        ///////// <returns></returns>
        //////public static void Load(string name, ABType abType, Action<UnityEngine.Object[]> callback, bool keepAB = false)
        //////{
        //////    ABOperator.Instance.Add(new ABRes(name, abType, callback, true, keepAB));

        //////    return;
        //////    //////AssetBundle ab = null;
        //////    //////UnityEngine.Object[] objs = null;
        //////    ////////---->
        //////    ////////here handle a special case:
        //////    ////////Load a ab sync then load it async at the same time.
        //////    ////////Normally it cost an error:The AssetBundle 'xxx' can't be loaded because another AssetBundle with the same files is already loaded.
        //////    ////////To solve this problem,we can get async's req,direct call req.assetBundle it will stop async and we can get ab.
        //////    ////////then we use this ab to invoke async's callback,and sync's callback
        //////    ////////More details can see here:https://answer.uwa4d.com/question/5af3db530e95a527a7a81d31, especially replied by leeman.            
        //////    //////if (asyncLoadingDic.ContainsKey(name))
        //////    //////{
        //////    //////    var asyncMsg = asyncLoadingDic[name];

        //////    //////    CoroutineManager.Instance.RemoveCoroutine(asyncMsg.asyncId);

        //////    //////    Debug.LogError("stop async loading");
        //////    //////    ab = asyncMsg.req.assetBundle;

        //////    //////    objs = ab.LoadAllAssets();
        //////    //////    asyncMsg.callbacks?.Invoke(objs);
        //////    //////    asyncLoadingDic.Remove(name);
        //////    //////}
        //////    ////////<----


        //////    //////if (ab == null)
        //////    //////{
        //////    //////    string nameNew = ABTypeUtil.GetPreFix(abType) + name;
        //////    //////    string path = IOTools.GetABPath(nameNew);
        //////    //////    //Debug.LogError("path111:" + path + ", name:" + getABRealNameFromABPath(path));
        //////    //////    try
        //////    //////    {
        //////    //////        ab = AssetBundle.LoadFromFile(path, 0, (ulong)ConfigUtility.Data.ABResByteOffset);

        //////    //////    }
        //////    //////    catch (System.Exception ex)
        //////    //////    {

        //////    //////        DebugExt.LogE("error while loadAb:abName:" + name + ", path:" + path + ", errorMsg:" + ex.ToString());
        //////    //////    }

        //////    //////    if (abType == ABType.Scene)
        //////    //////    {
        //////    //////        callback?.Invoke(new UnityEngine.Object[] { ab });
        //////    //////        return;
        //////    //////    }


        //////    //////    objs = ab.LoadAllAssets();
        //////    //////}

        //////    //////if (keepAB == false)
        //////    //////{
        //////    //////    ab.Unload(false);
        //////    //////}

        //////    //////callback?.Invoke(objs);

        //////}

        //////public static void LoadAsync(string name, ABType abType, Action<UnityEngine.Object[]> callback)
        //////{

        //////    ABOperator.Instance.Add(new ABRes(name, abType, callback, false, false)); 
        //////    return;
        //////    //////if (asyncLoadingDic.ContainsKey(name))
        //////    //////{
        //////    //////    if (callback != null)
        //////    //////    {
        //////    //////        asyncLoadingDic[name].callbacks += callback;

        //////    //////    }
        //////    //////}
        //////    //////else
        //////    //////{
        //////    //////    AsyncLoadingMsg asyncMsg = new AsyncLoadingMsg();
        //////    //////    asyncLoadingDic[name] = asyncMsg;
        //////    //////    if (callback != null)
        //////    //////    {
        //////    //////        asyncLoadingDic[name].callbacks = callback;
        //////    //////    }
        //////    //////    //Debug.LogError("start async load:" + name + ", time:" + Time.frameCount);
        //////    //////    var id = CoroutineManager.Instance.AddCoroutine(LoadABEnumerator(name, abType, asyncMsg));
        //////    //////    asyncMsg.asyncId = id;
        //////    //////}

        //////} 
        #endregion

        /// <summary>
        /// name不带后缀，需要带前缀
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static IEnumerator LoadABEnumerator(string name, ABType abType, AsyncLoadingMsg asyncMsg)
        {
            string nameNew = ABTypeUtil.GetPreFix(abType) + name;

            string path = IOTools.GetABPath(nameNew);
            //Debug.LogError("path:" + path + ", name:" + getABRealNameFromABPath(path));
            AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path, 0, (ulong)ConfigUtility.Data.ABResByteOffset);
            asyncMsg.req = req;
            yield return req;


            AssetBundle ab = req.assetBundle;
            if (ab != null)
            {
                //specific deal with scene
                if (abType == ABType.Scene)
                {
                    if (asyncLoadingDic.ContainsKey(name))
                    {
                        asyncLoadingDic[name].callbacks.Invoke(new UnityEngine.Object[] { ab });
                        asyncLoadingDic.Remove(name);
                    }
                }
                else
                {

                    AssetBundleRequest abReq = ab.LoadAllAssetsAsync();
                    yield return abReq;

                    ab.Unload(false);


                    var assets = abReq.allAssets;//As for sprite atlas: allAssets[0] is the origin Texture2D，the rest is Sprites.


                    if (asyncLoadingDic.ContainsKey(name))
                    {
                        asyncLoadingDic[name].callbacks.Invoke(assets);
                        asyncLoadingDic.Remove(name);
                    }
                }
            }

            yield return null;
        }

    }
}