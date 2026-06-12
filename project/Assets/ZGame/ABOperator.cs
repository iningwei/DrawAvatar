using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame
{
    public enum AsyncStatus
    {
        Ready,
        Processing,
        Done,
    }

    [Serializable]
    public class ABRes
    {
        public string name;
        public ABType abType;
        public bool isSync = true;
        public bool keepAB = false;


        public AsyncStatus status;//当isAsyncLoad为true，status才有意义
        public AssetBundleCreateRequest request;

        public Action<UnityEngine.Object[]> callback;

        string resPath = "";
        public string GetResPath()
        {
            return this.resPath;
        }


        public ABRes(string name, ABType abType, Action<UnityEngine.Object[]> callback, bool isSync = true, bool keepAB = false)
        {
            this.name = name;
            this.abType = abType;
            this.callback = callback;
            this.keepAB = keepAB;
            this.isSync = isSync;



            string nameNew = ABTypeUtil.GetPreFix(abType) + name;
            resPath = IOTools.GetABPath(nameNew);
        }

        public void AsyncLoad()
        {
            request = AssetBundle.LoadFromFileAsync(resPath, 0, (ulong)ConfigUtility.Data.ABResByteOffset);
        }

        public void Load()
        {
            try
            {
                var ab = AssetBundle.LoadFromFile(resPath, 0, (ulong)ConfigUtility.Data.ABResByteOffset);
                if (ab != null)
                {
                    if (abType == ABType.Scene)
                    {
                        callback?.Invoke(new UnityEngine.Object[] { ab });
                        return;
                    }

                    var objs = ab.LoadAllAssets();
                    callback?.Invoke(objs);

                    if (keepAB == false)
                    {
                        ab.Unload(false);
                    }
                }
                else
                {
                    Debug.LogError("ab is null, path:" + resPath);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("load ab:" + resPath + " error, ex:" + ex.ToString());
            }

        }

        public bool IsAsyncDone()
        {
            if (request != null)
            {
                if (request.isDone)
                {
                    return true;
                }
            }

            return false;
        }

        public void HandleAsyncCallback()
        {
            AssetBundle ab = request.assetBundle;
            if (ab != null)
            {
                if (abType == ABType.Scene)
                {
                    callback?.Invoke(new UnityEngine.Object[] { ab });
                    return;
                }

                var objs = ab.LoadAllAssets();//为了省事，没用 ab.LoadAllAssetsAsync......//TODO:持续优化
                callback?.Invoke(objs);
                if (keepAB == false)
                {
                    ab.Unload(false);
                }
            }
        }
    }


    public class ABOperator : SingletonMonoBehaviour<ABOperator>
    {
        public List<ABRes> asyncReadyList = new List<ABRes>();

        public List<ABRes> asyncProcessList = new List<ABRes>();


        int asyncProcessLimit = 5;

        public void Add(ABRes res)
        {
            if (res.isSync)
            {
                var sameAysncRes = removeABResFromAsyncList(res.GetResPath());
                AssetBundle ab = null;
                UnityEngine.Object[] objs = null;
                if (sameAysncRes != null)
                {
                    if (sameAysncRes.status == AsyncStatus.Ready)
                    {
                        if (sameAysncRes.callback != null)
                        {
                            res.callback += sameAysncRes.callback;
                        }
                    }
                    else if (sameAysncRes.status == AsyncStatus.Processing)
                    {
                        ab = sameAysncRes.request.assetBundle;
                        if (sameAysncRes.callback != null && ab != null)
                        {
                            objs = ab.LoadAllAssets();
                            sameAysncRes.callback?.Invoke(objs);
                        }
                    }
                }

                if (ab == null)
                {
                    res.Load();
                }
                else
                {
                    if (res.keepAB == false)
                    {
                        ab.Unload(false);
                    }

                    res.callback?.Invoke(objs);
                }
            }
            else
            {
                for (int i = 0; i < asyncProcessList.Count; i++)
                {
                    var processItem = asyncProcessList[i];
                    if (processItem.GetResPath() == res.GetResPath())
                    {
                        if (res.callback != null)
                        {
                            processItem.callback += res.callback;
                        }
                        return;
                    }
                }
                for (int i = 0; i < asyncReadyList.Count; i++)
                {
                    var readyItem = asyncReadyList[i];
                    if (readyItem.GetResPath() == res.GetResPath())
                    {
                        if (res.callback != null)
                        {
                            readyItem.callback += res.callback;
                        }
                        return;
                    }
                }

                asyncReadyList.Add(res);
            }
        }




        ABRes removeABResFromAsyncList(string resPath)
        {
            for (int i = 0; i < asyncProcessList.Count; i++)
            {
                var processItem = asyncProcessList[i];
                if (processItem.GetResPath() == resPath)
                {
                    asyncProcessList.Remove(processItem);
                    return processItem;
                }
            }

            for (int i = 0; i < asyncReadyList.Count; i++)
            {
                var readyItem = asyncReadyList[i];
                if (readyItem.GetResPath() == resPath)
                {
                    asyncReadyList.Remove(readyItem);
                    return readyItem;
                }
            }
            return null;

        }


        void process()
        {

            if (asyncProcessList.Count < asyncProcessLimit)
            {
                int gap = asyncProcessLimit - asyncProcessList.Count;
                if (gap < 1)
                {
                    return;
                }
                for (int i = 0; i < gap; i++)
                {
                    if (asyncReadyList.Count > 0)
                    {
                        var res = asyncReadyList[0];
                        if (res != null)
                        {
                            res.status = AsyncStatus.Processing;
                            res.AsyncLoad();
                            asyncReadyList.RemoveAt(0);
                            asyncProcessList.Add(res);
                        }

                    }
                }
            }
        }
        void check()
        {
            var count = asyncProcessList.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                var res = asyncProcessList[i];

                if (res.IsAsyncDone() && res.status != AsyncStatus.Done)
                {
                    res.HandleAsyncCallback();
                    res.status = AsyncStatus.Done;
                    asyncProcessList.Remove(res);
                }

            }
        }

        void Update()
        {
            process();
            check();
        }
    }
}