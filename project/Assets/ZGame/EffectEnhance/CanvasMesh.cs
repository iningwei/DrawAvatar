//https://gist.github.com/YclepticStudios/f2313ab08d2c81a31c94d5ed6b1e6eed

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CanvasMesh : Graphic
{
    public MeshFilter meshFiler;
    // Inspector properties
    Mesh mesh = null;

    protected override void Awake()
    {
        mesh = meshFiler.sharedMesh;
    }
    /// <summary>
    /// Callback function when a UI element needs to generate vertices.
    /// </summary>
    /// <param name="vh">VertexHelper utility.</param>
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        if (mesh == null) return;
        // Get data from mesh
        Vector3[] verts = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        if (uvs.Length < verts.Length)
            uvs = new Vector2[verts.Length];
        // Get mesh bounds parameters
        Vector2 meshMin = mesh.bounds.min;
        Vector2 meshSize = mesh.bounds.size;
        // Add scaled vertices
        for (int ii = 0; ii < verts.Length; ii++)
        {
            Vector2 v = verts[ii];
            v.x = (v.x - meshMin.x) / meshSize.x;
            v.y = (v.y - meshMin.y) / meshSize.y;
            v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
            vh.AddVert(v, color, uvs[ii]);
        }
        // Add triangles
        int[] tris = mesh.triangles;
        for (int ii = 0; ii < tris.Length; ii += 3)
            vh.AddTriangle(tris[ii], tris[ii + 1], tris[ii + 2]);
    }

    /// <summary>
    /// Converts a vertex in mesh coordinates to a point in world coordinates.
    /// </summary>
    /// <param name="vertex">The input vertex.</param>
    /// <returns>A point in world coordinates.</returns>
    public Vector3 TransformVertex(Vector3 vertex)
    {
        // Convert vertex into local coordinates
        Vector2 v;
        v.x = (vertex.x - mesh.bounds.min.x) / mesh.bounds.size.x;
        v.y = (vertex.y - mesh.bounds.min.y) / mesh.bounds.size.y;
        v = Vector2.Scale(v - rectTransform.pivot, rectTransform.rect.size);
        // Convert from local into world
        return transform.TransformPoint(v);
    }

    /// <summary>
    /// Converts a vertex in world coordinates into a vertex in mesh coordinates.
    /// </summary>
    /// <param name="vertex">The input vertex.</param>
    /// <returns>A point in mesh coordinates.</returns>
    public Vector3 InverseTransformVertex(Vector3 vertex)
    {
        // Convert from world into local coordinates
        Vector2 v = transform.InverseTransformPoint(vertex);
        // Convert into mesh coordinates
        v.x /= rectTransform.rect.size.x;
        v.y /= rectTransform.rect.size.y;
        v += rectTransform.pivot;
        v = Vector2.Scale(v, mesh.bounds.size);
        v.x += mesh.bounds.min.x;
        v.y += mesh.bounds.min.y;
        return v;
    }
}
