using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

public static class AvatarContourExtractor
{
    public static List<MatOfPoint> ExtractContours(
        Mat src,
        AvatarArtConfig config)
    {
        Mat gray = new Mat();

        Imgproc.cvtColor(
            src,
            gray,
            Imgproc.COLOR_RGBA2GRAY);

        //--------------------------------
        // 风格化降噪
        //--------------------------------

        int blur =
            Mathf.RoundToInt(
                Mathf.Lerp(1, 15,
                config.Abstraction));

        Imgproc.bilateralFilter(
            gray,
            gray,
            blur,
            75,
            75);

        //--------------------------------
        // 边缘检测
        //--------------------------------

        Mat edge = new Mat();

        double threshold =
            Mathf.Lerp(
                20,
                120,
                1f - config.Detail);

        Imgproc.Canny(
            gray,
            edge,
            threshold,
            threshold * 2);

        //--------------------------------
        // 查找轮廓
        //--------------------------------

        List<MatOfPoint> contours =
            new List<MatOfPoint>();

        Imgproc.findContours(
            edge,
            contours,
            new Mat(),
            Imgproc.RETR_LIST,
            Imgproc.CHAIN_APPROX_NONE);

        //--------------------------------
        // 过滤小轮廓
        //--------------------------------

        contours.RemoveAll(contour =>
        {
            return Imgproc.contourArea(contour)
                < config.MinContourArea;
        });

        //--------------------------------
        // 按面积排序
        //--------------------------------

        contours.Sort((a, b) =>
        {
            return Imgproc.contourArea(b)
                .CompareTo(
                Imgproc.contourArea(a));
        });

        if (contours.Count >
            config.MaxContours)
        {
            contours.RemoveRange(
                config.MaxContours,
                contours.Count -
                config.MaxContours);
        }

        return contours;
    }
}