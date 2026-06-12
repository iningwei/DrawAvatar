
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

public class CompResCollection
{
    public void AddBundleBuildData(string assetName, string assetPath, ABType abType, ref Dictionary<string, AssetBundleBuild> buildMap)
    {
        //buildMap若已包含assetPath则不可再加，否则打ab报错
        foreach (var item in buildMap)
        {
            for (int i = 0; i < item.Value.assetNames.Length; i++)
            {
                if (item.Value.assetNames[i] == assetPath)
                {
                    Debug.LogWarning("-->try add assetName:" + assetName + ", assetPath:" + assetPath + ", abType:" + abType.ToString() + "。but already exist assetPath:" + assetPath + ", it's name is :" + item.Key);
                    return;
                }
            }
        }

       


        string bundleName = ABTypeUtil.GetPreFix(abType) + assetName.ToLower() + IOTools.abSuffix;
        if (buildMap.ContainsKey(bundleName) == false)
        {
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = new string[] { assetPath };
            buildMap.Add(bundleName, build);
        }
    }


    public bool IsStripIgnoredFBX(GameObject[] stripIgnoredFBXs, string fbxPath)
    {
        if (stripIgnoredFBXs != null && stripIgnoredFBXs.Length > 0)
        {
            for (int i = 0; i < stripIgnoredFBXs.Length; i++)
            {
                string path = AssetDatabase.GetAssetPath(stripIgnoredFBXs[i]);
                if (path == fbxPath)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
