using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GetAssetTypeTool
{
    [MenuItem("Assets/ZGame/获得资源类型")]
    static void LogAssetType()
    {
        UnityEngine.Object[] selectedAssets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];

        for (int i = 0; i < selectedAssets.Length; i++)
        {
            var asset = selectedAssets[i];
            string path = AssetDatabase.GetAssetPath(asset);
            var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);  // Texture和Sprite获得的类型都是UnityEngine.Texture2D
            string assetTypeName = assetType.ToString();
            if (assetType.Name == typeof(UnityEngine.Texture2D).Name)
            {
                var texTypeInt = AssetDatabaseExt.GetTextureImporterType(path);
                var importerType = (TextureImporterType)texTypeInt;
                assetTypeName = "Texture:" + importerType.ToString();
            }
            Debug.Log(path + ", assetType:" + assetTypeName);
        }
    }
}
