using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ProjectSyncTool : EditorWindow
{
    private string sourceProjectPath; // 移除初始值
    private string targetProjectPath; // 移除初始值
    private bool syncExistingOnly = false;

    [MenuItem("Tools/Project Sync Tool")]
    public static void ShowWindow()
    {
        GetWindow<ProjectSyncTool>("Project Sync Tool");
    }

    void OnEnable()
    {
        // 在 OnEnable 中初始化路径
        sourceProjectPath = Path.Combine(Application.dataPath, "../project");
        targetProjectPath = Path.Combine(Application.dataPath, "../art_project");
    }

    void OnGUI()
    {
        GUILayout.Label("Project Sync Tool", EditorStyles.boldLabel);

        sourceProjectPath = EditorGUILayout.TextField("Source Project Path", sourceProjectPath);
        targetProjectPath = EditorGUILayout.TextField("Target Project Path", targetProjectPath);
        syncExistingOnly = EditorGUILayout.Toggle("Sync Existing Files Only", syncExistingOnly);

        if (GUILayout.Button("Sync Selected Files"))
        {
            SyncSelectedFiles();
        }
    }

    private void SyncSelectedFiles()
    {
        // 获取选中的对象
        Object[] selectedAssets = Selection.GetFiltered(typeof(Object), SelectionMode.Assets);

        if (selectedAssets.Length == 0)
        {
            EditorUtility.DisplayDialog("Error", "Please select at least one file or folder", "OK");
            return;
        }

        // 转换为实际的文件路径
        List<string> sourcePaths = new List<string>();
        foreach (Object asset in selectedAssets)
        {
            string path = AssetDatabase.GetAssetPath(asset);
            string fullPath = Path.Combine(Application.dataPath, path.Substring("Assets/".Length));
            sourcePaths.Add(fullPath);

            // 如果是文件夹，添加所有子文件
            if (Directory.Exists(fullPath))
            {
                string[] files = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
                sourcePaths.AddRange(files);
            }
        }

        // 执行同步
        foreach (string sourcePath in sourcePaths)
        {
            if (!File.Exists(sourcePath) && !Directory.Exists(sourcePath)) continue;

            string relativePath = sourcePath.Replace(sourceProjectPath, "").TrimStart(Path.DirectorySeparatorChar);
            string targetPath = Path.Combine(targetProjectPath, relativePath);
            string targetMetaPath = targetPath + ".meta";
            string sourceMetaPath = sourcePath + ".meta";

            // 如果只同步存在的文件，检查目标路径是否存在
            if (syncExistingOnly && !File.Exists(targetPath) && !Directory.Exists(targetPath))
            {
                continue;
            }

            try
            {
                // 创建目标文件夹
                string targetDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                // 复制主文件
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath, true);
                    Debug.Log($"Synced file: {relativePath}");
                }

                // 复制.meta文件
                if (File.Exists(sourceMetaPath))
                {
                    File.Copy(sourceMetaPath, targetMetaPath, true);
                    Debug.Log($"Synced meta: {relativePath}.meta");
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to sync {relativePath}: {e.Message}");
            }
        }

        //EditorUtility.DisplayDialog("Success", "File sync completed!", "OK");
        //AssetDatabase.Refresh();
    }
}