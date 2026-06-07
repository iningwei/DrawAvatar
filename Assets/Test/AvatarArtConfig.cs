using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AvatarArtConfig
{
    [Header("轮廓")]
    [Range(0, 1)]
    public float Detail = 0.5f;

    [Range(0, 1)]
    public float Abstraction = 0.55f;

    [Range(0, 1)]
    public float Smoothness = 0.5f;

    [Range(1, 1000)]
    public float MinContourArea = 200;

    [Range(10, 1000)]
    public float MinContourLength = 80;

    [Range(1, 25000)]
    public int MaxContours = 50;

    [Range(1, 20)]
    public int StrokeWidth = 2;

    public Color StrokeColor = Color.black;

    public Color BackgroundColor = Color.white;

    public bool KeepOnlyOuterContours = true;

    [Range(0, 1f)]
    public float MinContourAreaRatio = 0.005f;

    [Header("噪声抑制")]
    [Range(0, 1)]
    public float NoiseSuppression = 0.5f;
}
