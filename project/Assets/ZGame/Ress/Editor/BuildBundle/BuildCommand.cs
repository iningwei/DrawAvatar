using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Obfuscation;
using ZGame.Res;
using ZGame.Ress.AB.Holder;
using ZGame.Ress.Info;

namespace ZGame.RessEditor
{
    public class BuildCommand
    {

        [MenuItem("工具/打包/打ab包/一键打所有资源ab包")]
        public static void OneKeyBuild()
        {
            if (EditorUtility.DisplayDialog("警告", "确定要一键打所有资源ab包吗？", "OK", "Cancel"))
            {
                BuildConfig.Init();
                if (!CheckCommonRes())
                {
                    return;
                }

                //打音效
                var audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/ArtResources/Audio" });
                for (int i = 0; i < audioGuids.Length; i++)
                {
                    string guid = audioGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    AudioClip asset = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
                }


                //打预制件
                var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
                for (int i = 0; i < prefabGuids.Length; i++)
                {
                    string guid = prefabGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    if (path.Contains("temp_for_prefab"))
                    {
                        continue;
                    }


                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);


                    GameObject asset = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
                    if (asset != null)
                    {
                        var holder = asset.GetComponent<RootCompInfoHolder>();
                        if (holder != null)
                        {
                            BuildConfig.getBuildFunc(path, asset)(asset);
                        }
                    }

                }

                //打图片（虽然预制件对图片有进行引用，但仍有图集或者图片没有通过预制件直接引用）
                var spriteGuids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/ArtResources/Sprite" });
                for (int i = 0; i < spriteGuids.Length; i++)
                {
                    string guid = spriteGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);

                    //Debug.Log("path:" + path);
                    //过滤掉一些当前版本没用到的图片资源
                    if (fileName == "textmeshpro-imgs"
                        || fileName == "battlepetshow_2"

                        || fileName == "task")
                    {
                        continue;
                    }
                    Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
                }


                var textureGuids = AssetDatabase.FindAssets("t:Texture", new[] { "Assets/ArtResources/Texture/BattleBoxingShown" });
                for (int i = 0; i < textureGuids.Length; i++)
                {
                    string guid = textureGuids[i];
                    string path = AssetDatabase.GUIDToAssetPath(guid);

                    string allPath = Application.dataPath + path.Replace("Assets", "");
                    string fileName = Path.GetFileNameWithoutExtension(allPath);

                    if (fileName == "battle_show_bast"
                        || fileName == "battle_show_corgi"
                       )
                    {
                        continue;
                    }
                    Texture asset = AssetDatabase.LoadAssetAtPath<Texture>(path);
                    BuildConfig.getBuildFunc(path, asset)(asset);
                }


                // 完成后要删除一些无用的文件
                DeleteUselessAfterBuildAB();

                Debug.Log("一键打所有资源ab包    完毕");
            }
            else
            {
                Debug.LogError("取消打包");
            }

        }



        [MenuItem("Assets/ZGame/对选择项打AB包")]
        public static void Build()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            for (int i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[i]);
                BuildConfig.getBuildFunc(path, assets[i])(assets[i]);
            }

            DeleteUselessAfterBuildAB();
            Debug.Log("finished bundle build!");
        }


        [MenuItem("Assets/ZGame/对选择文件夹内所有预制件添加RootCompInfoHolder，并设置类型为OtherPrefab")]
        public static void SetPrefabRootCompInfoHolder()
        {
            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            Debug.Log("count:" + allAssetPathList.Count);

            if (allAssetPathList.Count > 0)
            {
                foreach (var path in allAssetPathList)
                {
                    string assetPath = path.Substring(path.IndexOf("Assets"));
                    GameObject asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
                    var holder = asset.GetOrAddComponent<RootCompInfoHolder>();
                    holder.abType = Ress.AB.ABType.OtherPrefab;

                    EditorUtility.SetDirty(asset);
                    AssetDatabase.SaveAssets(); //保存
                }
            }

        }

        [MenuItem("Assets/ZGame/对选择文件夹内所有预制件打AB包(类型为OtherPrefab)")]
        public static void BuildPrefabAB()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            foreach (var path in allAssetPathList)
            {
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                new BuildOtherPrefab().Build(asset);
            }

            DeleteUselessAfterBuildAB();
        }


        [MenuItem("Assets/ZGame/对选择文件夹内所有光照贴图打ab")]
        public static void BuildLightMap()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.hdr");
            foreach (var path in allAssetPathList)
            {
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Texture));
                new BuildTexture().Build(asset);
            }

            DeleteUselessAfterBuildAB();
        }

        [MenuItem("Assets/ZGame/对选择文件夹内所有窗体打ab")]
        public static void BuildWindow()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }

            var allAssetPathList = GetAllTargetFilesFormSelectFolder("*.prefab");
            foreach (var path in allAssetPathList)
            {
                if (path.EndsWith("Window.prefab") == false)
                {
                    continue;
                }
                string assetPath = path.Substring(path.IndexOf("Assets"));
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject));
                new BuildWindow().Build(asset);
            }

            DeleteUselessAfterBuildAB();
        }


        static List<string> GetAllTargetFilesFormSelectFolder(string _extension)
        {
            // 获取所有选中 文件、文件夹的 GUID
            string[] guids = Selection.assetGUIDs;
            List<string> allAssetPathList = new List<string>();
            foreach (var guid in guids)
            {
                // 将 GUID 转换为 路径
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                // 判断是否文件夹
                if (Directory.Exists(assetPath))
                {
                    searchDirectory(assetPath, _extension, ref allAssetPathList);
                }
            }
            return allAssetPathList;
        }

        static void searchDirectory(string directory, string _extension, ref List<string> outputList)
        {
            DirectoryInfo dInfo = new DirectoryInfo(directory);
            // 获取 文件夹以及子文件加中所有扩展名为  _extension 的文件
            FileInfo[] fileInfoArr = dInfo.GetFiles(_extension, SearchOption.AllDirectories);
            for (int i = 0; i < fileInfoArr.Length; ++i)
            {
                string fullName = fileInfoArr[i].FullName;
                Debug.Log(fullName);
                outputList.Add(fullName);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool BuildLightMapTexAB(string path)
        {
            var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Texture));
            return new BuildTexture().Build(asset);
        }

        public static bool BuildBytes(string path)
        {
            Debug.Log("----build bytes path:" + path);
            string assetPath = path.Substring(path.IndexOf("Assets"));
            Debug.Log("----build bytes assetPath:" + assetPath);
            var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(TextAsset));
            return new BuildByte().Build(asset);
        }

        //由于场景打AB会出现rootcompinfo信息丢失，故提供一种分步打ab的方式。
        [MenuItem("Assets/ZGame/对场景打AB/step1-->收集场景RootCompInfo")]
        public static void CollectSceneRootCompInfo()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }


            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            if (assets != null)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[0]);
                if (path.Contains(".unity") == false)
                {
                    Debug.LogError("error,your select is not a scene");
                    return;
                }
            }
            if (assets.Length > 1)
            {
                Debug.LogError("Only support select one scene!");
                return;
            }

            new BuildScene().GetSceneRootCompInfo(assets[0]);



        }
        [MenuItem("Assets/ZGame/对场景打AB/step2-->打AB")]
        public static void BuildSceneAB()
        {
            BuildConfig.Init();
            if (!CheckCommonRes())
            {
                return;
            }


            UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
            if (assets != null)
            {
                var path = AssetDatabase.GetAssetOrScenePath(assets[0]);
                if (path.Contains(".unity") == false)
                {
                    Debug.LogError("error,your select is not a scene");
                    return;
                }
            }
            if (assets.Length > 1)
            {
                Debug.LogError("Only support select one scene!");
                return;
            }

            new BuildScene().BuildSceneAB(assets[0]);

            DeleteUselessAfterBuildAB();
        }


        [MenuItem("工具/打包/打ab包/删除无用文件")]
        public static void DeleteUselessAfterBuildAB()
        {
            // 完成后要删除一些无用的文件（manifest、meta等文件）
            var files = Directory.GetFileSystemEntries(BuildConfig.outputPath);
            foreach (var v in files)
            {
                if (!v.EndsWith(IOTools.abSuffix))
                {
                    File.Delete(v);
                }
                else if (Path.GetFileNameWithoutExtension(v).StartsWith("fbx_"))//删除临时生成的fbx对应的AB
                {
                    File.Delete(v);
                }
            }
        }


        static AssetBundleBuild commonResMap;
        static List<Shader> shaders;
        //check common res
        public static bool CheckCommonRes()
        {
            Debug.Log("CheckCommonRes!!!");
            commonResMap = new AssetBundleBuild();
            commonResMap.assetBundleName = "common";
            commonResMap.assetBundleVariant = IOTools.abSuffixWithoutPoint;

            List<string> assetPaths = new List<string>();

            //add shader
            string shaderListPath = "Assets/ArtResources/ShaderList/ShaderList.prefab";
            var shaderList = AssetDatabase.LoadAssetAtPath(shaderListPath, typeof(GameObject)) as GameObject;
            if (shaderList == null)
            {
                Debug.LogError("error," + shaderListPath);
                return false;
            }

            shaders = new List<Shader>(shaderList.GetComponent<ShaderList>().Shaders);
            Debug.Log("shaderList shader count:" + shaders.Count);
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i] == null)
                {
                    Debug.LogError("shaderlist have null shader");
                    return false;
                }

                string path = AssetDatabase.GetAssetPath(shaders[i]);
                if (path == null)
                {
                    Debug.LogError("该shader不在工程内,shader name:" + shaders[i].name);
                    return false;
                }
                if (!path.Contains("Assets"))
                {
                    Debug.LogWarning("warnning,该shader不在Assets目录下，path:" + path);
                }

                assetPaths.Add(path);

            }

            //添加shader 变体资源
            string shaderVariantsPath = "Assets/ArtResources/ShaderList/ShaderVariants.shadervariants";
            if (File.Exists(Application.dataPath + shaderVariantsPath.Replace("Assets", "")))
            {
                assetPaths.Add(shaderVariantsPath);
            }

            //添加字体资源 
            for (int i = 0; i < BuildConfig.fontPathList.Count; i++)
            {
                if (!File.Exists(Application.dataPath + BuildConfig.fontPathList[i].Replace("Assets", "")))
                {
                    Debug.LogError("Error, specific font not exist：" + BuildConfig.fontPathList[i]);
                    return false;
                }
                else
                {
                    assetPaths.Add(BuildConfig.fontPathList[i]);
                }
            }


            commonResMap.assetNames = assetPaths.ToArray();
            return true;
        }

        public static AssetBundleBuild GetCommonMap()
        {
            return commonResMap;
        }


        /// <summary>
        /// 对Renderer组件进行合法性检测
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckValidOfRenderer(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfRenderer:" + targetHierarchyPath);
            var renderers = target.GetComponents<Renderer>();//一个obj上可能会挂载多个renderer组件。比如挂载LineRenderer和SpriteRenderer


            if (renderers != null && renderers.Length > 0)
            {
                for (int i = 0; i < renderers.Length; i++)
                {
                    var render = renderers[i];
                    var mats = render.sharedMaterials;
                    for (int j = 0; j < mats.Length; j++)
                    {
                        var mat = mats[j];
                        if (mat != null)
                        {
                            if (BuildConfig.buildInMaterialNames.Contains(mat.name))
                            {
                                Debug.LogError("使用了默认材质球， mat.name:" + mat.name + ", objPath:" + render.transform.GetHierarchy());
                                return false;
                            }

                            if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                            {
                                return false;
                            }
                        }
                    }
                }

            }

            return true;
        }



        /// <summary>
        /// 对Unity2D的组件进行合法性检测
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool CheckValidOfU2D(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfU2D:" + targetHierarchyPath);

            SpriteRenderer sr = target.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                if (sr.sharedMaterial != null && !isValidMatOrShaderOfU2D(target, sr.sharedMaterial))
                {
                    return false;
                }

            }
            return true;
        }





        /// <summary>
        /// 是否使用了默认材质球或者shader
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        static bool isValidMatOrShaderOfU2D(GameObject target, Material mat)
        {
            if (mat.name == "Sprites-Default")//使用了Unity2D的默认材质球
            {
                Debug.LogError("使用了U2D默认材质球，" + target.name);

                return false;
            }
            else
            {
                if (mat.shader == null)
                {
                    Debug.LogError("材质球无shader," + target.name);
                    return false;
                }
                if (mat.shader.name == "Sprites/Default")//使用了U2D的默认shader
                {
                    Debug.LogError("材质球使用了U2D默认shader," + target.name);
                    return false;
                }


                if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                {
                    return false;
                }
            }

            return true;
        }



        static bool isShaderInList(string shaderName, string objHirarchy)
        {
            if (shaders.Find(a => a.name == shaderName) != null)
            {
                return true;
            }

            //////if (shaderName == "GUI/Text Shader")//TODO:TextMesh组件使用的MeshRender
            //////{
            //////    return true;
            //////}
            ///

            //if (shaderName == "Spine/Skeleton")//TODO:暂时spine骨骼还没有做shader、图片等拆分
            //{
            //    return true;
            //}
            Debug.LogError("使用的shader没有加入shaderList中，shader:" + shaderName + ",      objPath:" + objHirarchy);
            return false;
        }



        /// <summary>
        /// 对UGUI 的合法性进行检测
        /// </summary>
        /// <param name="target"></param>  
        /// <returns></returns>
        public static bool CheckValidOfUGUI(GameObject target)
        {
            string targetHierarchyPath = getHierarchyPath(target.transform);
            Debug.Log("CheckValidOfUGUI：" + targetHierarchyPath);


            Image img = target.GetComponent<Image>();
            if (img != null)//图片
            {
                if (!isValidImageOfUGUI(img))
                {
                    return false;
                }
                if (!isValidMatOrShaderOfUGUI(target, img.material))
                {
                    return false;
                }
            }

            RawImage rawImage = target.GetComponent<RawImage>();
            if (rawImage)
            {
                if (!isValidRawImageOfUGUI(rawImage))
                {
                    return false;
                }
                if (!isValidMatOrShaderOfUGUI(target, rawImage.material))
                {
                    return false;
                }
            }

            Text text = target.GetComponent<Text>();
            if (text != null)//文字
            {
                if (!isValidMatOrShaderOfUGUI(target, text.material))
                {
                    return false;
                }

                if (!isValidTextOfUGUI(text))
                {
                    return false;
                }
            }


            return true;
        }

        public static void FillRootCompInfoHolder(GameObject obj)
        {
            CompInfoHolder holder = obj.GetComponent<CompInfoHolder>();
            if (holder == null)
            {
                Debug.LogError("error, " + obj.GetHierarchy() + ", have no CompInfoHolder Component");
                return;
            }
            fillCompInfoHolder(obj, holder);
        }

        public static void FillDynamicCompInfoHolder(GameObject obj)
        {
            List<GameObject> childs = obj.GetChilds(false, true);
            for (int i = 0; i < childs.Count; i++)
            {
                GameObject child = childs[i];
                var dynamicInfoHolder = child.GetComponent<DynamicCompInfoHolder>();
                if (dynamicInfoHolder != null)
                {
                    fillCompInfoHolder(child, dynamicInfoHolder);
                }
            }
        }

        static void fillCompInfoHolder(GameObject obj, CompInfoHolder infoHolder)
        {
            //////infoHolder.buildInCompImageInfos = GenericExt.ParseList<BuildInCompImageInfo>(new BuildInCompImageCollection().GetCompInfo(obj));

            //////infoHolder.buildInCompSpriteRendererInfos = GenericExt.ParseList<BuildInCompSpriteRendererInfo>(new BuildInCompSpriteRendererCollection().GetCompInfo(obj));
            //////infoHolder.buildInCompTextInfos = GenericExt.ParseList<BuildInCompTextInfo>(new BuildInCompTextCollection().GetCompInfo(obj));
            //////infoHolder.buildInCompRendererInfos = GenericExt.ParseList<BuildInCompRendererInfo>(new BuildInCompRendererCollection().GetCompInfo(obj));

            //////infoHolder.extCompImageSequenceInfos = GenericExt.ParseList<ExtCompImageSequenceInfo>(new ExtCompImageSequenceCollection().GetCompInfo(obj));
            //////infoHolder.extCompSpriteSequenceInfos = GenericExt.ParseList<ExtCompSpriteSequenceInfo>(new ExtCompSpriteSequenceCollection().GetCompInfo(obj));
            //////infoHolder.extCompMaterialTextureSequenceInfos = GenericExt.ParseList<ExtCompMaterialTextureSequenceInfo>(new ExtCompMaterialTextureSequenceCollection().GetCompInfo(obj));

            //////infoHolder.extCompSwapSpriteInfos = GenericExt.ParseList<ExtCompSwapSpriteInfo>(new ExtCompSwapSpriteCollection().GetCompInfo(obj));
            //////infoHolder.extCompSwitch2ButtonInfos = GenericExt.ParseList<ExtCompSwitch2ButtonInfo>(new ExtCompSwitch2ButtonCollection().GetCompInfo(obj));



            if (infoHolder is RootCompInfoHolder)
            {
                var abType = (infoHolder as RootCompInfoHolder).abType;
                if (abType == Ress.AB.ABType.Scene)
                {
                    //////Debug.LogError("LightmapInfoCollection@");

                    infoHolder.lightmapInfo = new LightmapInfoCollection().GetCompInfo(obj);
                }
            }


            Type t = infoHolder.GetType();
            var fis = t.GetFields();
            foreach (var fi in fis)
            {
                Type ft = fi.FieldType;

                if (ft.IsPublic && ft.IsGenericType)
                {
                    Type[] genericAs = ft.GetGenericArguments();//get fields's generic params
                    object[] atts = fi.GetCustomAttributes(typeof(CompInfoRefCollectionAttribute), true);
                    if (atts != null && atts.Length > 0)
                    {
                        string collectionClassName = ((CompInfoRefCollectionAttribute)atts[0]).refCollectionClassName;

                        Type collectionT = Type.GetType(collectionClassName);
                        object target = Activator.CreateInstance(collectionT);
                        MethodInfo mi = collectionT.GetMethod("GetCompInfo");
                        var compInfoResult = mi.Invoke(target, new object[] { obj });
                        List<CompInfo> ciList = null;
                        if (compInfoResult != null)
                        {
                            ciList = compInfoResult as List<CompInfo>;
                        }


                        Type genericExtType = typeof(GenericExt); //or  Type.GetType("GenericExt");
                        MethodInfo parseListMI = genericExtType.GetMethod("ParseList");
                        MethodInfo info = parseListMI.MakeGenericMethod(genericAs[0]);//对普通方法封装为对应类型的泛型方法，再使用Invoke进行调用
                        var result = info.Invoke(genericExtType, new object[] { ciList });

                        fi.SetValue(infoHolder, result);
                    }

                }

            }
        }

        public static Dictionary<string, AssetBundleBuild> GetGameObjectAssetBundleBuildMap(GameObject obj)
        {
            Dictionary<string, AssetBundleBuild> texMap = new Dictionary<string, AssetBundleBuild>();

            /*
            //////BuildInCompImageCollection buildInCompImageCollection = new BuildInCompImageCollection();
            //////texMap.AddRange(buildInCompImageCollection.GetResMap(obj));
            */


            //反射获得所有实现了IRefResCollection的类 
            Assembly ass = Assembly.GetAssembly(typeof(IRefResCollection));
            Type[] types = ass.GetTypes();
            foreach (var item in types)
            {
                if (item.IsInterface)
                {
                    continue;//跳过接口自己
                }

                Type[] ins = item.GetInterfaces();
                foreach (var ty in ins)
                {
                    if (ty == typeof(IRefResCollection))
                    {
                        //Debug.LogError("xxx:" + item.ToString());                        
                        object target = Activator.CreateInstance(item);
                        MethodInfo getResMap = item.GetMethod("GetResMap");
                        var result = getResMap.Invoke(target, new object[] { obj });
                        if (result != null)
                        {
                            var tmpMap = result as Dictionary<string, AssetBundleBuild>;

                            //去重
                            foreach (var tmpItem in tmpMap)
                            {
                                if (texMap.ContainsKey(tmpItem.Key) == false)
                                {
                                    texMap.Add(tmpItem.Key, tmpItem.Value);
                                }
                            }
                        }
                    }
                }

            }

            //获得Lightmap中的 tex
            LightmapInfoCollection lightmapInfoCollection = new LightmapInfoCollection();
            //////texMap.AddRange(lightmapInfoCollection.GetResMap(obj));
            var items = lightmapInfoCollection.GetResMap(obj);
            foreach (var item in items)
            {
                texMap.Add(item.Key, item.Value);
            }
            return texMap;
        }


        static bool isValidTextOfUGUI(Text text)
        {
            if (text.font == null)
            {
                Debug.LogError("未指定字体，" + text.gameObject);
                return false;
            }
            else
            {
                if (text.font.name == "Arial")
                {
                    Debug.LogError("使用了默认Arial字体，" + text.gameObject);
                    return false;
                }
            }
            return true;
        }

        static bool isValidImageOfUGUI(Image img)
        {
            if (img.sprite == null)
            {
                Debug.LogWarning("warning, image have no sprite attached:" + img.transform.GetHierarchy());
                //only warning
            }
            else
            {
                if (BuildConfig.buildInImageSpriteNames.Contains(img.sprite.name))
                {
                    Debug.LogError("使用了内置贴图," + img.name + ", 贴图名：" + img.sprite.name + "， path:" + img.transform.GetHierarchy());
                    return false;
                }
            }
            return true;
        }

        static bool isValidRawImageOfUGUI(RawImage rawImg)
        {
            if (rawImg.texture == null)
            {
                Debug.LogWarning("warning, rawImage have no texture attached:" + rawImg.transform.GetHierarchy());
                //only warning
            }
            else
            {
                //未使用texture格式贴图
                var texType = AssetDatabaseExt.GetTextureImporterType(rawImg.texture);
                if (texType != TextureImporterType.Default)
                {
                    Debug.LogWarning("warning,rawImage:" + rawImg.transform.GetHierarchy() + "'s texture is not defaultTexture type ");
                }
            }
            return true;
        }


        /// <summary>
        /// 是否使用了默认材质球或者shader
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        static bool isValidMatOrShaderOfUGUI(GameObject target, Material mat)
        {

            if (BuildConfig.buildInMaterialNames.Contains(mat.name))
            {
                Debug.LogError("used   default mat: " + mat.name + ", path:" + target.GetHierarchy());
                return false;
            }
            else
            {
                if (mat.shader == null)
                {
                    Debug.LogError("mat have no shader: " + target.GetHierarchy());
                    return false;
                }

                if (isShaderInList(mat.shader.name, getHierarchyPath(target.transform)) == false)
                {
                    return false;
                }
            }

            return true;
        }

        static string getHierarchyPath(Transform target)
        {
            string path = target.name;
            Transform sun = target;
            while (sun.transform.parent != null)
            {
                sun = sun.transform.parent;
                path = sun.name + "/" + path;
            }

            return path;
        }
    }
}