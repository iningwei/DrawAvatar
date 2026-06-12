using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class GameMonitor : MonoBehaviour
{
    float duration = 0.3f;
    float lastTime = 0f;

    //性能指标
    ProfilerRecorder setPassCallsRecorder;
    ProfilerRecorder batchesCallsRecorder;//
    ProfilerRecorder drawCallsRecorder;
    ProfilerRecorder verticesRecorder;
    ProfilerRecorder triangleRecorder;//

    //用时指标
    ProfilerRecorder mainThreadCostRecorder;//
    ProfilerRecorder cameraRenderCostRecorder;//
    ProfilerRecorder renderLoopDrawCostRecorder;//

    //内存相关
    ProfilerRecorder systemUsedMemoryRecorder;
    ProfilerRecorder totalReservedMemoryRecorder;//
    ProfilerRecorder totalUsedMemoryRecorder;//

    ProfilerRecorder gcReservedMemoryRecorder;
    ProfilerRecorder gcUsedMemoryRecorder;//
    ProfilerRecorder gfxUsedMemoryRecorder;//





    public long setpassCount = 0;
    public long batchesCount = 0;
    public long drawcallCount = 0;
    public long verticesCount = 0;
    public long triangleCount = 0;

    public string mainThreadCostValue = "0";//单位毫秒ms
    public string cameraRenderCostValue = "0";//
    public string renderLoopDrawCostValue = "0";//

    public string systemUsedMomoryValue="0";//单位MB//
    public string totalReservedMemoryValue = "0";//
    public string totalUsedMemoryValue = "0";
    public string gcUsedMemoryValue = "0";
    public string gfxUsedMemoryValue = "0";
    public string gcReservedMemoryValue = "0";//

    public string fps = "0";

    private void OnEnable()
    {
        setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "SetPass Calls Count");

        batchesCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Batches Count");
        drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Draw Calls Count");
        verticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Vertices Count");
        triangleRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Triangles Count");

        mainThreadCostRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Main Thread");
        cameraRenderCostRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "Camera.Render");
        renderLoopDrawCostRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, "RenderLoop.Draw");

        systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "System Used Memory");
        totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Reserved Memory");
        totalUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Total Used Memory");

        gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Reserved Memory");
        gcUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Used Memory");


        gfxUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "Gfx Used Memory");
         
    }
    static double GetRecorderFrameAverage(ProfilerRecorder recorder)
    {
        var samplesCount = recorder.Capacity;
        if (samplesCount == 0)
            return 0;

        double r = 0;
        var samples = new List<ProfilerRecorderSample>(samplesCount);
        recorder.CopyTo(samples);
        for (var i = 0; i < samples.Count; ++i)
            r += samples[i].Value;
        r /= samplesCount;

        return r;
    }



    private int frame;
    private void Update()
    {
        frame++;


        if (Time.time - lastTime > duration)
        {

            fps = (frame / (Time.time - lastTime)).ToString("F1");
            frame = 0;

            if (setPassCallsRecorder.Valid)
            {
                setpassCount = setPassCallsRecorder.LastValue;
            }
            if (batchesCallsRecorder.Valid)
            {
                batchesCount = batchesCallsRecorder.LastValue;
            }
            if (drawCallsRecorder.Valid)
            {
                drawcallCount = drawCallsRecorder.LastValue;
            }
            if (verticesRecorder.Valid)
            {
                verticesCount = verticesRecorder.LastValue;
            }
            if (triangleRecorder.Valid)
            {
                triangleCount = triangleRecorder.LastValue;
            }


            if (mainThreadCostRecorder.Valid)
            {
                mainThreadCostValue = (GetRecorderFrameAverage(mainThreadCostRecorder) * (1e-6f)).ToString("F2");
            }
            if (cameraRenderCostRecorder.Valid)
            {
                cameraRenderCostValue = (GetRecorderFrameAverage(cameraRenderCostRecorder) * (1e-6f)).ToString("F2");
            }
            if (renderLoopDrawCostRecorder.Valid)
            {
                renderLoopDrawCostValue = (GetRecorderFrameAverage(renderLoopDrawCostRecorder) * (1e-6f)).ToString("F2");
            }



            if (systemUsedMemoryRecorder.Valid)
            {
                systemUsedMomoryValue = (systemUsedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }
            if (totalReservedMemoryRecorder.Valid)
            {
                totalReservedMemoryValue = (totalReservedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }
            if (totalUsedMemoryRecorder.Valid)
            {
                totalUsedMemoryValue = (totalUsedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }



            if (gcReservedMemoryRecorder.Valid)
            {
                gcReservedMemoryValue = (gcReservedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }
            if (gcUsedMemoryRecorder.Valid)
            {
                gcUsedMemoryValue = (gcUsedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }
            if (gfxUsedMemoryRecorder.Valid)
            {
                gfxUsedMemoryValue = (gfxUsedMemoryRecorder.LastValue / 1024 / 1024).ToString("F1");
            }

            lastTime = Time.time;
        }
    }

    private void OnDisable()
    {
        setPassCallsRecorder.Dispose();
        batchesCallsRecorder.Dispose();
        drawCallsRecorder.Dispose();
        verticesRecorder.Dispose();
        triangleRecorder.Dispose();

        mainThreadCostRecorder.Dispose();
        cameraRenderCostRecorder.Dispose();
        renderLoopDrawCostRecorder.Dispose();

        systemUsedMemoryRecorder.Dispose();
        totalReservedMemoryRecorder.Dispose();
        totalUsedMemoryRecorder.Dispose();
        gcReservedMemoryRecorder.Dispose();
        gcUsedMemoryRecorder.Dispose();
        gfxUsedMemoryRecorder.Dispose();
    }
}
