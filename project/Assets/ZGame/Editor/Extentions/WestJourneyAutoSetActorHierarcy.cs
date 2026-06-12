using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WestJourneyAutoSetActorHierarcy : MonoBehaviour
{
    [MenuItem("GameObject/ZGame/WestJourney西游项目/设置模型锚点")]
    static void SetModel()
    {
        var selectObj = Selection.activeGameObject;

        // attackPoint_0 节点
        GameObject attackPoint_0 = new GameObject("attackPoint_0");
        attackPoint_0.transform.parent = selectObj.transform;
        attackPoint_0.transform.localPosition = Vector3.zero;
        //hitPoint_0 节点
        GameObject hitPoint_0 = new GameObject("hitPoint_0");
        hitPoint_0.transform.parent = selectObj.transform;
        hitPoint_0.transform.localPosition = Vector3.zero;
        //hudPoint 节点
        GameObject hudPoint = new GameObject("hudPoint");
        hudPoint.transform.parent = selectObj.transform;
        hudPoint.transform.localPosition = Vector3.zero;
    }
}
