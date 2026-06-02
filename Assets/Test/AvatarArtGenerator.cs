using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using System.Collections;

public class AvatarArtGenerator : MonoBehaviour
{
    [Header("输入头像")]
    public Texture2D Avatar;

    [Header("显示结果")]
    public RawImage ArtImage;

    [Header("配置")]
    public AvatarArtConfig Config;

    [Header("昵称")]
    public string NickName;

    public AvatarSignature Signature;

    [ContextMenu("Generate")]
    public void Generate()
    {
        if (Avatar == null)
            return;

        //----------------------------------
        // Texture -> Mat
        //----------------------------------

        Mat src = new Mat(
            Avatar.height,
            Avatar.width,
            CvType.CV_8UC4);

        Utils.texture2DToMat(
            Avatar,
            src);

        //----------------------------------
        // 提取轮廓
        //----------------------------------

        List<MatOfPoint> contours =
            AvatarContourExtractor.ExtractContours(
                src,
                Config);

        //// 绘制艺术图 
        //Texture2D result =
        //    DrawArtTexture(
        //        contours,
        //        src.width(),
        //        src.height());
        //// 显示 
        //ArtImage.texture = result;

        //动态绘制
        StartCoroutine(
    DrawArtAnimated(
        contours,
        src.width(),
        src.height()));

        //// 昵称 
        //if (Signature != null)
        //{
        //    Signature.SetName(
        //        NickName);
        //}
    }

    private Texture2D DrawArtTexture(
        List<MatOfPoint> contours,
        int width,
        int height)
    {
        //----------------------------------
        // 创建白底画布
        //----------------------------------

        Scalar bg =
            new Scalar(
                Config.BackgroundColor.b * 255,
                Config.BackgroundColor.g * 255,
                Config.BackgroundColor.r * 255,
                255);

        Mat canvas =
            new Mat(
                height,
                width,
                CvType.CV_8UC4,
                bg);

        //----------------------------------
        // 绘制轮廓
        //----------------------------------

        foreach (var contour in contours)
        {
            MatOfPoint simplified =
                ContourSimplifier.Simplify(
                    contour,
                    Config.Abstraction);

            DrawContour(
                canvas,
                simplified);
        }

        //----------------------------------
        // Mat -> Texture
        //----------------------------------

        Texture2D tex =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false);

        Utils.matToTexture2D(
            canvas,
            tex);

        return tex;
    }

    private void DrawContour(
     Mat canvas,
     MatOfPoint contour)
    {
        //--------------------
        Point[] points = contour.toArray();

        if (points.Length < 4)
            return;

        //----------------------------------
        // Point -> Vector2
        //----------------------------------

        List<Vector2> unityPoints =
            new List<Vector2>();

        foreach (Point p in points)
        {
            unityPoints.Add(
                new Vector2(
                    (float)p.x,
                    (float)p.y));
        }

        //----------------------------------
        // 平滑等级
        //----------------------------------

        int smoothLevel =
            Mathf.RoundToInt(
                Mathf.Lerp(
                    2,
                    12,
                    Config.Smoothness));

        //----------------------------------
        // CatmullRom
        //----------------------------------

        List<Vector2> smoothPoints =
            CatmullRomUtility.SmoothClosed(
                unityPoints,
                smoothLevel);//使用闭合版本

        //----------------------------------
        // Vector2 -> Point
        //----------------------------------

        Point[] drawPoints =
            new Point[smoothPoints.Count];

        for (int i = 0;
             i < smoothPoints.Count;
             i++)
        {
            drawPoints[i] =
                new Point(
                    smoothPoints[i].x,
                    smoothPoints[i].y);
        }

        MatOfPoint smoothContour =
            new MatOfPoint(drawPoints);

        //----------------------------------
        // 绘制
        //----------------------------------

        List<MatOfPoint> drawList =
            new List<MatOfPoint>();

        drawList.Add(
            smoothContour);

        Scalar stroke =
            new Scalar(
                Config.StrokeColor.b * 255,
                Config.StrokeColor.g * 255,
                Config.StrokeColor.r * 255,
                255);

        Imgproc.polylines(
            canvas,
            drawList,
            true,
            stroke,
            Config.StrokeWidth,
            Imgproc.LINE_AA);
    }

    private Texture2D DrawArtTextureAnimated(
    List<MatOfPoint> finishedContours,
    MatOfPoint growingContour,
    float progress,
    int width,
    int height)
    {
        Scalar bg =
            new Scalar(
                Config.BackgroundColor.b * 255,
                Config.BackgroundColor.g * 255,
                Config.BackgroundColor.r * 255,
                255);

        Mat canvas =
            new Mat(
                height,
                width,
                CvType.CV_8UC4,
                bg);

        //---------------------------------
        // 已完成轮廓
        //---------------------------------

        foreach (var contour in finishedContours)
        {
            DrawContour(
                canvas,
                contour);
        }

        //---------------------------------
        // 当前生长轮廓
        //---------------------------------

        DrawPartialContour(
            canvas,
            growingContour,
            progress);

        //---------------------------------
        // 转Texture
        //---------------------------------

        Texture2D tex =
            new Texture2D(
                width,
                height,
                TextureFormat.RGBA32,
                false);

        Utils.matToTexture2D(
            canvas,
            tex);

        return tex;
    }

    private void DrawPartialContour(
    Mat canvas,
    MatOfPoint contour,
    float progress)
    {
        //---------------------------------
        // 简化
        //---------------------------------

        contour =
            ContourSimplifier.Simplify(
                contour,
                Config.Abstraction);

        Point[] points =
            contour.toArray();

        if (points.Length < 2)
            return;

        //---------------------------------
        // 平滑
        //---------------------------------

        List<Vector2> unityPoints =
            new List<Vector2>();

        foreach (Point p in points)
        {
            unityPoints.Add(
                new Vector2(
                    (float)p.x,
                    (float)p.y));
        }

        int smoothLevel =
            Mathf.RoundToInt(
                Mathf.Lerp(
                    2,
                    12,
                    Config.Smoothness));

        if (unityPoints.Count >= 4)
        {
            unityPoints =
                CatmullRomUtility.SmoothClosed(
                    unityPoints,
                    smoothLevel);
        }

        //---------------------------------
        // 生长进度
        //---------------------------------

        int visibleCount =
            Mathf.Clamp(
                Mathf.RoundToInt(
                    unityPoints.Count *
                    progress),
                2,
                unityPoints.Count);

        Point[] drawPoints =
            new Point[visibleCount];

        for (int i = 0;
             i < visibleCount;
             i++)
        {
            drawPoints[i] =
                new Point(
                    unityPoints[i].x,
                    unityPoints[i].y);
        }

        MatOfPoint partial =
            new MatOfPoint(
                drawPoints);

        List<MatOfPoint> drawList =
            new List<MatOfPoint>();

        drawList.Add(
            partial);

        Scalar stroke =
            new Scalar(
                Config.StrokeColor.b * 255,
                Config.StrokeColor.g * 255,
                Config.StrokeColor.r * 255,
                255);

        Imgproc.polylines(
            canvas,
            drawList,
            false,//不闭合.才会有：一点一点画出来的效果。
            stroke,
            Config.StrokeWidth,
            Imgproc.LINE_AA);
    }

    //动画协程
    public IEnumerator DrawArtAnimated(
    List<MatOfPoint> contours,
    int width,
    int height)
    {
        //---------------------------------
        // 大轮廓优先
        //---------------------------------

        contours.Sort((a, b) =>
        {
            return Imgproc.contourArea(b)
                .CompareTo(
                    Imgproc.contourArea(a));
        });

        List<MatOfPoint> finished =
            new List<MatOfPoint>();

        foreach (var contour in contours)
        {
            //---------------------------------
            // 按轮廓长度决定动画时间
            //---------------------------------

            double length =
                Imgproc.arcLength(
                    new MatOfPoint2f(
                        contour.toArray()),
                    true);

            float duration =
                Mathf.Lerp(
                    0.15f,
                    1.0f,
                    Mathf.Clamp01(
                        (float)length / 1000f));

            float timer = 0;

            while (timer < duration)
            {
                timer += Time.deltaTime;

                float progress =
                    Mathf.Clamp01(
                        timer /
                        duration);

                Texture2D tex =
                    DrawArtTextureAnimated(
                        finished,
                        contour,
                        progress,
                        width,
                        height);

                ArtImage.texture =
                    tex;

                yield return null;
            }

            finished.Add(
                contour);
        }

        //---------------------------------
        // 最终图
        //---------------------------------

        Texture2D final =
            DrawArtTexture(
                contours,
                width,
                height);

        ArtImage.texture =
            final;

        //---------------------------------
        // 签名淡入
        //---------------------------------

        if (Signature != null)
        {
            Signature.SetName(
                NickName);
        }
    }
}