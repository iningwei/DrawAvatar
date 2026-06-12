using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.Info;
using ZGame.RessEditor;

public class BuildInCompRawImageCollection : CompResCollection, IRefResCollection
{
    //建议RawImage处不使用Sprite类型图片资源
    //当然也可以使用，但是会跳过资源依赖处理

    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var rawImgChilds = new List<RawImage>();
        obj.GetComponentsInChildren(true, rawImgChilds);

        for (int i = 0; i < rawImgChilds.Count; i++)
        {
            RawImage rawImg = rawImgChilds[i];
            if (rawImg.texture == null || rawImg.material == null)
            {
                Debug.LogWarning("warning,rawImage:" + rawImg.transform.GetHierarchy() + " has no texture or mat");
                continue;
            }
            var texType = (TextureImporterType)AssetDatabaseExt.GetTextureImporterType(rawImg.texture);

            if (texType != TextureImporterType.Default)
            {
                Debug.LogWarning("warning,rawImage:" + rawImg.transform.GetHierarchy() + "'s texture is not defaultTexture type,so ignore res collection");
                continue;
            }


            //compInfos add elements
            string texName = rawImg.texture.name;
            CompInfo buildInCompRawImgInfo = new BuildInCompRawImageInfo(rawImg.transform, rawImg, texName, "", 0, rawImg.material.name, rawImg.material.shader.name);
            compInfos.Add(buildInCompRawImgInfo);
        }
        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();

        var rawImgChilds = new List<RawImage>();
        obj.GetComponentsInChildren(true, rawImgChilds);
        Debug.Log(obj.name + "'s RawImage count:" + rawImgChilds.Count);

        for (int i = 0; i < rawImgChilds.Count; i++)
        {
            RawImage rawImg = rawImgChilds[i];
            if (rawImg.texture == null || rawImg.material == null)
            {
                Debug.LogWarning("warning,rawImage:" + rawImg.transform.GetHierarchy() + " has no texture or mat");
                continue;
            }
            string texPath = AssetDatabase.GetAssetPath(rawImg.texture);
            var texType = (TextureImporterType)AssetDatabaseExt.GetTextureImporterType(texPath);
            if (texType != TextureImporterType.Default)
            {
                Debug.LogWarning("warning,rawImage:" + rawImg.transform.GetHierarchy() + "'s texture is not defaultTexture type,so ignore bundle build");
                continue;
            }

            string texName = rawImg.texture.name;

            if (texPath.Contains("Assets/UI"))
            {
                Debug.LogError("do not use tex in Assets/UI, path:" + rawImg.transform.GetHierarchy());
                continue;
            }

            //buildMap add tex element
            this.AddBundleBuildData(texName, texPath, ZGame.Ress.AB.ABType.Texture, ref buildMap);

            //buildMap add mat element
            this.AddBundleBuildData(rawImg.material.name, AssetDatabase.GetAssetPath(rawImg.material), ZGame.Ress.AB.ABType.Material, ref buildMap);
        }


        return buildMap;
    }


}
