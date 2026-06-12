using OpenCVForUnity.CoreModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimatedContour
{
    public MatOfPoint OriginalContour;

    public List<Vector2> SmoothPoints;

    public float Length;
}