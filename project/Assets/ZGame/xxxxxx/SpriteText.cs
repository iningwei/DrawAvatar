using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//供textmesh使用，使其渲染在sprite上层
public class SpriteText : MonoBehaviour
{
    void Start()
    {
        var parent = transform.parent;
        var parentRenderer = parent.GetComponent<Renderer>();
        var renderer = GetComponent<Renderer>();
        renderer.sortingLayerID = parentRenderer.sortingLayerID;
        renderer.sortingOrder = parentRenderer.sortingOrder;
    }
}
