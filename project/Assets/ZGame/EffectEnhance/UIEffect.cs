using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class UIEffect : MaskableGraphic
{
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    Mesh mesh = null;
    protected override void OnEnable()
    {

        meshRenderer = this.transform.GetComponent<MeshRenderer>();
        meshFilter = this.transform.GetComponent<MeshFilter>();

        if (Application.isPlaying)
        {
            meshRenderer.enabled = false;
        }

        mesh = new Mesh();
        mesh.MarkDynamic();


        Canvas.willRenderCanvases += UpdateMesh;
    }

    void UpdateMesh()
    {

        mesh = meshFilter.sharedMesh;

        canvasRenderer.SetMesh(mesh);

        Material[] materials = meshRenderer.sharedMaterials;

        if (canvasRenderer.materialCount != materials.Length)
        {
            canvasRenderer.materialCount = materials.Length;
        }

        for (var i = 0; i < materials.Length; ++i)
        {
            canvasRenderer.SetMaterial(materials[i], i);

            canvasRenderer.SetTexture(materials[i].mainTexture);
        }


    }

    protected override void OnDisable()
    {
        Canvas.willRenderCanvases -= UpdateMesh;
        canvasRenderer.Clear();
    }
}


