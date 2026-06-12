using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

public static class ContourSimplifier
{
    public static MatOfPoint Simplify(
        MatOfPoint contour,
        float abstraction)
    {
        MatOfPoint2f src =
            new MatOfPoint2f(
                contour.toArray());
         
        double perimeter =
            Imgproc.arcLength(
                src,
                true);

        double epsilon =
            perimeter *
            Mathf.Lerp(
                0.001f,
                0.08f,
                abstraction);

        MatOfPoint2f dst =
            new MatOfPoint2f();

        Imgproc.approxPolyDP(
            src,
            dst,
            epsilon,
            true);

        return new MatOfPoint(
            dst.toArray());
    }
}