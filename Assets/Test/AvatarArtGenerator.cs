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
    [Header("头像")]
    public Texture2D Avatar;

    [Header("结果")]
    public RawImage ArtImage;

    [Header("配置")]
    public AvatarArtConfig Config;

    [Header("昵称")]
    public string NickName;
    [Header("笔尖")]
    public Image PenImg;

    public AvatarSignature Signature;

    private int _padOffsetX;
    private int _padOffsetY;
    private int _paddedSize;

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

        Mat src = new Mat(
            Avatar.height,
            Avatar.width,
            CvType.CV_8UC4);

        OpenCVMatUtils.Texture2DToMat(
            Avatar,
            src);

        List<MatOfPoint> contours =
            AvatarContourExtractor.ExtractContours(
                src,
                Config);

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

        Mat src = new Mat(
            Avatar.height,
            Avatar.width,
            CvType.CV_8UC4);

        OpenCVMatUtils.Texture2DToMat(
            Avatar,
            src);

        List<MatOfPoint> contours =
            AvatarContourExtractor.ExtractContours(
                src,
                Config);

        Texture2D result =
            DrawArtTexture(
                contours,
                src.width(),
                src.height(),
                true);
        ArtImage.texture = result;
        Debug.LogError("contours count:" + contours.Count);

    }

    private Texture2D DrawArtTexture(
        List<MatOfPoint> contours,
        int width,
        int height,
        bool autoPadSquare = false)
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

        if (autoPadSquare)
            canvas = PadToSquare(canvas, bg);

        Texture2D tex =
            new Texture2D(
                canvas.width(),
                canvas.height(),
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
        Point[] points = contour.toArray();

        if (points.Length < 4)
            return;

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
            CatmullRomUtility.SmoothClosed(
                unityPoints,
                smoothLevel);

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

        foreach (var contour in finishedContours)
        {
            DrawContour(
                canvas,
                contour);
        }

        DrawPartialContour(
            canvas,
            animContour.SmoothPoints,
            progress);

        canvas = PadToSquare(canvas, bg);

        Texture2D tex =
            new Texture2D(
                canvas.width(),
                canvas.height(),
                TextureFormat.RGBA32,
                false);

        OpenCVMatUtils.MatToTexture2D(
            canvas,
            tex);

        return tex;
    }

    private Mat PadToSquare(Mat source, Scalar bgColor)
    {
        int w = source.width();
        int h = source.height();

        if (w == h)
        {
            _padOffsetX = 0;
            _padOffsetY = 0;
            _paddedSize = w;
            return source;
        }

        int size = Mathf.Max(w, h);
        Mat square = new Mat(size, size, CvType.CV_8UC4, bgColor);

        int offsetX = (size - w) / 2;
        int offsetY = (size - h) / 2;

        Mat roi = square.submat(offsetY, offsetY + h, offsetX, offsetX + w);
        source.copyTo(roi);
        source.Dispose();

        _padOffsetX = offsetX;
        _padOffsetY = offsetY;
        _paddedSize = size;

        return square;
    }

    private void DrawPartialContour(
    Mat canvas,
    List<Vector2> smoothPoints,
    float progress)
    {
        int visibleCount = Mathf.Clamp(
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
            false,
            stroke,
            Config.StrokeWidth,
            Imgproc.LINE_AA);
    }


    List<AnimatedContour> animatedContours =
    new List<AnimatedContour>();
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

                Vector2 penPoint =
                    GetPenPosition(
                      contour.SmoothPoints,
                        progress);

                PenImg.rectTransform
                    .anchoredPosition =
                    ImagePixelToUIPosition(
                        penPoint.x,
                        penPoint.y);

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

        Texture2D final =
            DrawArtTexture(
                contours,
                width,
                height,
                true);

        ArtImage.texture =
            final;

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

        float px = x + _padOffsetX;
        float py = y + _padOffsetY;

        float texW = _paddedSize;
        float texH = _paddedSize;

        float ux =
            (px / texW) * rt.rect.width;

        float uy =
            (py / texH) * rt.rect.height;

        ux -= rt.rect.width * 0.5f;
        uy -= rt.rect.height * 0.5f;

        uy = -uy;

        return new Vector2(ux, uy);
    }


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
