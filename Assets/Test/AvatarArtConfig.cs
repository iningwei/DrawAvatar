using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvatarArtConfig
{
    [Header("ÂÖÀª")]
    [Range(0, 1)]
    public float Detail = 0.7f;

    [Range(0, 1)]
    public float Abstraction = 0.4f;

    [Range(0, 1)]
    public float Smoothness = 0.6f;

    [Range(1, 1000)]
    public float MinContourArea = 100;

    [Range(1, 25000)]
    public int MaxContours = 30;

    [Range(1, 20)]
    public int StrokeWidth = 3;

    public Color StrokeColor = Color.black;

    public Color BackgroundColor = Color.white;
}
