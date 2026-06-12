using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NodeTypeObjPair
{
    public string typeName;
    public GameObject obj;
}
public class HolderTag : MonoBehaviour
{
    //node节点类型名称和Obj对应KV对
    public List<NodeTypeObjPair> nodeTypeObjPairs;

}
