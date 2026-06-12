using Codice.Utils;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.RessEditor;
using static UnityEngine.Mesh;

public class BuildInCompRendererCollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    {
        List<CompInfo> compInfos = new List<CompInfo>();

        BuildMapIgnoreDatas buildMapIgnore = obj.GetComponent<BuildMapIgnoreDatas>();

        var rendererChilds = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(true, rendererChilds);

        for (int i = 0; i < rendererChilds.Count; i++)
        {
            Renderer renderer = rendererChilds[i];

            if (renderer is SpriteRenderer)//SpriteRenderer也继承自Renderer，但其特殊处理
            {
                continue;
            }
            string meshName = "";
            string meshPath = "";
            if (renderer is MeshRenderer)
            {
                var mesh = renderer.transform.GetComponent<MeshFilter>().sharedMesh;
                if (mesh != null)
                {
                    meshName = mesh.name;
                    meshPath = AssetDatabase.GetAssetPath(mesh);
                }
                else
                {
                    Debug.LogWarning("mesh is null:" + renderer.transform.GetHierarchy());
                }

            }
            else if (renderer is SkinnedMeshRenderer)
            {
                var mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
                meshName = mesh.name;
                meshPath = AssetDatabase.GetAssetPath(mesh);
            }
            else if (renderer is ParticleSystemRenderer)
            {
                ParticleSystemRenderer psRenderer = renderer as ParticleSystemRenderer;
                if (psRenderer.renderMode == ParticleSystemRenderMode.Mesh)
                {
                    meshName = psRenderer.mesh.name;
                    meshPath = AssetDatabase.GetAssetPath(psRenderer.mesh);
                }
            }
            if (meshPath.Contains("Library/unity default resources"))
            {
                meshName = "";//暂时不对内置mesh处理
            }


            ////////引用了fbx内部的mesh，且该fbx不被剥离。则该mesh不单独打ab包。
            //////if (meshName != "" && (meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX")))
            //////{
            //////    if ((buildMapIgnore != null && this.IsStripIgnoredFBX(buildMapIgnore.stripIgnoredFBXs, meshPath) == true))
            //////    {
            //////        meshName = "";
            //////    }
            //////}

            //暂时先不把fbx剥离了，问题挺多的（比如avatar也是在fbx里面的fbx剥离后avatar也会丢失）
            if ((meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX") || meshPath.EndsWith(".obj") || meshPath.EndsWith(".OBJ")))
            {
                meshName = "";//这里要把meshName置空
            }

            if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning("warning renderer has no mat attached:" + renderer.transform.GetHierarchy());
                continue;
            }

            for (int j = 0; j < renderer.sharedMaterials.Length; j++)
            {
                //compInfos add elements
                List<TextureInfo> textureInfos = new List<TextureInfo>();
                var mat = renderer.sharedMaterials[j];
                if (mat == null)
                {
                    Debug.LogWarning("warning, mat is null:" + renderer.gameObject.GetHierarchy());
                    continue;
                }
                if (BuildConfig.ignoredMats.Contains(mat.name))
                {
                    Debug.LogError("GetCompInfo mat ignore check:" + mat.name + "， path:" + renderer.transform.GetHierarchy());
                    continue;
                }

                for (int k = 0; k < ShaderUtil.GetPropertyCount(mat.shader); k++)
                {
                    var t = ShaderUtil.GetPropertyType(mat.shader, k);
                    if (t == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyName = ShaderUtil.GetPropertyName(mat.shader, k);
                        Texture tex = mat.GetTexture(propertyName);

                        if (tex == null)
                        {
                            continue;
                        }
                        //检测使用的图片是否在工程内
                        string texTmpPath = AssetDatabase.GetAssetPath(tex);
                        if (!texTmpPath.Contains("Assets"))
                        {
                            Debug.LogError("error, texPath:" + texTmpPath + ",   renderer:" + renderer.transform.GetHierarchy());
                            continue;
                        }


                        string texName = tex.name;
                        //Debug.LogError("xxx:" + texName);

                        TextureInfo textureInfo = new TextureInfo(texName, propertyName);
                        textureInfos.Add(textureInfo);
                    }
                }

                CompInfo compInfo = new BuildInCompRendererInfo(
                            renderer.transform,
                            renderer,
                            textureInfos,
                            meshName,
                            j,
                           mat.name,
                            mat.shader.name
                            );
                compInfos.Add(compInfo);
            }
        }

        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();


        var rendererChilds = new List<Renderer>();
        obj.GetComponentsInChildren<Renderer>(true, rendererChilds);
        Debug.Log(obj.name + "'s Renderer count:" + rendererChilds.Count);

        BuildMapIgnoreDatas buildMapIgnore = obj.GetComponent<BuildMapIgnoreDatas>();
        for (int i = 0; i < rendererChilds.Count; i++)
        {
            Renderer renderer = rendererChilds[i];
            if (renderer is SpriteRenderer)
            {
                continue;
            }

            if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
            {
                Debug.LogWarning("warning,renderer:" + renderer.transform.GetHierarchy() + " material not set");
                continue;
            }

            //buildMap add mesh element
            string meshName = "";
            string meshPath = "";
            if (renderer is MeshRenderer)
            {
                var mesh = renderer.transform.GetComponent<MeshFilter>().sharedMesh;
                if (mesh != null)
                {
                    meshName = mesh.name;
                    meshPath = AssetDatabase.GetAssetPath(mesh);
                    //Debug.LogError("meshName:" + meshName + ", meshPath:" + meshPath);

                }
                else
                {
                    Debug.LogWarning("mesh is null:" + renderer.transform.GetHierarchy());
                }

            }
            else if (renderer is SkinnedMeshRenderer)
            {
                var mesh = (renderer as SkinnedMeshRenderer).sharedMesh;
                meshName = mesh.name;
                meshPath = AssetDatabase.GetAssetPath(mesh);
            }
            else if (renderer is ParticleSystemRenderer)
            {
                ParticleSystemRenderer psRenderer = renderer as ParticleSystemRenderer;
                if (psRenderer.renderMode == ParticleSystemRenderMode.Mesh)
                {
                    meshName = psRenderer.mesh.name;
                    meshPath = AssetDatabase.GetAssetPath(psRenderer.mesh);
                }
            }
            if (meshPath.Contains("Library/unity default resources"))
            {
                Debug.LogWarning("used build in mesh :" + renderer.transform.GetHierarchy());
                //对于使用的内置mesh，暂时不对其打ab包
                meshName = "";
            }

            //Debug.Log("meshName:" + meshName + ", path:" + meshPath);
            //暂时先不把fbx剥离了，问题挺多的（比如avatar也是在fbx里面的fbx剥离后avatar也会丢失）
            //有些资源是obj格式
            if ((meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX") || meshPath.EndsWith(".obj") || meshPath.EndsWith(".OBJ")))
            {
                meshName = "";//这里要把meshName置空
            }
            //////string cachedFBXName = "";
            //////string cachedFBXPath = "";
            //////if (meshName != "" && (meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX")))
            //////{
            //////    if ((buildMapIgnore != null && this.IsStripIgnoredFBX(buildMapIgnore.stripIgnoredFBXs, meshPath) == true))
            //////    {
            //////        meshName = "";
            //////    }
            //////    else
            //////    {

            //////        cachedFBXName = Path.GetFileNameWithoutExtension(Application.dataPath + meshPath);
            //////        cachedFBXPath = meshPath;
            //////        //缓存的fbx/FBX文件需要加入到buildMap中。否则打prefab或者场景时，由于是对fbx/FBX的直接依赖，会把fbx打入到对应prefab或场景对应的ab文件中。该fbx bundle在BuildCommand中DeleteAfterBuildAB会被删除
            //////        this.AddBundleBuildData(cachedFBXName, cachedFBXPath, ABType.FBX, ref buildMap);


            //////        //引用的是fbx内的mesh，那么则从fbx内导出临时mesh文件，用来打ab
            //////        AssetDatabaseExt.ExportAssetToFBXTmpFolder(meshPath, "mesh");
            //////        //从临时文件夹内获得真实的路径
            //////        meshPath = AssetDatabaseExt.GetAssetPathFromFBXTmpFolder(meshName, "mesh");
            //////    }
            //////}

            if (meshName != "")
            {
                if (meshPath.EndsWith(".mesh") || meshPath.EndsWith(".asset"))//.asset是使用MantisLODEditor生成的mesh资源的后缀
                {
                    Debug.Log($"meshName:{meshName},meshPath:{meshPath}");
                    this.AddBundleBuildData(meshName, meshPath, ABType.Mesh, ref buildMap);
                }
                else
                {
                    Debug.LogError(" get mesh from tmp folder error:" + meshName);
                }
            }





            for (int j = 0; j < renderer.sharedMaterials.Length; j++)
            {
                //compInfos add elements
                List<TextureInfo> textureInfos = new List<TextureInfo>();
                var mat = renderer.sharedMaterials[j];
                if (mat == null)
                {
                    Debug.LogError("error, mat is null:" + renderer.gameObject.GetHierarchy());
                    continue;
                }
                string matPath = AssetDatabase.GetAssetPath(mat);
                if (matPath.EndsWith(".fbx") || matPath.EndsWith(".FBX"))
                {
                    Debug.LogError("error,use mat in fbx, matPath:" + matPath);
                    continue;
                }


                if (BuildConfig.ignoredMats.Contains(mat.name))
                {
                    Debug.LogError("GetResMap mat ignore check:" + mat.name + "， path:" + renderer.transform.GetHierarchy());
                    continue;
                }


                for (int k = 0; k < ShaderUtil.GetPropertyCount(mat.shader); k++)
                {
                    var t = ShaderUtil.GetPropertyType(mat.shader, k);
                    if (t == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyName = ShaderUtil.GetPropertyName(mat.shader, k);
                        Texture tex = mat.GetTexture(propertyName);

                        if (tex == null)
                        {
                            continue;
                        }
                        //检测使用的图片是否在工程内
                        string texTmpPath = AssetDatabase.GetAssetPath(tex);
                        if (!texTmpPath.Contains("Assets"))
                        {
                            Debug.LogError("error, texPath:" + texTmpPath + ", renderer:" + renderer.gameObject);
                            continue;
                        }
                        if (texTmpPath.Contains("Assets/UI"))
                        {
                            Debug.LogError("do not use tex in Assets/UI, path:" + renderer.transform.GetHierarchy());
                            continue;
                        }

                        string texName = tex.name;

                        //buildMap add tex elements                         
                        this.AddBundleBuildData(texName, texTmpPath, ABType.Texture, ref buildMap);
                    }
                }

                //buildMap add mat element
                this.AddBundleBuildData(mat.name, AssetDatabase.GetAssetPath(mat), ABType.Material, ref buildMap);
            }
        }
        return buildMap;
    }
}
