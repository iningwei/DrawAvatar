using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using OpenCVForUnity.CoreModule;
using OpenCVForUnity.UnityUtils;
using OpenCVForUnity.ImgprocModule;
using System.Collections;
using OpenCVForUnity.UnityIntegration;

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
    [Header("笔")]
    public Image PenImg;

    public AvatarSignature Signature;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            this.GenerateWithAnim();
        }
    }

    public void GenerateWithAnim()
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

        OpenCVMatUtils.Texture2DToMat(
            Avatar,
            src);

        //----------------------------------
        // 提取轮廓
        //----------------------------------

        List<MatOfPoint> contours =
            AvatarContourExtractor.ExtractContours(
                src,
                Config);

        //动态绘制
        StartCoroutine(
    DrawArtAnimated(
        contours,
        src.width(),
        src.height()));
    }

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

        OpenCVMatUtils.Texture2DToMat(
            Avatar,
            src);

        //----------------------------------
        // 提取轮廓
        //----------------------------------

        List<MatOfPoint> contours =
            AvatarContourExtractor.ExtractContours(
                src,
                Config);

        // 绘制艺术图 
        Texture2D result =
            DrawArtTexture(
                contours,
                src.width(),
                src.height());
        // 显示 
        ArtImage.texture = result;

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

        OpenCVMatUtils.MatToTexture2D(
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
    AnimatedContour animContour,
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
            animContour.SmoothPoints,
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

        OpenCVMatUtils.MatToTexture2D(
            canvas,
            tex);

        return tex;
    }

    private void DrawPartialContour(
    Mat canvas,
    List<Vector2> smoothPoints,
    float progress)
    {

        //---------------------------------
        // 生长进度
        //--------------------------------- 
        //int visibleCount =
        //    Mathf.Clamp(
        //        Mathf.RoundToInt(//按点数增长
        //            smoothPoints.Count *
        //            progress),
        //        2,
        //        smoothPoints.Count); 
        int visibleCount = Mathf.Clamp(//按路径长度生长
    GetVisiblePointCount(
        smoothPoints,
        progress), 2, smoothPoints.Count);

        Point[] drawPoints =
            new Point[visibleCount];

        for (int i = 0;
             i < visibleCount;
             i++)
        {
            drawPoints[i] =
                new Point(
                    smoothPoints[i].x,
                    smoothPoints[i].y);
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


    List<AnimatedContour> animatedContours =
    new List<AnimatedContour>();
    //动画协程
    public IEnumerator DrawArtAnimated(
    List<MatOfPoint> contours,
    int width,
    int height)
    {
        foreach (var contour in contours)
        {
            animatedContours.Add(
       BuildAnimatedContour(
           contour));
        }

        //---------------------------------
        // 大轮廓优先
        //---------------------------------
        animatedContours.Sort(
    (a, b) =>
    {
        return Imgproc.contourArea(
            b.OriginalContour)
        .CompareTo(
            Imgproc.contourArea(
            a.OriginalContour));
    });



        List<MatOfPoint> finished =
            new List<MatOfPoint>();
        int index = 0;
        foreach (var contour in animatedContours)
        {

            //---------------------------------
            // 按轮廓长度决定动画时间
            //--------------------------------- 
            double length =
                Imgproc.arcLength(
                    new MatOfPoint2f(
                        contour.OriginalContour.toArray()),
                    true);

            float duration =
                Mathf.Lerp(
                    0.15f,
                    1.0f,
                    Mathf.Clamp01(
                        (float)length / 1000f));

            float timer = 0;
            Debug.Log("begin drow contour:" + index + ",totalCount:" + contours.Count + ", duration:" + duration + ",length:" + length);
            index++;

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

                //--------------------------------
                // 笔尖跟随
                //-------------------------------- 
                Vector2 penPoint =
                    GetPenPosition(
                      contour.SmoothPoints,
                        progress);

                PenImg.rectTransform
                    .anchoredPosition =
                    ImagePixelToUIPosition(
                        penPoint.x,
                        penPoint.y);

                // 笔尖旋转（朝向运动方向）
                Vector2 next =
    GetPenPosition(
        contour.SmoothPoints,
        Mathf.Clamp01(
            progress + 0.01f));
                UpdatePenRotation(
    penPoint,
    next);

                yield return null;
            }

            finished.Add(
                contour.OriginalContour);
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


    private Vector2 ImagePixelToUIPosition(
    float x,
    float y)
    {
        RectTransform rt =
            ArtImage.rectTransform;

        float width =
            ArtImage.texture.width;

        float height =
            ArtImage.texture.height;

        float px =
            (x / width) * rt.rect.width;

        float py =
            (y / height) * rt.rect.height;

        px -= rt.rect.width * 0.5f;
        py -= rt.rect.height * 0.5f;

        py = -py;

        return new Vector2(px, py);
    }


    //路径长度计算
    private float CalculateLength(
    List<Vector2> points)
    {
        float length = 0;

        for (int i = 1;
             i < points.Count;
             i++)
        {
            length +=
                Vector2.Distance(
                    points[i - 1],
                    points[i]);
        }

        return length;
    }

    //获取路径进度点
    private int GetVisiblePointCount(
    List<Vector2> points,
    float progress)
    {
        float totalLength =
            CalculateLength(points);

        float targetLength =
            totalLength * progress;

        float currentLength = 0;

        for (int i = 1;
             i < points.Count;
             i++)
        {
            currentLength +=
                Vector2.Distance(
                    points[i - 1],
                    points[i]);

            if (currentLength >= targetLength)
                return i;
        }

        return points.Count;
    }

    //获取笔尖位置
    private Vector2 GetPenPosition(
    List<Vector2> points,
    float progress)
    {
        float totalLength =
            CalculateLength(points);

        float targetLength =
            totalLength * progress;

        float currentLength = 0;

        for (int i = 1;
             i < points.Count;
             i++)
        {
            float segment =
                Vector2.Distance(
                    points[i - 1],
                    points[i]);

            if (currentLength + segment >= targetLength)
            {
                float remain =
                    targetLength -
                    currentLength;

                float t =
                    remain / segment;

                return Vector2.Lerp(
                    points[i - 1],
                    points[i],
                    t);
            }

            currentLength += segment;
        }

        return points[^1];
    }

    private void UpdatePenRotation(
    Vector2 current,
    Vector2 next)
    {
        Vector2 dir =
            next - current;

        float angle =
            Mathf.Atan2(
                dir.y,
                dir.x)
                * Mathf.Rad2Deg;

        PenImg.rectTransform.rotation =
            Quaternion.Euler(
                0,
                0,
                angle - 90);
    }

    //预处理轮廓
    private AnimatedContour BuildAnimatedContour(
    MatOfPoint contour)
    {
        contour =
            ContourSimplifier.Simplify(
                contour,
                Config.Abstraction);

        Point[] points =
            contour.toArray();

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

        List<Vector2> smoothPoints =
            unityPoints;

        if (unityPoints.Count >= 4)
        {
            smoothPoints =
                CatmullRomUtility.SmoothClosed(
                    unityPoints,
                    smoothLevel);
        }

        AnimatedContour result =
            new AnimatedContour();

        result.OriginalContour =
            contour;

        result.SmoothPoints =
            smoothPoints;

        result.Length =
            CalculateLength(
                smoothPoints);

        return result;
    }
}