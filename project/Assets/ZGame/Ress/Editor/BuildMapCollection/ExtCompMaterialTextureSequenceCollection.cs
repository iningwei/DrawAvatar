using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;

public class ExtCompMaterialTextureSequenceCollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        var mtSequenceChilds = new List<MaterialTextureSequence>();
        obj.GetComponentsInChildren<MaterialTextureSequence>(true, mtSequenceChilds);

        for (int i = 0; i < mtSequenceChilds.Count; i++)
        {
            MaterialTextureSequence mtSequence = mtSequenceChilds[i];

            if (mtSequence.textures == null || mtSequence.textures.Length == 0)//异常情况。
            {
                continue;
            }

            List<TextureInfo> texInfos = new List<TextureInfo>();
            for (int j = 0; j < mtSequence.textures.Length; j++)
            {
                //compInfos add element
                string texName = mtSequence.textures[j].name;
                string shaderProp = "_MainTex";
                TextureInfo spriteInfo = new TextureInfo(texName, shaderProp);
                texInfos.Add(spriteInfo);
            }

            CompInfo extCompMaterialTextureSequenceInfo = new ExtCompMaterialTextureSequenceInfo(
                    mtSequence.transform,
                    mtSequence,
                    texInfos);
            compInfos.Add(extCompMaterialTextureSequenceInfo);

        }

        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();

        var mtSequenceChilds = new List<MaterialTextureSequence>();
        obj.GetComponentsInChildren<MaterialTextureSequence>(true, mtSequenceChilds);
        Debug.Log(obj.name + "'s MaterialTextureSequence count:" + mtSequenceChilds.Count);


        for (int i = 0; i < mtSequenceChilds.Count; i++)
        {
            MaterialTextureSequence mtSequence = mtSequenceChilds[i];

            if (mtSequence.textures == null || mtSequence.textures.Length == 0)//异常情况。
            {
                Debug.LogWarning("warning,MaterialTextureSequence:" + mtSequence.transform.GetHierarchy() + "  textures not set");
                continue;
            }

            for (int j = 0; j < mtSequence.textures.Length; j++)
            {
                string texName = mtSequence.textures[j].name;
                string texPath = AssetDatabase.GetAssetPath(mtSequence.textures[j]);
                if (texPath.Contains("Assets/UI"))
                {
                    Debug.LogError("do not use tex in Assets/UI, path:" + mtSequence.transform.GetHierarchy());
                    continue;
                }

                //buildMap add element 
                this.AddBundleBuildData(texName, texPath, ABType.Texture, ref buildMap);

            }
        }
        return buildMap;
    }
}
