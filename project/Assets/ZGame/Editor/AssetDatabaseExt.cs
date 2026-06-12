using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetDatabaseExt
{
    public static TextureImporterType GetTextureImporterType(Texture tex)
    {
        string texPath = AssetDatabase.GetAssetPath(tex);
        var texType = (TextureImporterType)GetTextureImporterType(texPath);
        return texType;
    }
    public static int GetTextureImporterType(string path)
    {
        var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);
        if (assetType.Name == typeof(UnityEngine.Texture2D).Name)
        {
            string metaFile = path + ".meta";
            var lines = IOTools.GetFileLines(metaFile);
            if (lines != null)
            {
                for (int j = 0; j < lines.Length; j++)
                {
                    var line = lines[j];
                    if (line.Contains("textureType:"))
                    {
                        int texTypeInt = int.Parse(line.Split(':')[1].Trim());
                        return texTypeInt;
                    }
                }
            }
        }
        Debug.LogError("get textureImporterType error," + path);
        return -1;
    }




    /// <summary>
    /// 
    /// </summary>
    /// <param name="originFBXFilePath">start with:Assets/</param>
    /// <param name="assetType">mesh; anim</param>
    public static void ExportAssetToFBXTmpFolder(string originFBXFilePath, string assetType)
    {
        if (!originFBXFilePath.EndsWith(".fbx") && !originFBXFilePath.EndsWith(".FBX"))
        {
            Debug.LogError("it is not fbx/FBX:" + originFBXFilePath);
            return;
        }
        string fbxFileName = Path.GetFileNameWithoutExtension(Path.Combine(Application.dataPath, originFBXFilePath));

        string tmpFolder = Path.Combine(Application.dataPath, "temp_for_fbx/" + fbxFileName);
        if (!Directory.Exists(tmpFolder))
        {
            Directory.CreateDirectory(tmpFolder);
        }
        string outputFolder = "Assets/" + "/temp_for_fbx/" + fbxFileName;
        //create meshes
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(originFBXFilePath);
        for (int i = 0; i < objects.Length; i++)
        {
            if (assetType == "mesh")
            {
                if (objects[i] is Mesh)
                {
                    Mesh mesh = Object.Instantiate(objects[i]) as Mesh;
                    string outputPath = outputFolder + "/" + objects[i].name + ".mesh";
                    AssetDatabase.CreateAsset(mesh, outputPath);
                }
            }
            else if (assetType == "anim")
            {
                if (objects[i] is AnimationClip)
                {
                    AnimationClip clip = Object.Instantiate(objects[i]) as AnimationClip;
                    string outputPath = outputFolder + "/" + objects[i].name + ".anim";
                    AssetDatabase.CreateAsset(clip, outputPath);
                }
            }
            else
            {
                Debug.LogError("not supported assetType:" + assetType);
            }

        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="assetType">mesh; anim</param>
    /// <returns></returns>
    public static string GetAssetPathFromFBXTmpFolder(string name, string assetType)
    {
        string[] files = null;
        if (assetType == "mesh")
        {
            files = Directory.GetFiles(Application.dataPath + "/temp_for_fbx", "*.mesh", SearchOption.AllDirectories);
        }
        else if (assetType == "anim")
        {
            files = Directory.GetFiles(Application.dataPath + "/temp_for_fbx", "*.anim", SearchOption.AllDirectories);
        }
        else
        {
            Debug.LogError("not supported assetType:" + assetType);
        }

        if (files.Length > 0)
        {
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i];
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (fileName == name)
                {
                    return filePath.Substring(filePath.IndexOf("Assets"));
                }
            }
        }
        return "";
    }
}
