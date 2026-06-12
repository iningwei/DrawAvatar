using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.RessEditor;

public class LightmapInfoCollection : CompResCollection
{
    public LightmapInfo GetCompInfo(GameObject obj)
    {
        LightmapInfo info = null;

        //光照贴图
        List<string> lightmapColors = new List<string>();
        LightmapData[] lightmaps = LightmapSettings.lightmaps;
        for (int i = 0; i < lightmaps.Length; i++)
        {
            if (lightmaps[i].lightmapColor != null)
            {
                var name = lightmaps[i].lightmapColor.name;
                lightmapColors.Add(name);
                Debug.Log("get lightmapColors:" + name);
            }
            if (lightmaps[i].lightmapDir != null)
            {
                //TODO:
            }
            if (lightmaps[i].shadowMask != null)
            {
                //TODO:
            }
        }

        //Render信息
        List<LightmapRenderInfo> renderInfos = new List<LightmapRenderInfo>();
        if (lightmaps.Length > 0)
        {
            var rendererChilds = new List<Renderer>();
            obj.GetComponentsInChildren<Renderer>(true, rendererChilds);
            for (int i = 0; i < rendererChilds.Count; i++)
            {
                Renderer renderer = rendererChilds[i];
                if (renderer is MeshRenderer)
                {
                    MeshRenderer meshRenderer = renderer as MeshRenderer;
                    if (meshRenderer.receiveGI == ReceiveGI.Lightmaps)
                    {
                        int index = meshRenderer.lightmapIndex;

                        Vector4 scaleOffset = meshRenderer.lightmapScaleOffset;
                        LightmapRenderInfo renderInfo = new LightmapRenderInfo(renderer.transform, index, scaleOffset);
                        renderInfos.Add(renderInfo);
                    }
                }
            }
        }



        info = new LightmapInfo(lightmapColors, null, null, renderInfos);
        info.attachedSceneName = EditorSceneManager.GetActiveScene().name;
        return info;
    }


    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();



        LightmapData[] lightmaps = LightmapSettings.lightmaps;
        for (int i = 0; i < lightmaps.Length; i++)
        {
            if (lightmaps[i].lightmapColor != null)
            {
                var tex = lightmaps[i].lightmapColor;
                var texPath = AssetDatabase.GetAssetPath(tex);
                var texName = tex.name;

                //////string bundleName = ABTypeUtil.GetPreFix(ABType.Texture) + texName.ToLower() + IOTools.abSuffix;
                //////if (buildMap.ContainsKey(bundleName) == false)
                //////{
                //////    AssetBundleBuild build = new AssetBundleBuild();
                //////    build.assetBundleName = bundleName;
                //////    build.assetNames = new string[] { texPath };
                //////    buildMap.Add(bundleName, build);
                //////} 
                this.AddBundleBuildData(texName, texPath, ABType.Texture, ref buildMap);
            }
            if (lightmaps[i].lightmapDir != null)
            {
                //TODO:
            }
            if (lightmaps[i].shadowMask != null)
            {
                //TODO:
            }
        }


        return buildMap;
    }
}
