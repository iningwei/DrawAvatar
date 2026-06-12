using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.AB.Holder;

namespace ZGame.RessEditor
{
    public enum TextType
    {
        Legacy,
        TMP,
    }
    /// <summary>
    /// AssetBundle打包配置类
    /// </summary>
    public class BuildConfig
    {
        public static BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle |
            BuildAssetBundleOptions.ChunkBasedCompression | BuildAssetBundleOptions.ForceRebuildAssetBundle;

        static string baseOutputPath = null;
        public static string outputPath
        {
            get
            {
                return baseOutputPath;
            }
        }



        public static List<string> buildInImageSpriteNames = new List<string>()
        {
            "UISprite","Background","Knob","UIMask","InputFieldBackground","DropdownArrow","Checkmark"
        };
        public static List<string> buildInMaterialNames = new List<string>()
        {
            "Default UI Material",//Image、RawImage、Text组件默认材质球
            "Sprites-Default",//SpriteRenderer组件默认材质球
            "Default-ParticleSystem",//BuiltIn管线下ParticleSystemRenderer组件默认材质球 
            "ParticlesUnlit",//URP管线下 ParticleSystemRenderer组件默认材质球
             
        };

        public static TextType textType = TextType.TMP;
        public static string defaultTextFontName = "有爱魔兽圆体-B";
        public static string defaultTextFontPath = "Assets/ArtResources/Font/有爱魔兽圆体-B.ttf";

        public static string defaultTMPTextFontName = "SourceHanSansSC-Medium SDF";
        public static string defaultTMPTextFontPath = "Assets/ArtResources/Font/SourceHanSansSC-Medium SDF.asset";

        //打AB时，不处理的material列表
        public static List<string> ignoredMats = new List<string>();

        public static List<string> fontPathList = new List<string>();

        static BuildConfig()
        {
            //字体资源
            //字体暂时不通过类似Shader的方式进行添加，统一在这里把需要的字体添加到list中，然后统一打进common bundle中
            //特别要注意，如果新用到了字体，需要在这里添加,因为暂时没有提供类似shader的 字体检测工具    
            //fontPathList.Add("Assets/ArtResources/Font/fzltthjt.ttf");
            //fontPathList.Add("Assets/ArtResources/Font/arial.ttf");
            //Textmeshpro sdf 文件
            fontPathList.Add("Assets/ArtResources/Font/SourceHanSansSC-Medium SDF.asset");
            fontPathList.Add("Assets/ArtResources/Font/CKTKingkong SDF.asset");
        }
        public static void Init()
        {
            ignoredMats.Clear();
            ignoredMats.Add("SourceHanSansSC-Medium Atlas Material");
            ignoredMats.Add("arial Atlas Material");



            //generator ab output path
            if (baseOutputPath == null)
            {
                baseOutputPath = Application.dataPath + "/../ResEx/" + IOTools.PlatformFolderName;
            }
            if (!Directory.Exists(baseOutputPath))
            {
                Directory.CreateDirectory(baseOutputPath);
            }

            Debug.Log("baseOutputPath:" + baseOutputPath);

            //generator temp path for prefab
            string tmpPath = Application.dataPath + "/temp_for_prefab";
            if (!Directory.Exists(tmpPath))
            {
                Directory.CreateDirectory(tmpPath);
            }
        }


        //资源配置项
        public class ResBuildConfig
        {
            public string rootPath;
            public Func<UnityEngine.Object, bool> buildFunc;
        }


        public static Dictionary<ABType, ResBuildConfig> allRes = new Dictionary<ABType, ResBuildConfig>()
    {
        //精灵
        {
            ABType.Sprite,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Sprite",
                buildFunc=new BuildSprite().Build
            }

        },
        //图片
        {
            ABType.Texture,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Texture",
                buildFunc=new BuildTexture().Build
            }
        },
                         
        //音效
        {
            ABType.Audio,
            new ResBuildConfig
            {
                rootPath="Assets/ArtResources/Audio",
                buildFunc=new BuildAudio().Build
            }
        }
    };

        /// <summary>
        /// 通过资源路径，获得其编译函数
        /// </summary>
        /// <param name="resPath"></param>
        /// <returns></returns>
        public static Func<UnityEngine.Object, bool> getBuildFunc(string resPath, object asset)
        {
            if (resPath.EndsWith("AnimationClipList.prefab"))
            {
                Debug.LogError("实质上并不会对AnimationClipList打AB，而是会找到附加的所有AnimationClip，并分别打AB");
                return new BuildAnimationClipList().Build;
            }
            //预制件要通过holder中的abType来确定
            if (resPath.EndsWith(".prefab"))
            {
                var holder = (asset as GameObject).GetComponent<RootCompInfoHolder>();
                if (holder == null)
                {
                    Debug.LogError("no RootCompInfoHolder attach to:" + resPath);
                    return null;
                }
                else
                {
                    if (holder.abType == ABType.Window)
                    {
                        return new BuildWindow().Build;
                    }
                    else if (holder.abType == ABType.Effect)
                    {
                        return new BuildEffect().Build;
                    }
                    else if (holder.abType == ABType.OtherPrefab)
                    {
                        return new BuildOtherPrefab().Build;
                    }
                    else
                    {
                        Debug.LogError("please set correct abType, cur abType:" + holder.abType.ToString());
                        return null;
                    }
                }
            }


            //二进制文件 
            if (resPath.EndsWith(".bytes"))
            {
                return new BuildByte().Build;
            }
            // .asset文件，诸如NodeCanvas的行为树资源
            if (resPath.EndsWith(".asset"))
            {
                return new BuildObject().Build;
            }
            //场景
            if (resPath.EndsWith(".unity"))
            {
                return new BuildScene().Build;
            }
            //材质球
            if (resPath.EndsWith(".mat"))
            {
                return new BuildMat().Build;
            }
            //光照贴图
            if (resPath.EndsWith(".hdr"))
            {
                return new BuildTexture().Build;
            }

            foreach (var item in allRes)
            {
                if (resPath.Contains(item.Value.rootPath))
                {
                    return item.Value.buildFunc;
                }

                if (resPath.Contains("Sprite"))
                {
                    return new BuildSprite().Build;
                }
                if (resPath.Contains("Texture"))
                {
                    return new BuildTexture().Build;
                }
                if (resPath.Contains("Audio"))
                {
                    return new BuildAudio().Build;
                }
            }

            return new BuildBase().Build;
        }
    }
}