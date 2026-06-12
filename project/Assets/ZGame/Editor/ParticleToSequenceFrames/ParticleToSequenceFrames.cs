using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ParticleToSequenceFrames : EditorWindow
{
    private GameObject particleEffect;
    private Camera captureCamera;

    private int frameRate = 30;
    private int resolution = 512;
    private float totalDuration = 0f;

    private bool isExporting = false;
    private string currentSavePath;

    // 用于协程的引用
    private IEnumerator exportCoroutine;

    [MenuItem("Tools/粒子特效转序列帧")]
    public static void ShowWindow()
    {
        GetWindow<ParticleToSequenceFrames>("粒子转序列帧").Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("粒子特效转序列帧工具", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("1. 拖入粒子特效 GameObject：");
        particleEffect = (GameObject)EditorGUILayout.ObjectField(particleEffect, typeof(GameObject), true);
        if (particleEffect != null)
        {
            UpdateTotalDuration();
            GUILayout.Label($"粒子特效总时长：{totalDuration:F2} 秒", EditorStyles.helpBox);
        }

        GUILayout.Label("2. 拖入用于渲染的 Camera：");
        captureCamera = (Camera)EditorGUILayout.ObjectField(captureCamera, typeof(Camera), true);



        GUILayout.Space(10);
        frameRate = EditorGUILayout.IntField("导出帧率 (FPS)", frameRate);
        resolution = EditorGUILayout.IntField("序列帧分辨率 (正方形)", resolution);

        GUI.enabled = !isExporting;
        if (GUILayout.Button("开始导出序列帧", GUILayout.Height(40)))
        {
            if (particleEffect == null || captureCamera == null)
            {
                EditorUtility.DisplayDialog("错误", "请先拖入粒子特效物体和相机！", "确定");
                return;
            }
            if (frameRate <= 0 || resolution <= 0)
            {
                EditorUtility.DisplayDialog("错误", "帧率和分辨率必须大于0！", "确定");
                return;
            }

            StartExport();
        }
        GUI.enabled = true;

        if (isExporting)
        {
            EditorGUILayout.HelpBox("正在导出中... 请勿关闭窗口", MessageType.Info);
        }
    }


    private void UpdateTotalDuration()
    {
        if (particleEffect == null) return;

        float maxDuration = 0f;
        var systems = particleEffect.GetComponentsInChildren<ParticleSystem>(true);

        foreach (var ps in systems)
        {
            var main = ps.main;
            float dur = main.duration + main.startDelay.constantMax;

            // 对循环粒子给一个合理的默认时长
            if (main.loop)
                dur = Mathf.Max(dur, 5f);   // 可自行修改这个默认值

            maxDuration = Mathf.Max(maxDuration, dur);
        }

        totalDuration = maxDuration;
    }

    private void StartExport()
    {
        string exportRootPath = Path.Combine(Path.GetDirectoryName(Application.dataPath), "ParticleToSequenceFrames");
        if (!Directory.Exists(exportRootPath))
            Directory.CreateDirectory(exportRootPath);

        string folderName = particleEffect.name;
        currentSavePath = Path.Combine(exportRootPath, folderName);

        if (!Directory.Exists(currentSavePath))
            Directory.CreateDirectory(currentSavePath);

        isExporting = true;

        // 使用 EditorWindow 的方式启动协程
        exportCoroutine = ExportSequence();
        EditorApplication.update += EditorUpdate;
    }

    private void EditorUpdate()
    {
        if (exportCoroutine != null)
        {
            if (!exportCoroutine.MoveNext())
            {
                // 协程结束
                FinishExport();
            }
        }
    }

    private IEnumerator ExportSequence()
    {
        // URP 下推荐使用 ARGB32 + 不开启 HDR
        RenderTexture rt = new RenderTexture(resolution, resolution, 32, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 4;
        rt.filterMode = FilterMode.Bilinear;

        captureCamera.targetTexture = rt;
        captureCamera.clearFlags = CameraClearFlags.SolidColor;
        captureCamera.backgroundColor = new Color(0, 0, 0, 0);

        var allPS = particleEffect.GetComponentsInChildren<ParticleSystem>(true);

        // 重置粒子
        foreach (var ps in allPS)
        {
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps.Play(true);
        }

        int totalFrames = Mathf.CeilToInt(totalDuration * frameRate);
        float frameTime = 1f / frameRate;

        for (int i = 0; i < totalFrames; i++)
        {
            // 推进模拟
            foreach (var ps in allPS)
            {
                ps.Simulate(frameTime, true, false, true);
            }

            // URP 下强制刷新
            UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
            UnityEditor.SceneView.RepaintAll();

            captureCamera.Render();

            RenderTexture.active = rt;
            Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
            tex.Apply();

            string fileName = $"{particleEffect.name}_{i:D4}.png";//{i:D4}
            string filePath = Path.Combine(currentSavePath, fileName);
            File.WriteAllBytes(filePath, tex.EncodeToPNG());

            Object.DestroyImmediate(tex);

            EditorUtility.DisplayProgressBar("导出序列帧",
                $"正在导出第 {i + 1}/{totalFrames} 帧", (float)i / totalFrames);

            //yield return null;
            yield return new WaitForEndOfFrame();
        }

        // 清理
        RenderTexture.active = null;
        captureCamera.targetTexture = null;
        rt.Release();
        Object.DestroyImmediate(rt);

        foreach (var ps in allPS)
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        EditorUtility.ClearProgressBar();
    }
    private void FinishExport()
    {
        EditorApplication.update -= EditorUpdate;
        isExporting = false;
        exportCoroutine = null;

        EditorUtility.DisplayDialog("导出完成",
            $"序列帧导出成功！\n\n路径：{currentSavePath}\n共 {Mathf.CeilToInt(totalDuration * frameRate)} 张 PNG",
            "确定");

        // 打开导出文件夹
        EditorUtility.RevealInFinder(currentSavePath);
    }

    private void OnDestroy()
    {
        if (isExporting)
        {
            EditorApplication.update -= EditorUpdate;
            EditorUtility.ClearProgressBar();
        }
    }
}