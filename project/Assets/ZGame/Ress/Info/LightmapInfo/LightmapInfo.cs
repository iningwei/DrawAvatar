using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ZGame.Ress.Info;

[System.Serializable]
public class LightmapRenderInfo : CompInfo
{
    public int originLightmapIndex;//means origin index attach to scene.
    public int curLightmapIndex;
    public Vector4 lightmapScaleOffset;


    public LightmapRenderInfo(Transform tran, int orginLightmapIndex, Vector4 lightmapScaleOffset):base(tran)
    {        
        this.originLightmapIndex = orginLightmapIndex;
        this.curLightmapIndex = orginLightmapIndex;
        this.lightmapScaleOffset = lightmapScaleOffset;

    }
}


[System.Serializable]
public class LightmapInfo
{
    public string attachedSceneName;

    //记录的是烘焙贴图名
    public List<string> lightmapColors;
    public List<string> lightmapDirs;
    public List<string> lightmapShadowMasks;

    public List<LightmapRenderInfo> lightmapRenders;

    public LightmapInfo(List<string> lightmapColors, List<string> lightmapDirs, List<string> lightmapShadowMasks, List<LightmapRenderInfo> lightmapRenders)
    {
        this.lightmapColors = lightmapColors;
        this.lightmapDirs = lightmapDirs;
        this.lightmapShadowMasks = lightmapShadowMasks;
        this.lightmapRenders = lightmapRenders;
    }
}


public class SceneLightmapMsg
{
    public string sceneName;
    public int count;//lightmap tex count

    public LightmapData[] datas;
    public List<LightmapRenderInfo> renderInfos;
}
