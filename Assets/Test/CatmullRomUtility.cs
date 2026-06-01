using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public static class CatmullRomUtility
{
    public static List<Vector2> Smooth(
        List<Vector2> points,
        int subdivisions)
    {
        List<Vector2> result =
            new List<Vector2>();

        if (points.Count < 4)
            return points;

        for (int i = 1; i < points.Count - 2; i++)
        {
            Vector2 p0 = points[i - 1];
            Vector2 p1 = points[i];
            Vector2 p2 = points[i + 1];
            Vector2 p3 = points[i + 2];

            for (int j = 0;
                 j < subdivisions;
                 j++)
            {
                float t =
                    j / (float)subdivisions;

                result.Add(
                    GetPoint(
                        p0,
                        p1,
                        p2,
                        p3,
                        t));
            }
        }

        return result;
    }

    //Æ½»¬ÇÒ±ÕºÏ
    public static List<Vector2> SmoothClosed(
    List<Vector2> points,
    int subdivisions)
    {
        List<Vector2> result =
            new List<Vector2>();

        int count =
            points.Count;

        for (int i = 0;
             i < count;
             i++)
        {
            Vector2 p0 =
                points[(i - 1 + count) % count];

            Vector2 p1 =
                points[i];

            Vector2 p2 =
                points[(i + 1) % count];

            Vector2 p3 =
                points[(i + 2) % count];

            for (int j = 0;
                 j < subdivisions;
                 j++)
            {
                float t =
                    j / (float)subdivisions;

                result.Add(
                    GetPoint(
                        p0,
                        p1,
                        p2,
                        p3,
                        t));
            }
        }

        return result;
    }

    static Vector2 GetPoint(
        Vector2 p0,
        Vector2 p1,
        Vector2 p2,
        Vector2 p3,
        float t)
    {
        return 0.5f *
        (
            (2f * p1) +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t * t +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t
        );
    }
}