using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnity.UnityUtils;

public class SimplifiedContourGenerator : MonoBehaviour
{
    [Header("输入输出设置")]
    public Texture2D inputTexture;
    public RenderTexture outputRenderTexture;

    [Header("轮廓参数（调大更精简）")]
    [Range(10, 150)] public int cannyLowThresh = 50;
    [Range(100, 300)] public int cannyHighThresh = 150;
    [Range(1, 15)] public int blurKernelSize = 5;
    [Range(1, 5)] public int morphKernelSize = 2;

    void Start()
    {
        if (inputTexture == null)
        {
            Debug.LogError("请给 inputTexture 赋值一张图片");
            return;
        }
        GenerateContour();
    }

    [ContextMenu("生成精简轮廓图")]
    public void GenerateContour()
    {
        // 1. 将 Texture2D 转为 OpenCV 的 Mat
        Mat srcMat = new Mat(inputTexture.height, inputTexture.width, CvType.CV_8UC3);
        Utils.texture2DToMat(inputTexture, srcMat);
        //修正通道顺序，避免边缘检测错乱
        //Unity里面图片是RGB顺序，OpenCV里Mat默认是BGR顺序。
        Imgproc.cvtColor(srcMat, srcMat, Imgproc.COLOR_BGRA2BGR);


        // 2. 灰度化 + 高斯模糊降噪
        Mat grayMat = new Mat();
        Imgproc.cvtColor(srcMat, grayMat, Imgproc.COLOR_RGB2GRAY);

        Mat blurredMat = new Mat();
        Imgproc.GaussianBlur(grayMat, blurredMat, new Size(blurKernelSize, blurKernelSize), 0);

        // 3. Canny 边缘检测（核心）
        Mat edgesMat = new Mat();
        Imgproc.Canny(blurredMat, edgesMat, cannyLowThresh, cannyHighThresh);

        // 4. 形态学操作，去除细碎噪点线条
        Mat morphMat = new Mat();
        Mat kernel = Mat.ones(morphKernelSize, morphKernelSize, CvType.CV_8UC1);
        Imgproc.morphologyEx(edgesMat, morphMat, Imgproc.MORPH_CLOSE, kernel);

        // 5. 反转颜色，得到白底黑线条
        Mat contourMat = new Mat();
        Core.bitwise_not(morphMat, contourMat);

        // 6. 转回 Texture2D 并显示
        Texture2D resultTexture = new Texture2D(contourMat.cols(), contourMat.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(contourMat, resultTexture);

        // 如果赋值了 RenderTexture，也可以写入
        if (outputRenderTexture != null)
        {
            Graphics.Blit(resultTexture, outputRenderTexture);
        }

        // 释放资源
        srcMat.Dispose();
        grayMat.Dispose();
        blurredMat.Dispose();
        edgesMat.Dispose();
        morphMat.Dispose();
        kernel.Dispose();
        contourMat.Dispose();

        Debug.Log("✅ 轮廓图生成完成！");
    }
}