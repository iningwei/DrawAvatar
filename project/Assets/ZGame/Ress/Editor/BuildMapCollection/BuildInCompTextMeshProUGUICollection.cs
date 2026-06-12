using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.Info;
using static PlasticGui.PlasticTableColumn;
using ZGame.RessEditor;

public class BuildInCompTextMeshProUGUICollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();
        var textChilds = new List<TextMeshProUGUI>();
        obj.GetComponentsInChildren<TextMeshProUGUI>(true, textChilds);

        for (int i = 0; i < textChilds.Count; i++)
        {
            TextMeshProUGUI text = textChilds[i];
            //////if (BuildConfig.ignoredMats.Contains(text.font.material.name))
            //////{
            //////    Debug.LogError("GetCompInfo mat ignore check:" + text.font.material.name + "， path:" + text.transform.GetHierarchy());
            //////    continue;
            //////}
            //be careful not use text.material,otherwise it will return old ugui text's default material.
             CompInfo buildInCompTextMeshProInfo = new BuildInCompTextMeshProUGUIInfo(text.transform, text, "", text.font.material.name, text.font.material.shader.name);
          
            compInfos.Add(buildInCompTextMeshProInfo);

            Debug.Log("---->shader name:" + text.font.material.shader.name + ", transform:" + text.transform.name);
        }
        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();

        var textChilds = new List<TextMeshProUGUI>();
        obj.GetComponentsInChildren<TextMeshProUGUI>(true, textChilds);
        Debug.Log(obj.name + "'s TextMeshProUGUI count:" + textChilds.Count);
        for (int i = 0; i < textChilds.Count; i++)
        {
            TextMeshProUGUI tmpTMP = textChilds[i];

            //buildMap add mat element
            //////this.AddBundleBuildData(tmpTMP.material.name, AssetDatabase.GetAssetPath(tmpTMP.material), ZGame.Ress.AB.ABType.Material, ref buildMap);
            //TMP的字体不需要对材质球打ab，其没有类似于Lagacy字体组件的材质球设置。
            //使用的是字体本身的材质球
        }

        return buildMap;
    }


}
