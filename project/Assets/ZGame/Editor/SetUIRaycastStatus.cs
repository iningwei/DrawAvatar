using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SetUIRaycastStatus : Editor
{

    [MenuItem("GameObject/UIRaycast/node and childs all open")]
    static void SetUIRaycastOpen()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var rectTrans = obj.GetComponentsInChildren<RectTransform>(true);
        setStatus(rectTrans, true);
        Debug.Log("finish SetUIRaycast open");
        EditorUtility.SetDirty(obj);
    }

    static void setStatus(Transform[] rectTrans, bool status)
    {
        for (int i = 0; i < rectTrans.Length; i++)
        {
            var rectTran = rectTrans[i];
            //text
            var text = rectTran.GetComponent<Text>();
            if (text)
            {
                text.raycastTarget = status;
            }
            //TextMeshProUGUI
            var tmp_ugui = rectTran.GetComponent<TextMeshProUGUI>();
            if (tmp_ugui)
            {
                tmp_ugui.raycastTarget = status;
            }

            var tmp = rectTran.GetComponent<TextMeshPro>();
            if (tmp)
            {
                tmp.raycastTarget = status;
            }

            //image
            var image = rectTran.GetComponent<Image>();
            if (image)
            {
                image.raycastTarget = status;
            }
            //rawImage
            var rawImage = rectTran.GetComponent<RawImage>();
            if (rawImage)
            {
                rawImage.raycastTarget = status;
            }
            //RoundRawImage
            var rri = rectTran.GetComponent<RoundedRawImage>();
            if (rri)
            {
                rri.raycastTarget = status;
            }
        }
    }

    [MenuItem("GameObject/UIRaycast/node and childs all close")]
    static void SetUIRaycastClose()
    {
        GameObject obj = Selection.activeObject as GameObject;
        var rectTrans = obj.GetComponentsInChildren<RectTransform>(true);
        setStatus(rectTrans, false);
        Debug.Log("finish SetUIRaycast close");
        EditorUtility.SetDirty(obj);
    }
}
