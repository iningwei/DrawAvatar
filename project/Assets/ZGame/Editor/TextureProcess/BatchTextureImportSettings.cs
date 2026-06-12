using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class BatchTextureImportSettings : EditorWindow
{
    private int maxTextureSize = 1024; // 默认值
    private readonly int[] textureSizeOptions = { 256, 512, 1024, 2048, 4096 }; // 可选分辨率
    private readonly string[] textureSizeDisplay = { "256", "512", "1024", "2048", "4096" };


    [MenuItem("Tools/Batch Texture Import Settings")]
    static void Init()
    {
        BatchTextureImportSettings window = (BatchTextureImportSettings)EditorWindow.GetWindow(typeof(BatchTextureImportSettings));
        window.Show();
    }

    void OnGUI()
    {
        // 提供下拉菜单选择maxTextureSize
        GUILayout.Label("Max Texture Size:", EditorStyles.boldLabel);
        maxTextureSize = textureSizeOptions[EditorGUILayout.Popup(System.Array.IndexOf(textureSizeOptions, maxTextureSize), textureSizeDisplay)];


        if (GUILayout.Button("Apply Import Settings to Selected Textures"))
        {
            ApplySettings();
        }
    }

    void ApplySettings()
    {
        string[] guids = Selection.assetGUIDs;
        List<string> allAssetPathList = new List<string>();
        foreach (var guid in guids)
        {
            // 将 GUID 转换为 路径
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            // 判断是否文件夹
            if (Directory.Exists(assetPath))
            {
                Debug.Log("process folder:" + assetPath);
                ProcessFolder(assetPath);
            }
            else
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                if (asset != null)
                {
                    ProcessTexture(assetPath);
                }
            }
        }



        // 刷新AssetDatabase
        AssetDatabase.Refresh();
        Debug.Log("Batch texture import settings completed!");
    }

    void ProcessFolder(string folderPath)
    {
        // 获取文件夹及其子文件夹中的所有文件
        string[] assetPaths = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith(".meta")) // 排除meta文件
            .Select(file => file.Replace(Application.dataPath, "Assets").Replace("\\", "/"))
            .ToArray();

        foreach (string path in assetPaths)
        {
            // 只处理Texture2D类型的资源
            Object asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            if (asset != null)
            {
                ProcessTexture(path);
            }
        }
    }

    void ProcessTexture(string path)
    {
        TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;

        if (textureImporter != null)
        {
            // 设置基本参数
            textureImporter.maxTextureSize = maxTextureSize;

            // 获取Windows平台的设置
            TextureImporterPlatformSettings windowsSettings = textureImporter.GetPlatformTextureSettings("Standalone");

            // 启用Windows平台覆写
            windowsSettings.overridden = true;
            windowsSettings.maxTextureSize = maxTextureSize;

            // 检查是否是Normal Map
            bool isNormalMap = textureImporter.textureType == TextureImporterType.NormalMap;

            if (isNormalMap)
            {
                // Normal Map使用RGBA Crunched DXT5|BC3
                windowsSettings.format = TextureImporterFormat.DXT5Crunched;
                Debug.Log($"Processed Normal Map texture: {textureImporter.assetPath} with DXT5Crunched, MaxSize: {maxTextureSize}");
            }
            else
            {
                // 检查是否有Alpha通道
                bool hasAlpha = textureImporter.DoesSourceTextureHaveAlpha();

                // 根据是否有Alpha通道设置Format
                if (hasAlpha)
                {
                    windowsSettings.format = TextureImporterFormat.DXT5Crunched; // RGBA Crunched DXT5|BC3
                }
                else
                {
                    windowsSettings.format = TextureImporterFormat.DXT1Crunched; // RGB Crunched DXT1|BC1
                }
                Debug.Log($"Processed texture: {textureImporter.assetPath} with {(hasAlpha ? "DXT5Crunched" : "DXT1Crunched")}, MaxSize: {maxTextureSize}");
            }

            // 应用Windows平台的设置
            textureImporter.SetPlatformTextureSettings(windowsSettings);

            // 保存更改
            AssetDatabase.ImportAsset(path);
        }
    }
}