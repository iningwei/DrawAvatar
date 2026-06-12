using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Debug = UnityEngine.Debug;
using ZGame.Res;


//TODO：
//1，本脚本只使用了默认平行光，对于那些需要诸如点光源的变体就无法收集到，考虑后续引入点光源或者其它更多条件
//2，增加不需要的变体删除，比如若项目未使用阴影、烘焙，那么可以移除所有相关变体
//3，该方法收集到的变体是不确定的，时多时少。网上有朋友说需要先清理 Library/ShaderCache 目录，再第一次收集的话就是对的。（这个说法待测试）
//4，后续针对具体Material，动态设置其keyword，避免使用了shader_feature导致变体未收集到

//注意：由于通过代码设置了 EditorApplication.isPlaying=ture，需要查看详细日志的话，在编辑中需要取消 Clear on Play的选项。否则的话在运行时会清除日志
//Origin:https://github.com/networm/ShaderVariantCollectionExporter
//扩展： 一种Shader变体收集和打包编译优化的思路 https://github.com/lujian101/ShaderVariantCollector
[InitializeOnLoad]
public static class ShaderVariantCollectionExporter
{

    static List<string> ignoreShaderNames = new List<string>() { "Shader Graphs/PhysicalMaterial3DsMax", "Custom/Flop", "My/Legacy Shaders/Diffuse", "My/Standard", "My/Standard (Specular setup)", "My/Legacy Shaders/Transparent/Diffuse" };


    [MenuItem("工具/Shader/Export ShaderVariantCollection")]
    private static void Export()
    {
        var dict = new Dictionary<string, bool>();

        string[] allAssetBundleNames = AssetDatabase.GetAllAssetBundleNames();
        foreach (var assetBundleName in allAssetBundleNames)
        {
            string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName);
            foreach (var dependency in AssetDatabase.GetDependencies(assetPaths, true))
            {
                if (!dict.ContainsKey(dependency))
                {
                    dict.Add(dependency, true);
                }
            }
        }

        var di = new DirectoryInfo("Assets");
        foreach (var fi in di.GetFiles("*", SearchOption.AllDirectories))
        {
            if (fi.Extension == ".meta")
            {
                continue;
            }

            string assetPath = fi.FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets");


            foreach (var dependency in AssetDatabase.GetDependencies(assetPath, true))
            {
                if (!dict.ContainsKey(dependency))
                {
                    dict.Add(dependency, true);
                }
            }
        }

        string[] scenes = (from scene in EditorBuildSettings.scenes
                           where scene.enabled
                           select scene.path).ToArray();
        foreach (var dependency in AssetDatabase.GetDependencies(scenes, true))
        {
            if (!dict.ContainsKey(dependency))
            {
                dict.Add(dependency, true);
            }
        }

        //加载shaderlist
        List<string> usedShaderList = new List<string>();
        var shaderListObj = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ArtResources/ShaderList/ShaderList.prefab");
        Shader[] shaderArray = shaderListObj?.GetComponent<ShaderList>().Shaders;
        if (shaderArray != null)
        {
            for (int i = 0; i < shaderArray.Length; i++)
            {
                usedShaderList.Add(shaderArray[i].name);
            }
        }


        //TODO:是否要把几个内置的加进去？比如天空盒等等

        var materials = new List<Material>();
        var shaderDict = new Dictionary<Shader, List<Material>>();
        foreach (var assetPath in dict.Keys)
        {
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            if (material != null && material.shader != null)
            {
                if (ignoreShaderNames.Contains(material.shader.name))
                {
                    Debug.LogError($"mat {assetPath} ignore,becase it's shader name is:" + material.shader.name);
                    continue;
                }
                if (material.shader != null)//&& usedShaderList.Contains(material.shader.name)
                {
                    if (!shaderDict.ContainsKey(material.shader))
                    {
                        shaderDict.Add(material.shader, new List<Material>());
                    }

                    if (!shaderDict[material.shader].Contains(material))
                    {
                        shaderDict[material.shader].Add(material);
                        Debug.Log("shader:" + material.shader.name + ", attached mat:" + assetPath);
                    }


                    if (!materials.Contains(material))
                    {
                        materials.Add(material);
                    }
                }


            }
        }

        ProcessMaterials(materials);

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("Summary-----click to see detail---->");
        foreach (var kvp in shaderDict)
        {
            sb.AppendLine(kvp.Key + " " + kvp.Value.Count + " times");


            //Debug.LogWarning("Shader: " + kvp.Key.name, kvp.Key);

            //foreach (var m in kvp.Value)
            //{
            //    Debug.Log(AssetDatabase.GetAssetPath(m), m);
            //}

        }

        Debug.Log(sb.ToString());
    }

    static ShaderVariantCollectionExporter()
    {
        EditorApplication.update += EditorUpdate;
    }

    private static void EditorUpdate()
    {


        if (_isStarted)
        {
            EditorUtility.DisplayProgressBar("ShaderVariantCollection", "Try collect!", _elapsedTime.ElapsedMilliseconds / WaitTimeBeforeSave);

            if (_elapsedTime.ElapsedMilliseconds >= WaitTimeBeforeSave)
            {

                var getCurrentShaderVariantCollectionVariantCount = InvokeInternalStaticMethod(typeof(ShaderUtil),
                "GetCurrentShaderVariantCollectionVariantCount");
                Debug.Log("getCurrentShaderVariantCollection VariantCount:" + getCurrentShaderVariantCollectionVariantCount);


                var getCurrentShaderVariantCollectionShaderCount = InvokeInternalStaticMethod(typeof(ShaderUtil), "GetCurrentShaderVariantCollectionShaderCount");
                Debug.Log("getCurrentShaderVariantCollection ShaderCount:" + getCurrentShaderVariantCollectionShaderCount);

                InvokeInternalStaticMethod(typeof(ShaderUtil), "SaveCurrentShaderVariantCollection",
               ShaderVariantCollectionPath);
                Debug.Log("shaderVariant file output path:" + ShaderVariantCollectionPath);

                EditorUtility.ClearProgressBar();

                Debug.Log("set isPlaying false");
                EditorApplication.isPlaying = false;


                _elapsedTime.Stop();
                _elapsedTime.Reset();
                _isStarted = false;
            }

        }

    }

    private static void ProcessMaterials(List<Material> materials)
    {
        Debug.Log("process materials 1:" + materials.Count);
        //创建新场景（默认场景中带了平行光的，就不需要额外创建了）
        EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects);
        InvokeInternalStaticMethod(typeof(ShaderUtil), "ClearCurrentShaderVariantCollection");
        var getCurrentShaderVariantCollectionShaderCount = InvokeInternalStaticMethod(typeof(ShaderUtil), "GetCurrentShaderVariantCollectionShaderCount");
        Debug.Log("getCurrentShaderVariantCollectionShaderCount:" + getCurrentShaderVariantCollectionShaderCount);

        int matCount = materials.Count;
        Debug.Log("process materials 2:" + matCount);
        var camera = Camera.main;
        if (camera == null)
        {
            Debug.LogError("Main Camera didn't exist");
            return;
        }

        float aspect = camera.aspect;
        //根据材质球的数量设置相机参数，使可以完全展示所有材质球
        float height = Mathf.Sqrt(matCount / aspect) + 1;
        float width = Mathf.Sqrt(matCount / aspect) * aspect + 1;

        float halfHeight = Mathf.CeilToInt(height / 2f);
        float halfWidth = Mathf.CeilToInt(width / 2f);

        camera.orthographic = true;
        camera.orthographicSize = halfHeight;
        camera.transform.position = new Vector3(0f, 0f, -10f);

        Selection.activeGameObject = camera.gameObject;
        EditorApplication.ExecuteMenuItem("GameObject/Align View to Selected");

        int xMax = (int)(width - 1);

        int x = 0;
        int y = 0;

        for (int i = 0; i < materials.Count; i++)
        {
            var material = materials[i];

            var position = new Vector3(x - halfWidth + 1f, y - halfHeight + 1f, 0f);
            CreateSphere(material, position, x, y, i);

            if (x == xMax)
            {
                x = 0;
                y++;
            }
            else
            {
                x++;
            }
        }

        _elapsedTime.Stop();
        _elapsedTime.Reset();
        _elapsedTime.Start();
        _isStarted = true;

        //EditorApplication.isPlaying = true;
    }

    private static void CreateSphere(Material material, Vector3 position, int x, int y, int index)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.GetComponent<Renderer>().material = material;
        go.transform.position = position;
        go.name = string.Format("Sphere_{0}|{1}_{2}|{3}", index, x, y, material.name);


    }

    private static object InvokeInternalStaticMethod(System.Type type, string method, params object[] parameters)
    {
        var methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
        if (methodInfo == null)
        {
            Debug.LogError(string.Format("{0} method didn't exist", method));
            return null;
        }

        return methodInfo.Invoke(null, parameters);
    }

    private static bool _isStarted;
    private static readonly Stopwatch _elapsedTime = new Stopwatch();

    private const string ShaderVariantCollectionPath = "Assets/ShaderVariantCollection.shadervariants";
    private const int WaitTimeBeforeSave = 5000;
}