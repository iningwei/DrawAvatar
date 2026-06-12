using System;
using System.Collections;
using System.Collections.Generic; 
using UnityEngine;

namespace ZGame.Window
{

    //Holder的Node GameObject池

    //自动在Holder的同层级下创建 HolderNodeObjPool 节点
    public class HolderNodeObjPool
    {
        Dictionary<string, Queue<GameObject>> nodeObjPoolDic = new Dictionary<string, Queue<GameObject>>();


        Dictionary<string, GameObject> prototypeObjDic = new Dictionary<string, GameObject>();

        Transform poolTran;
        public HolderNodeObjPool(Transform holderTran, List<KeyValuePair<Type, GameObject>> nodeTypeObjKVList)
        {
            GameObject obj = new GameObject();
            obj.name = holderTran.name + "_NodeObjPool";
            obj.transform.parent = holderTran.parent;
            poolTran = obj.transform;

            for (int i = 0; i < nodeTypeObjKVList.Count; i++)
            {
                var kv = nodeTypeObjKVList[i];
                kv.Value.SetActive(false);//隐藏传过来的原型obj
                this.prototypeObjDic.Add(kv.Key.Name, kv.Value);
            }
        }

        GameObject getObj(string prototypeName)
        {
            if (nodeObjPoolDic.ContainsKey(prototypeName) && nodeObjPoolDic[prototypeName].Count > 0)
            {
                return nodeObjPoolDic[prototypeName].Dequeue();
            }
            //Debug.LogError("nodeObjPool无" + prototypeName + ", so Instantiate one");
            GameObject targetObj = GameObject.Instantiate(prototypeObjDic[prototypeName]);
            targetObj.name = prototypeName;
            return targetObj;
        }
        public GameObject Get(string prototypeName)
        {
            return this.getObj(prototypeName);
        }

        public void Recycle(GameObject obj)
        {
            string name = obj.name;
            if (!nodeObjPoolDic.ContainsKey(name))
            {
                Queue<GameObject> queue = new Queue<GameObject>();
                nodeObjPoolDic.Add(name, queue);
            }

            obj.SetActive(false);
            if (obj.transform is RectTransform)
            {
                obj.transform.SetParent(poolTran);
            }
            else
            {
                obj.transform.parent = poolTran;
            }
#if UNITY_EDITOR
            var xxx = new List<GameObject>(nodeObjPoolDic[name].ToArray());
            for (int i = 0; i < xxx.Count; i++)
            {
                if (xxx[i].Equals(obj))
                {
                    Debug.LogError("nodeObjPoolDic:" + name + "已经存在相同的obj:" + obj.GetInstanceID());
                }
            }
#endif
            nodeObjPoolDic[name].Enqueue(obj);
        }

        public void Clear()
        {
            foreach (var item in nodeObjPoolDic)
            {
                var count = item.Value;
                while (item.Value.Count > 0)
                {
                    var obj = item.Value.Dequeue();
                    GameObject.DestroyImmediate(obj);
                }
                item.Value.Clear();
            }
            nodeObjPoolDic.Clear();
        }
    }
}