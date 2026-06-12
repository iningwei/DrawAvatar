using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
using ZGame.RessEditor;

public class BuildInCompMeshColliderCollection : CompResCollection, IRefResCollection
{
    public List<CompInfo> GetCompInfo(GameObject obj)
    { 
        BuildMapIgnoreDatas buildMapIgnore = obj.GetComponent<BuildMapIgnoreDatas>();

        List<CompInfo> compInfos = new List<CompInfo>();
        var meshColliderChilds = new List<MeshCollider>();
        obj.GetComponentsInChildren<MeshCollider>(true, meshColliderChilds);


        for (int i = 0; i < meshColliderChilds.Count; i++)
        {
            MeshCollider meshCollider = meshColliderChilds[i];
            var mesh = meshCollider.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("meshCollider's mesh is null:" + meshCollider.transform.GetHierarchy());
                continue;
            }
            string meshName = "";
            string meshPath = "";

            meshName = mesh.name;
            meshPath = AssetDatabase.GetAssetPath(mesh);
            if (meshPath.Contains("Library/unity default resources"))
            {
                meshName = "";//暂时不对内置mesh处理
                continue;
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
            if ((meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX")))
            {
                meshName = "";//这里要把meshName置空
            }

            //compInfos add element 
            CompInfo buildInCompMeshColliderInfo = new BuildInCompMeshColliderInfo(
               meshCollider.transform,
                meshCollider,
                 meshName,
                 -1,
               "",
                "");
            compInfos.Add(buildInCompMeshColliderInfo);

        }

        return compInfos;
    }

    public Dictionary<string, AssetBundleBuild> GetResMap(GameObject obj)
    {
        Dictionary<string, AssetBundleBuild> buildMap = new Dictionary<string, AssetBundleBuild>();



        var meshColliderChilds = new List<MeshCollider>();
        obj.GetComponentsInChildren<MeshCollider>(true, meshColliderChilds);
        Debug.Log(obj.name + "'s MeshCollider count:" + meshColliderChilds.Count);

        BuildMapIgnoreDatas buildMapIgnore = obj.GetComponent<BuildMapIgnoreDatas>();
        for (int i = 0; i < meshColliderChilds.Count; i++)
        {
            MeshCollider meshCollider = meshColliderChilds[i];
            var mesh = meshCollider.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("meshCollider's mesh is null:" + meshCollider.transform.GetHierarchy());
                continue;
            }


            //buildMap add mesh element
            string meshName = "";
            string meshPath = "";
            meshName = mesh.name;
            meshPath = AssetDatabase.GetAssetPath(mesh);
            if (meshPath.Contains("Library/unity default resources"))
            {
                meshName = "";//暂时不对内置mesh处理
                continue;
            }

            //暂时先不把fbx剥离了，问题挺多的（比如avatar也是在fbx里面的fbx剥离后avatar也会丢失）
            if ((meshPath.EndsWith(".fbx") || meshPath.EndsWith(".FBX")))
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
                if (meshPath.EndsWith(".mesh"))
                {
                    this.AddBundleBuildData(meshName, meshPath, ABType.Mesh, ref buildMap);
                }
                else
                {
                    Debug.LogError(" get mesh from tmp folder error:" + meshName);
                }
            }
        }
        return buildMap;
    }
}
