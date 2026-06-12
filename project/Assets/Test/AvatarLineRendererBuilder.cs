using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public static class AvatarLineRendererBuilder
{
    public static LineRenderer CreateLine(
        Transform parent,
        List<Vector2> points,
        AvatarArtConfig config)
    {
        GameObject go =
            new GameObject("Contour");

        go.transform.SetParent(parent);

        LineRenderer lr =
            go.AddComponent<LineRenderer>();

        lr.useWorldSpace = false;

        lr.startWidth =
            config.StrokeWidth;

        lr.endWidth =
            config.StrokeWidth;

        lr.positionCount =
            points.Count;

        lr.material =
            new Material(
                Shader.Find(
                    "Sprites/Default"));

        lr.startColor =
            config.StrokeColor;

        lr.endColor =
            config.StrokeColor;

        for (int i = 0;
             i < points.Count;
             i++)
        {
            lr.SetPosition(
                i,
                points[i]);
        }

        return lr;
    }
}