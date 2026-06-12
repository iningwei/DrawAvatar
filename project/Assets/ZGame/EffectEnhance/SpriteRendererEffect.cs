using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteRendererEffect : MaskableGraphic
{
    MeshRenderer meshRenderer;


    GameObject meshObj;
    MeshFilter meshFilter;
    Mesh mesh = null;
    protected override void OnEnable()
    {

        SpriteRenderer spriteRenderer = this.transform.GetComponent<SpriteRenderer>();
        GenerateMeshObjFromSprite(spriteRenderer);

        meshFilter = meshObj.transform.GetComponent<MeshFilter>();
        meshRenderer = meshObj.transform.GetComponent<MeshRenderer>();

        //if (Application.isPlaying)
        {
            spriteRenderer.enabled = false;
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

            canvasRenderer.SetTexture(materials[i].mainTexture);
        }


    }

    protected override void OnDisable()
    {
        Canvas.willRenderCanvases -= UpdateMesh;
        canvasRenderer.Clear();
    }



    void GenerateMeshObjFromSprite(SpriteRenderer _spriteRenderer)
    {
        //sprite的三角形数组
        ushort[] triangles0 = _spriteRenderer.sprite.triangles;
        //mesh的三角形数组
        int[] meshTriangles = new int[triangles0.Length];
        //mesh的三角形顶点
        Vector3[] vertices0 = new Vector3[_spriteRenderer.sprite.vertices.Length];
        //存储顶点数组
        for (int i = 0; i < _spriteRenderer.sprite.vertices.Length; i++)
        {
            vertices0[i] = new Vector3(_spriteRenderer.sprite.vertices[i].x, _spriteRenderer.sprite.vertices[i].y, 0);
        }
        //存储三角形数组
        for (int i = 0; i < meshTriangles.Length; i += 3)
        {
            meshTriangles[i] = triangles0[i];
            meshTriangles[i + 1] = triangles0[i + 1];
            meshTriangles[i + 2] = triangles0[i + 2];
            //////yield return new WaitForSeconds(0.02f);
            //////Debug.DrawLine(vertices0[meshTriangles[i]], vertices0[meshTriangles[i + 1]], Color.red, 100.0f);
            //////Debug.DrawLine(vertices0[meshTriangles[i + 1]], vertices0[meshTriangles[i + 2]], Color.red, 100.0f);
            //////Debug.DrawLine(vertices0[meshTriangles[i + 2]], vertices0[meshTriangles[i]], Color.red, 100.0f);
        }
        //存储sprite的texture
        Texture2D texture = _spriteRenderer.sprite.texture;

        if (meshObj != null)
        {
            DestroyImmediate(meshObj);
        }
        //生成一个新的物体挂载mesh组件
        meshObj = new GameObject();
        MeshFilter meshFilter = meshObj.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = meshObj.AddComponent<MeshRenderer>();
        Mesh _mesh = new Mesh();
        meshFilter.mesh = _mesh;

        //给mesh赋值
        _mesh.vertices = vertices0;
        _mesh.uv = _spriteRenderer.sprite.uv;
        _mesh.triangles = meshTriangles;

        //设置物体位置，名字等
        meshObj.name = "MeshSprite";
        //meshObj.transform.position = gameObject.transform.position;
        //meshObj.transform.localScale = gameObject.transform.localScale;
        //meshObj.transform.rotation = gameObject.transform.rotation;
        meshObj.transform.parent = gameObject.transform.parent;
        meshObj.transform.localPosition = gameObject.transform.localPosition;


        //设置mesh的属性
        meshRenderer.sharedMaterial = _spriteRenderer.sharedMaterial;
        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.sharedMaterial.color = _spriteRenderer.color;
        meshRenderer.sortingLayerName = _spriteRenderer.sortingLayerName;
        meshRenderer.sortingLayerID = _spriteRenderer.sortingLayerID;
        meshRenderer.sortingOrder = _spriteRenderer.sortingOrder;
    }

    protected override void OnDestroy()
    {
        if (meshObj != null)
        {
            GameObject.DestroyImmediate(meshObj);
        }
    }
}

