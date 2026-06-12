using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GetObjMatNames
{
    [MenuItem("GameObject/获得材质球名称")]
    static void GetMatNames()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj)
        {
            var render = obj.GetComponent<Renderer>();
            if (render)
            {
                var mats = render.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    Debug.Log("render-->mat " + i + ":" + mats[i].name);
                }
            }

            var image = obj.GetComponent<Image>();
            if (image)
            {
                if (image.material)
                {
                    Debug.Log("image->mat:" + image.material.name);
                }
                else
                {
                    Debug.LogError("image has no mat attached");
                }
            }


            var rawImage = obj.GetComponent<RawImage>();
            if (rawImage)
            {
                if (rawImage.material)
                {
                    Debug.Log("rawImage->mat:" + rawImage.material.name);
                }
                else
                {
                    Debug.LogError("rawImage has no mat attached");
                }
            }
        }
    }
}
