using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI; // 引入 UGUI 命名空间
using UnityEngine.Rendering;
using ZGame.Res;

public class CollectShaderList : Editor
{
    [MenuItem("Tools/CollectShaderList")]
    private static void CollectShaders()
    {
        try
        {
            // 获取ShaderList预制件
            string shaderListPath = "Assets/ArtResources/ShaderList/ShaderList.prefab";
            GameObject shaderListPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(shaderListPath);
            if (shaderListPrefab == null)
            {
                Debug.LogError($"未找到ShaderList预制件，请检查路径：{shaderListPath}");
                return;
            }

            // 获取ShaderList组件
            ShaderList shaderList = shaderListPrefab.GetComponent<ShaderList>();
            if (shaderList == null)
            {
                Debug.LogError("ShaderList预制件上未找到ShaderList组件");
                return;
            }

            // 获取Built-in Always Included Shaders
            HashSet<Shader> alwaysIncludedShaders = new HashSet<Shader>();
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            if (graphicsSettings != null)
            {
                var serializedObject = new SerializedObject(graphicsSettings);
                var alwaysIncludedShadersProp = serializedObject.FindProperty("m_AlwaysIncludedShaders");
                if (alwaysIncludedShadersProp != null)
                {
                    for (int i = 0; i < alwaysIncludedShadersProp.arraySize; i++)
                    {
                        var shader = alwaysIncludedShadersProp.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
                        if (shader != null)
                            alwaysIncludedShaders.Add(shader);
                    }
                }
            }

            // 收集所有prefab引用的shader并打印
            HashSet<Shader> collectedShaders = new HashSet<Shader>();
            Dictionary<string, List<Shader>> prefabShaderMap = new Dictionary<string, List<Shader>>();
            string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/ArtResources" });

            foreach (string guid in prefabGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                List<Shader> prefabShaders = new List<Shader>();

                // 1. 获取所有Renderer组件的Shader
                Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
                foreach (Renderer renderer in renderers)
                {
                    foreach (Material material in renderer.sharedMaterials)
                    {
                        if (material != null && material.shader != null)
                        {
                            Shader shader = material.shader;
                            prefabShaders.Add(shader);
                            collectedShaders.Add(shader);
                        }
                    }
                }

                // 2. 获取所有UGUI组件的Shader（Image, RawImage, Text等）
                Graphic[] uiGraphics = prefab.GetComponentsInChildren<Graphic>(true);
                foreach (Graphic graphic in uiGraphics)
                {
                    Material material = graphic.material;
                    if (material != null && material.shader != null)
                    {
                        Shader shader = material.shader;
                        prefabShaders.Add(shader);
                        collectedShaders.Add(shader);
                    }
                }

                // 记录每个Prefab引用的Shader
                if (prefabShaders.Count > 0)
                {
                    prefabShaderMap[path] = prefabShaders;
                }
            }

            // 打印所有Prefab引用的Shader
            Debug.Log("=== Assets/ArtResources 下所有Prefab引用的Shader ===");
            if (prefabShaderMap.Count == 0)
            {
                Debug.Log("未找到任何Prefab引用的Shader");
            }
            else
            {
                foreach (var pair in prefabShaderMap)
                {
                    string prefabPath = pair.Key;
                    List<Shader> shaders = pair.Value;
                    Debug.Log($"Prefab: {prefabPath}");
                    foreach (Shader shader in shaders)
                    {
                        Debug.Log($"  Shader: {shader.name} {(alwaysIncludedShaders.Contains(shader) ? "(Always Included)" : "")}");
                    }
                }
            }

            // 过滤掉Always Included Shaders
            List<Shader> finalShaders = collectedShaders
                .Where(shader => !alwaysIncludedShaders.Contains(shader))
                .ToList();

            // 更新ShaderList
            Undo.RecordObject(shaderList, "Update ShaderList");
            shaderList.Shaders = finalShaders.ToArray();

            // 打印最终设置到ShaderList的Shader
            Debug.Log("=== 最终设置到ShaderList.prefab的Shader ===");
            if (finalShaders.Count == 0)
            {
                Debug.Log("没有非Always Included的Shader被设置到ShaderList");
            }
            else
            {
                foreach (Shader shader in finalShaders)
                {
                    Debug.Log($"Shader: {shader.name}");
                }
            }

            // 标记预制件为已修改
            PrefabUtility.SavePrefabAsset(shaderListPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"成功收集 {finalShaders.Count} 个Shader到ShaderList");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"收集Shader时发生错误: {ex.Message}");
        }
    }
}