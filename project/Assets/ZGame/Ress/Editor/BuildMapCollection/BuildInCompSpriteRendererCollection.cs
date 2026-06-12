using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class BuildInCompSpriteRendererCollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();


        var spriteRendererChilds = new List<SpriteRenderer>();
        obj.GetComponentsInChildren<SpriteRenderer>(true, spriteRendererChilds);


        for (int i = 0; i < spriteRendererChilds.Count; i++)
        {
            SpriteRenderer spriteRenderer = spriteRendererChilds[i];

            if (spriteRenderer.sprite == null || spriteRenderer.material == null)//sprite和material有一个为null,则属于异常情况。
            {
                continue;
            }

            //compInfos add element
            string atlasName = spriteRenderer.sprite.texture.name;
            string texName = spriteRenderer.sprite.name;
            SpriteInfo spriteInfo = new SpriteInfo(atlasName, texName);
            List<SpriteInfo> spriteInfos = new List<SpriteInfo>() { spriteInfo };
            CompInfo buildInCompImageInfo = new BuildInCompSpriteRendererInfo(
                spriteRenderer.transform,
                spriteRenderer,
                spriteInfos,
                "",
                spriteRenderer.material.name,
                spriteRenderer.material.shader.name
                );
            compInfos.Add(buildInCompImageInfo);
        }

        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();
        var spriteRendererChilds = new List<SpriteRenderer>();
        obj.GetComponentsInChildren<SpriteRenderer>(true, spriteRendererChilds);
        Debug.Log(obj.name + "'s SpriteRenderer count:" + spriteRendererChilds.Count);


        for (int i = 0; i < spriteRendererChilds.Count; i++)
        {
            SpriteRenderer spriteRenderer = spriteRendererChilds[i];

            if (spriteRenderer.sprite == null || spriteRenderer.material == null)//sprite和material有一个为null,则属于异常情况。                                                              
            {
                Debug.LogWarning("warning,SpriteRenderer:" + spriteRenderer.transform.GetHierarchy() + "  sprite not set or  material not set");
                continue;
            }



            string atlasName = spriteRenderer.sprite.texture.name;

            string texPath = AssetDatabase.GetAssetPath(spriteRenderer.sprite.texture);

            if (texPath.Contains("Assets/UI"))
            {
                Debug.LogError(" do not use tex in Assets/UI, path:" + spriteRenderer.gameObject.transform.GetHierarchy());
                continue;
            }

            //buildMap add tex element             
            this.AddBundleBuildData(atlasName, texPath, ABType.Sprite, ref buildMap);

            //buildMap add mat element
            this.AddBundleBuildData(spriteRenderer.material.name, AssetDatabase.GetAssetPath(spriteRenderer.material), ABType.Material, ref buildMap);
        }
        return buildMap;
    }
}
