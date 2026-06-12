using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.Ress.Info;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompMeshColliderInfo : BuildInCompInfo, IFillCompElement
    {
        public MeshCollider concreteCompMeshCollider;
        public BuildInCompMeshColliderInfo(Transform tran, MeshCollider refMeshCollider, string meshName, int matIndex, string matName, string shaderName) : base(tran, meshName, matIndex, matName, shaderName)
        {
            concreteCompMeshCollider = refMeshCollider;
        }

        public void FillCompElement(bool sync)
        {
            if (this.meshName != "")
            {
                MeshRes oldMeshRes = null;
                string oldMeshName = "";
                MeshRes meshRes = null;
                Mesh mesh = null;
                ABManager.Instance.LoadMesh(this.meshName, (res) =>
                {
                    meshRes = res;
                    if (meshRes != null)
                    {
                        mesh = res.GetResAsset<Mesh>();

                        var curMesh = this.concreteCompMeshCollider.sharedMesh;
                        if (curMesh != null)
                        {
                            oldMeshRes = ABManager.Instance.GetCachedRes<MeshRes>(curMesh.name);
                            if (oldMeshRes != null)
                            {
                                oldMeshName = oldMeshRes.resName;
                            }
                        }

                        this.concreteCompMeshCollider.sharedMesh = mesh;

                        //remove mesh ref
                        if (oldMeshRes != null && oldMeshName != meshRes.resName)
                        {
                            oldMeshRes.RemoveRefTrs(this.tran);
                        }
                        //add mesh ref 
                        meshRes.AddRefTrs(this.tran);
                    }
                }, sync);
            }
        }
    }
}