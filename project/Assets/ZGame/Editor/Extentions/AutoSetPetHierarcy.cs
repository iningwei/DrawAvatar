using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

//自动设置宠物模型资源的结构树
public class AutoSetPetHierarcy
{
    [MenuItem("GameObject/ZGame/CrazyPets幻兽项目/设置宠物模型结构树")]
    static void SetModel()
    {
        var selectObj = Selection.activeGameObject;
        setHierarchy(selectObj);
    }
     

    private static void setHierarchy(GameObject selectObj)
    {
        Vector3 initPos = selectObj.transform.position;
        //concrete节点
        GameObject concrete = new GameObject("concrete");
        concrete.transform.position = initPos;
        //根节点
        GameObject root = new GameObject(selectObj.name);
        root.transform.position = initPos;
        //f0节点
        GameObject f0 = new GameObject("f0");
        f0.transform.position = initPos;
        //hudAnchorTran
        GameObject hudAnhor = new GameObject("hudAnchorTran");
        hudAnhor.transform.position = initPos;
        //bodyCollider
        GameObject bodyColliderObj = new GameObject("bodyCollider");
        bodyColliderObj.transform.position = initPos;
        var cc = bodyColliderObj.GetOrAddComponent<CapsuleCollider>();
        cc.radius = 0.2f;
        cc.height = 0.5f;

        //设置层级关系
        selectObj.transform.parent = concrete.transform;
        f0.transform.parent = concrete.transform;
        hudAnhor.transform.parent = concrete.transform;
        bodyColliderObj.transform.parent = concrete.transform;
        concrete.transform.parent = root.transform;

        bodyColliderObj.transform.localPosition = new Vector3(0, cc.height * 0.5f, 0f);
    }
}
