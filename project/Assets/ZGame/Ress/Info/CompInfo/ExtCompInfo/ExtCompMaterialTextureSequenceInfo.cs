using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompMaterialTextureSequenceInfo : ExtCompInfo, IFillCompElement
    {
        public MaterialTextureSequence concreteCompMTS;
        public List<TextureInfo> refTextures;
        public ExtCompMaterialTextureSequenceInfo(Transform tran, MaterialTextureSequence refMTS, List<TextureInfo> refTextures) : base(tran)
        {
            this.concreteCompMTS = refMTS;
            this.refTextures = refTextures;
        }

        public void FillCompElement(bool sync)
        {
            if (this.refTextures != null && this.refTextures.Count > 0)
            {
                for (int i = 0; i < this.refTextures.Count; i++)
                {
                    ABManager.Instance.LoadTextureToMatTextureSeq(this.concreteCompMTS, this.refTextures[i].texName, i, sync);
                }
            }
        }
    }
}