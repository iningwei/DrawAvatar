using System.Collections.Generic;
using UnityEngine;
using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgprocModule;

public static class AvatarContourExtractor
{
    public static List<MatOfPoint> ExtractContours(Mat src, AvatarArtConfig config)
    {
        float spatialRadius  = Mathf.Lerp(5f,  25f, config.Abstraction);
        float colorRadius    = Mathf.Lerp(20f, 60f, config.Abstraction);

        Mat srcRGB = new Mat();
        Imgproc.cvtColor(src, srcRGB, Imgproc.COLOR_RGBA2RGB);

        Mat simplified = new Mat();
        Imgproc.pyrMeanShiftFiltering(srcRGB, simplified,
            spatialRadius, colorRadius, 1);

        Mat gray = new Mat();
        Imgproc.cvtColor(simplified, gray, Imgproc.COLOR_RGB2GRAY);

        int blurSize = Mathf.RoundToInt(Mathf.Lerp(3, 15, config.Abstraction));
        if (blurSize % 2 == 0) blurSize++;

        Imgproc.bilateralFilter(gray, gray,
            blurSize, blurSize * 5, blurSize * 5);

        int medianSize = Mathf.Max(3, blurSize - 2);
        if (medianSize % 2 == 0) medianSize++;
        Imgproc.medianBlur(gray, gray, medianSize);

        // Stage 4: Canny + Otsu adaptive thresholds
        Mat edge = new Mat();
        Mat gradX = new Mat();
        Mat gradY = new Mat();
        Imgproc.Sobel(gray, gradX, CvType.CV_32F, 1, 0, 3);
        Imgproc.Sobel(gray, gradY, CvType.CV_32F, 0, 1, 3);
        Mat gradMag = new Mat();
        Core.magnitude(gradX, gradY, gradMag);

        Mat grad8u = new Mat();
        gradMag.convertTo(grad8u, CvType.CV_8UC1, 1.0);
        double otsuThresh = Imgproc.threshold(grad8u, grad8u, 0, 255,
            Imgproc.THRESH_BINARY | Imgproc.THRESH_OTSU);

        float detailScale = Mathf.Lerp(0.3f, 1.0f, config.Detail);
        float abstractScale = Mathf.Lerp(1.5f, 0.5f, config.Abstraction);
        double lowThreshold  = otsuThresh * detailScale * abstractScale;
        lowThreshold = Mathf.Clamp((float)lowThreshold, 5, 200);
        double highThreshold = lowThreshold * 2.5;

        Imgproc.Canny(gray, edge, lowThreshold, highThreshold);

        gradX.Dispose(); gradY.Dispose(); gradMag.Dispose(); grad8u.Dispose();

        // Stage 5: Dilate + Close + Bridge
        int morphSize = Mathf.RoundToInt(Mathf.Lerp(2, 5, 1.0f - config.Detail));
        if (morphSize > 0)
        {
            Mat kernel = Imgproc.getStructuringElement(
                Imgproc.MORPH_ELLIPSE, new Size(morphSize, morphSize));
            Imgproc.dilate(edge, edge, kernel);
            Imgproc.morphologyEx(edge, edge, Imgproc.MORPH_CLOSE, kernel);
        }
        // Bridging: extra dilation to reconnect fragments (scales with Abstraction)
        int bridgeSize = Mathf.RoundToInt(Mathf.Lerp(1, 4, config.Abstraction));
        if (bridgeSize > 1)
        {
            Mat bridgeKernel = Imgproc.getStructuringElement(
                Imgproc.MORPH_ELLIPSE, new Size(bridgeSize, bridgeSize));
            Imgproc.dilate(edge, edge, bridgeKernel);
        }

        // Stage 6: Find contours
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Mat hierarchy = new Mat();
        Imgproc.findContours(edge, contours, hierarchy,
            config.KeepOnlyOuterContours
                ? Imgproc.RETR_EXTERNAL
                : Imgproc.RETR_TREE,
            Imgproc.CHAIN_APPROX_SIMPLE);

        // Stage 7: Depth-aware hierarchy filtering
        if (hierarchy.total() > 0 && !config.KeepOnlyOuterContours && config.MinContourAreaRatio > 0f)
        {
            int count = contours.Count;
            double[] areas = new double[count];
            int[] parents = new int[count];
            int[] depths = new int[count];
            bool[] keep = new bool[count];

            for (int i = 0; i < count; i++)
            {
                areas[i] = Imgproc.contourArea(contours[i]);
                double[] entry = hierarchy.get(0, i);
                parents[i] = (int)entry[3];
                depths[i] = (parents[i] < 0) ? 0 : depths[parents[i]] + 1;
                keep[i] = true;
            }

            for (int i = 0; i < count; i++)
            {
                int p = parents[i];
                if (p >= 0 && p < count && areas[p] > 0)
                {
                    double ratio = areas[i] / areas[p];
                    // depth-1 children (eyes, mouth) use 5x more lenient threshold
                    double threshold = (depths[i] <= 1)
                        ? config.MinContourAreaRatio * 0.2f
                        : config.MinContourAreaRatio;
                    if (ratio < threshold)
                        keep[i] = false;
                }
            }

            List<MatOfPoint> filtered = new List<MatOfPoint>();
            for (int i = 0; i < count; i++) { if (keep[i]) filtered.Add(contours[i]); }
            contours = filtered;
        }

        // Stage 8: Density-based suppression
        if (config.NoiseSuppression > 0f)
        {
            int radius = Mathf.RoundToInt(Mathf.Lerp(55, 15, config.NoiseSuppression));
            int sqrRadius = radius * radius;
            int minNeighbors = Mathf.RoundToInt(Mathf.Lerp(10, 3, config.NoiseSuppression));

            int count = contours.Count;
            double[] areas = new double[count];
            Vector2[] centers = new Vector2[count];
            bool[] keep = new bool[count];

            double maxArea = 0;
            for (int i = 0; i < count; i++)
            {
                OpenCVForUnity.CoreModule.Rect r = Imgproc.boundingRect(contours[i]);
                centers[i] = new Vector2(r.x + r.width * 0.5f, r.y + r.height * 0.5f);
                areas[i] = Imgproc.contourArea(contours[i]);
                maxArea = Mathf.Max((float)maxArea, (float)areas[i]);
                keep[i] = true;
            }

            double areaThreshold = maxArea * 0.03;

            for (int i = 0; i < count; i++)
            {
                if (areas[i] >= areaThreshold) continue;
                int n = 0;
                for (int j = 0; j < count; j++)
                {
                    if (i == j) continue;
                    float dx = centers[i].x - centers[j].x;
                    float dy = centers[i].y - centers[j].y;
                    if (dx * dx + dy * dy < sqrRadius) n++;
                }
                if (n >= minNeighbors) keep[i] = false;
            }

            List<MatOfPoint> filtered = new List<MatOfPoint>();
            for (int i = 0; i < count; i++) { if (keep[i]) filtered.Add(contours[i]); }
            contours = filtered;
        }

        // Stage 9: Area/length filter (absolute + relative fallback)
        double maxContourArea = 0;
        for (int i = 0; i < contours.Count; i++)
        {
            double a = Imgproc.contourArea(contours[i]);
            if (a > maxContourArea) maxContourArea = a;
        }
        double relativeMinArea = maxContourArea * 0.001;

        contours.RemoveAll(contour =>
        {
            double area = Imgproc.contourArea(contour);
            double length = Imgproc.arcLength(
                new MatOfPoint2f(contour.toArray()), true);

            bool absFail = area < config.MinContourArea || length < config.MinContourLength;
            bool relFail = area < relativeMinArea;

            // Must fail BOTH absolute AND relative to be removed
            return absFail && relFail;
        });

        // Stage 10: Sort + limit
        contours.Sort((a, b) =>
            Imgproc.contourArea(b).CompareTo(Imgproc.contourArea(a)));

        if (contours.Count > config.MaxContours)
        {
            contours.RemoveRange(config.MaxContours,
                contours.Count - config.MaxContours);
        }

        srcRGB.Dispose();
        simplified.Dispose();
        gray.Dispose();
        edge.Dispose();

        return contours;
    }
}
