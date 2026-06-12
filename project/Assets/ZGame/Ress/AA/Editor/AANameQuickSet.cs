using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class AANameQuickSet : UnityEditor.Editor
{
    [MenuItem("Assets/ZGame/AA/对选择项设置AA并归一化名字")]
    public static void NameSet()
    {
        UnityEngine.Object[] assets = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets) as UnityEngine.Object[];
        for (int i = 0; i < assets.Length; i++)
        {
            var path = AssetDatabase.GetAssetPath(assets[i]);
            var guid = AssetDatabase.AssetPathToGUID(path);

            var assetEntry = AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(guid);
            if (assetEntry != null)
            {
                string newAddressName = Path.GetFileName(assetEntry.address);
                assetEntry.SetAddress(newAddressName, false);
            }
            else
            {
                Debug.LogError($"error, asset:{path} not enable Addressable");
            }
        }
        Debug.Log("set finished");
    }
}
