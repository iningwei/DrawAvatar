using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Event;
using ZGame.Ress.AB;

namespace ZGame.Ress
{
    //////public class ResLoadType
    //////{
    //////    public const int AssetBundle = 0;
    //////    public const int Resources = 1;
    //////    public const int Addressable = 2;
    //////}

    [Serializable]
    public class Res
    {
        public string resName;
        public UnityEngine.Object resObj;

        public Res(string name, UnityEngine.Object resObj)
        {
            this.resName = name;
            this.resObj = resObj;
        }



        public void SetResObj(UnityEngine.Object resObj)
        {
            this.resObj = resObj;
        }
        public virtual T GetResAsset<T>()
        {
            return default;
        }

        public virtual T GetResAsset<T>(string name)
        {
            if (!(typeof(T).Equals(typeof(Sprite))))
            {
                Debug.LogError("Only SpriteRes used this method,please change method without name param");
            }
            return default;
        }

        public SerializedDictionary<int, Transform> refTrsDic = new SerializedDictionary<int, Transform>();//key为Transform的ID

        [SerializeField, SetProperty("refTrsCount")]
        int _refTrsCount;

        public int refTrsCount
        {
            get
            {
                return _refTrsCount;
            }
            set
            {
                _refTrsCount = value;
            }
        }


        public void AddRefTrs(Transform refTrs)
        {
            var id = refTrs.GetInstanceID();
            //////if (this.refTrsDic.ContainsKey(id))
            //////{
            //////    Debug.LogError(this.resName + " already contain ref tran:" + refTrs.GetHierarchy());
            //////}
            refTrsDic[id] = refTrs;
            refTrsCount = this.refTrsDic.Count;
        }

        public void RemoveRefTrs(Transform refTrs)
        {
            var id = refTrs.GetInstanceID();
            if (this.refTrsDic.ContainsKey(id))
            {
                refTrsDic.Remove(id);

                refTrsCount = this.refTrsDic.Count;
                if (refTrsCount == 0)
                {
                    //TODO:后续引入驻留机制
                    this.Destroy();
                }
            }
        }

        public bool ContainRefTrs(Transform refTrs)
        {
            var f = this.refTrsDic.ContainsKey(refTrs.GetInstanceID());
            return f;
        }

        public virtual void Destroy()
        {
            if (resObj != null)
            {
                GameObject.DestroyImmediate(resObj, true);
            }
        }
    }
}
