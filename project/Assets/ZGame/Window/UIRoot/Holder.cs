using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress;
using ZGame.Window;

namespace ZGame.Window
{
    public class Holder : UIRoot
    {
        public List<Node> nodes;
        public HolderNodeObjPool nodeObjPool;

#if UNITY_EDITOR
        List<Type> supportedTypes = new List<Type>();
#endif
        public Holder(GameObject holderObj, UIRoot parentUIRoot, List<KeyValuePair<Type, GameObject>> nodeTypeObjKVList, params object[] paras) : base(holderObj, parentUIRoot, paras)
        {
            nodes = new List<Node>();
            nodeObjPool = new HolderNodeObjPool(this.obj.transform, nodeTypeObjKVList);
#if UNITY_EDITOR
            for (int i = 0; i < nodeTypeObjKVList.Count; i++)
            {
                this.supportedTypes.Add(nodeTypeObjKVList[i].Key);
            }
#endif
            this.FillItems();
        }


        public virtual void FillItems()
        {

        }

        public virtual T AddNode<T>(params object[] paras) where T : Node
        {
#if UNITY_EDITOR
            //编辑器下检测T是否在，避免传T类型不一致
            if (!this.supportedTypes.Contains(typeof(T)))
            {
                Debug.LogError("error, type  " + typeof(T).Name + " not support in holder:" + this.GetType().Name);
                return default;
            }
#endif 
            string typeName = typeof(T).Name;
            GameObject obj = nodeObjPool.Get(typeName);
            obj.transform.SetParent(this.obj.transform);
            obj.transform.ResetLocalPRS();
            T node = (T)Activator.CreateInstance(typeof(T), obj, this, paras);
            node.Show();
            this.nodes.Add(node);
            node.onDestroy += this.removeNode;
            return node;
        }

        private void removeNode(Node node)
        {
            nodeObjPool.Recycle(node.obj);//node的obj回收，不销毁
            node.onDestroy -= this.removeNode;
            if (nodes.Contains(node))
            {
                nodes.Remove(node);
            }
        }

        public void ClearNodes()
        {
            if (nodes.Count > 0)
            {
                for (int i = nodes.Count - 1; i >= 0; i--)
                {
                    var node = nodes[i];
                    node.Destroy();
                    node.onDestroy -= this.removeNode;
                }
            }
            nodes.Clear();
        }


        public override void Destroy()
        {
            base.Destroy();
            this.ClearNodes();
            nodeObjPool.Clear();
        }
    }
}