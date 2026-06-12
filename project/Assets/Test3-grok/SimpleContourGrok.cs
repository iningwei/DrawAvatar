using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;
using System.Collections.Generic;

public class SimpleContour : MonoBehaviour
{
    public Texture2D inputTexture;
    public float approxEpsilon = 0.015f;     // 精简程度，建议 0.01~0.025
    public int thickness = 4;

    void Start()
    {
        if (inputTexture == null) return;
        GenerateContour(inputTexture);
    }

    [ContextMenu("生成精简轮廓图2")]
    public void Gen()
    {
        if (inputTexture == null) return;
        GenerateContour(inputTexture);
    }

    public void GenerateContour(Texture2D tex)
    {
        Mat src = new Mat(tex.height, tex.width, CvType.CV_8UC4);
        Utils.texture2DToMat(tex, src);

        Mat gray = new Mat();
        Mat edges = new Mat();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();

        Imgproc.cvtColor(src, gray, Imgproc.COLOR_RGBA2GRAY);
        Imgproc.GaussianBlur(gray, gray, new Size(3, 3), 0);
        Imgproc.Canny(gray, edges, 60, 180);

        Imgproc.findContours(edges, contours, hierarchy, Imgproc.RETR_EXTERNAL, Imgproc.CHAIN_APPROX_NONE);

        Mat drawing = Mat.zeros(src.size(), CvType.CV_8UC3); // 黑底

        Scalar color = new Scalar(0, 255, 100); // 亮绿色

        for (int i = 0; i < contours.Count; i++)
        {
            MatOfPoint2f tmp = new MatOfPoint2f(contours[i].toArray());
            double eps = approxEpsilon * Imgproc.arcLength(tmp, true);
            MatOfPoint2f approx = new MatOfPoint2f();
            Imgproc.approxPolyDP(tmp, approx, eps, true);

            MatOfPoint approxInt = new MatOfPoint();
            approx.convertTo(approxInt, CvType.CV_32S);

            if (approxInt.total() > 15) // 过滤太小的轮廓
            {
                Imgproc.drawContours(drawing, new List<MatOfPoint> { approxInt }, -1, color, thickness);
            }

            tmp.Dispose();
            approx.Dispose();
            approxInt.Dispose();
        }

        Texture2D result = new Texture2D(drawing.cols(), drawing.rows(), TextureFormat.RGB24, false);
        Utils.matToTexture2D(drawing, result);
        result.Apply();

        GetComponent<UnityEngine.UI.RawImage>().texture = result;

        // 清理内存
        src.Dispose(); gray.Dispose(); edges.Dispose(); hierarchy.Dispose(); drawing.Dispose();
    }
}