using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;

public class ContourSimplifierGrok : MonoBehaviour
{
    [Header("输入纹理")]
    public Texture2D sourceTexture;

    [Header("参数调节")]
    [Range(0, 255)] public double cannyThreshold1 = 50;
    [Range(0, 255)] public double cannyThreshold2 = 150;
    [Range(0.001f, 0.1f)] public float approxEpsilon = 0.02f;   // 越小越精确，越大越精简

    [Header("输出")]
    public bool drawOnBlackBackground = true;   // true = 黑底白线，false = 原图上画
    public int contourThickness = 3;

    private Mat rgbMat;
    private Texture2D outputTexture;

    void Start()
    {
        if (sourceTexture == null) return;
        ProcessImage(sourceTexture);
    }

    [ContextMenu("生成精简轮廓图")]
    public void Gen()
    {
        if (sourceTexture == null) return;
        ProcessImage(sourceTexture);
    }

    public void ProcessImage(Texture2D inputTex)
    {
        // 1. Texture2D → Mat
        rgbMat = new Mat(inputTex.height, inputTex.width, CvType.CV_8UC4); // 或 CV_8UC3
        Utils.texture2DToMat(inputTex, rgbMat);

        // 如果是 RGBA，转成 RGB
        Imgproc.cvtColor(rgbMat, rgbMat, Imgproc.COLOR_RGBA2RGB);

        Mat gray = new Mat();
        Mat edges = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();

        // 2. 灰度 + 模糊
        Imgproc.cvtColor(rgbMat, gray, Imgproc.COLOR_RGB2GRAY);
        Imgproc.GaussianBlur(gray, gray, new Size(5, 5), 0);

        // 3. Canny 边缘检测
        Imgproc.Canny(gray, edges, cannyThreshold1, cannyThreshold2);

        // 4. 查找轮廓（推荐 RETR_EXTERNAL 只取外轮廓，CHAIN_APPROX_NONE 保留最多点方便后续简化）
        Imgproc.findContours(edges, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);

        // 5. 创建输出画布
        Mat drawing = drawOnBlackBackground ?
            Mat.zeros(rgbMat.size(), CvType.CV_8UC3) :
            rgbMat.clone();

        // 6. 遍历每个轮廓，进行简化并绘制
        for (int i = 0; i < contours.Count; i++)
        {
            // 简化轮廓（关键步骤）
            MatOfPoint2f approxCurve = new MatOfPoint2f();
            MatOfPoint2f contour2f = new MatOfPoint2f(contours[i].toArray());

            double epsilon = approxEpsilon * Imgproc.arcLength(contour2f, true);
            Imgproc.approxPolyDP(contour2f, approxCurve, epsilon, true);

            // 转回 MatOfPoint 绘制
            MatOfPoint approxContour = new MatOfPoint();
            approxCurve.convertTo(approxContour, CvType.CV_32S);

            // 绘制（颜色可自定义）
            Imgproc.drawContours(drawing, new List<MatOfPoint> { approxContour }, -1,
                                new Scalar(0, 255, 0), contourThickness);   // 绿色

            // 释放临时对象
            approxCurve.Dispose();
            contour2f.Dispose();
            approxContour.Dispose();
        }

        // 7. Mat → Texture2D
        if (outputTexture == null ||
            outputTexture.width != drawing.width() ||
            outputTexture.height != drawing.height())
        {
            outputTexture = new Texture2D(drawing.width(), drawing.height(), TextureFormat.RGB24, false);
        }

        Utils.matToTexture2D(drawing, outputTexture);
        outputTexture.Apply();

        // 显示（例如挂在 RawImage 上）
        GetComponent<UnityEngine.UI.RawImage>().texture = outputTexture;

        // 清理
        gray.Dispose();
        edges.Dispose();
        hierarchy.Dispose();
        drawing.Dispose();
        // rgbMat 可保留或 Dispose
    }

    void OnDestroy()
    {
        if (rgbMat != null) rgbMat.Dispose();
    }
}