using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class BuildInCompInfo : CompInfo
    {
        public string meshName;
        public int matIndex = -1;
        public string matName;
        public string shaderName;

        public BuildInCompInfo(Transform tran, string meshName, int matIndex, string matName, string shaderName) : base(tran)
        {
            this.meshName = meshName;
            this.matIndex = matIndex;
            this.matName = matName;
            this.shaderName = shaderName;
        }
    }
}