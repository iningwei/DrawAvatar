using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AutoTextureSetting : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture2D)
    {

        //Debug.LogError("assetPath:" + assetPath);

        var importer = assetImporter as TextureImporter;
        //importer.alphaSource = TextureImporterAlphaSource.FromInput;
        //importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        // importer.SaveAndReimport();//修改了mipmapEnabled后，需要SaveAndReimport()进行保存，否则图片会变成未进行纹理压缩处理的样子。 

        //////importer.sRGBTexture = false;//线性空间需要使用sRGB
        importer.isReadable = false;
        importer.npotScale = TextureImporterNPOTScale.None;

        TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
        //androidSettings.maxTextureSize = 1024;
        androidSettings.format = TextureImporterFormat.ASTC_6x6;



        //newbie_guide图集，使用ASTC失真，特殊处理
        if (assetPath.Contains("newbie_guide.png"))
        {
            androidSettings.format = TextureImporterFormat.ETC2_RGBA8;
        }

        //navigation图集使用清晰度更高的格式
        if (assetPath.Contains("navigation.png"))
        {
            androidSettings.format = TextureImporterFormat.ASTC_4x4;
        }

        androidSettings.name = "Android";
        androidSettings.overridden = true;
        importer.SetPlatformTextureSettings(androidSettings);


        //ASTC 在ios上有通道问题
        TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings();
        //iosSettings.maxTextureSize = 1024;
        iosSettings.format = TextureImporterFormat.ASTC_6x6;
        if (assetPath.Contains("newbie_guide.png"))
        {
            iosSettings.format = TextureImporterFormat.PVRTC_RGBA4;
        }
        if (assetPath.Contains("pet_fragment1.png") || assetPath.Contains("pet_fragment2.png") || assetPath.Contains("pet_fragment3.png"))
        {
            iosSettings.format = TextureImporterFormat.RGBA32;
        }


        iosSettings.name = "iPhone";
        iosSettings.overridden = true;

        importer.SetPlatformTextureSettings(iosSettings);
    }

    void OnPreprocessAnimation()
    {

        ModelImporter modelImporter = assetImporter as ModelImporter;
        modelImporter.animationCompression = ModelImporterAnimationCompression.Optimal;



    }

    void OnPreprocessAudio()
    {
        //TODO:后续自动把双通道改成单通道
        //实测双通道改单通道，音质变化太大。作废

        //if (assetPath.Contains("mono"))
        //{

        //}

        //AudioImporter audioImporter = assetImporter as AudioImporter;
        //audioImporter.forceToMono = true;
    }
}
