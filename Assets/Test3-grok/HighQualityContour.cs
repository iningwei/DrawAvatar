using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;
using UnityEngine.UI;

public class HighQualityContour : MonoBehaviour
{
    [Header("输入图片")]
    public Texture2D inputTexture;

    [Header("参数调节")]
    [Range(0.001f, 0.05f)]
    public float approxEpsilon = 0.012f;      // 越小越精细，越大越精简（推荐 0.008~0.018）

    [Range(1, 8)]
    public int thickness = 4;

    [Range(0, 2000)]
    public int minArea = 800;                 // 过滤小噪点

    public Color lineColor = Color.cyan;      // 线条颜色

    private Texture2D outputTexture;

    void Start()
    {
        if (inputTexture != null)
        {
            GenerateHighQualityContour(inputTexture);
        }
    }
    [ContextMenu("生成高级轮廓图")]
    public void Gen()
    {
        if (inputTexture != null)
        {
            GenerateHighQualityContour(inputTexture);
        }
    }


    /// <summary>
    /// 生成高质量精简轮廓图
    /// </summary>
    public void GenerateHighQualityContour(Texture2D tex)
    {
        if (tex == null) return;

        Mat src = new Mat(tex.height, tex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(tex, src);

        Mat gray = new Mat();
        Mat denoised = new Mat();
        Mat edges = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();

        // 1. 转灰度
        Imgproc.cvtColor(src, gray, Imgproc.COLOR_RGBA2GRAY);

        // 2. 强去噪 + 保留边缘（双边滤波）
        Imgproc.bilateralFilter(gray, denoised, 9, 75, 75);

        // 3. Canny 边缘检测
        Imgproc.Canny(denoised, edges, 40, 120);

        // 4. 轻微膨胀让线条更连续
        Mat kernel = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(2, 2));
        Imgproc.dilate(edges, edges, kernel);

        // 5. 查找轮廓
        Imgproc.findContours(edges, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_TC89_KCOS);

        // 6. 创建黑底画布
        Mat drawing = Mat.zeros(src.size(), CvType.CV_8UC3);

        // 颜色转换
        Scalar color = new Scalar(lineColor.b * 255, lineColor.g * 255, lineColor.r * 255);

        // 7. 处理每个轮廓
        for (int i = 0; i < contours.Count; i++)
        {
            double area = Imgproc.contourArea(contours[i]);
            if (area < minArea) continue;

            // 轮廓简化（精简关键）
            MatOfPoint2f tmp = new MatOfPoint2f(contours[i].toArray());
            double eps = approxEpsilon * Imgproc.arcLength(tmp, true);

            MatOfPoint2f approx = new MatOfPoint2f();
            Imgproc.approxPolyDP(tmp, approx, eps, true);

            MatOfPoint approxInt = new MatOfPoint();
            approx.convertTo(approxInt, CvType.CV_32S);

            if (approxInt.total() > 15) // 过滤过于简单的轮廓
            {
                Imgproc.drawContours(drawing, new List<MatOfPoint> { approxInt }, -1, color, thickness);
            }

            // 清理
            tmp.Dispose();
            approx.Dispose();
            approxInt.Dispose();
        }

        // 8. Mat 转 Texture2D
        if (outputTexture == null ||
            outputTexture.width != drawing.width() ||
            outputTexture.height != drawing.height())
        {
            outputTexture = new Texture2D(drawing.width(), drawing.height(), TextureFormat.RGB24, false);
        }

        Utils.matToTexture2D(drawing, outputTexture);
        outputTexture.Apply();

        // 显示到 RawImage
        RawImage rawImage = GetComponent<UnityEngine.UI.RawImage>();
        if (rawImage != null)
        {
            rawImage.texture = outputTexture;
        }
        else
        {
            // 如果没有 RawImage，尝试挂在 MeshRenderer 上
            var renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.mainTexture = outputTexture;
            }
        }

        // 清理内存
        src.Dispose();
        gray.Dispose();
        denoised.Dispose();
        edges.Dispose();
        hierarchy.Dispose();
        drawing.Dispose();
        kernel.Dispose();
    }

    void OnDestroy()
    {
        if (outputTexture != null)
            Destroy(outputTexture);
    }
}