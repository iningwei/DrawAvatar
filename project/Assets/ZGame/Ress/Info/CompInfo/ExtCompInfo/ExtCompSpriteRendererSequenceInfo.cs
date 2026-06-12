using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSpriteRendererSequenceInfo : ExtCompInfo, IFillCompElement
    {
        public SpriteRendererSequence concreteCompSRS;
        public List<SpriteInfo> refSprites;
        public ExtCompSpriteRendererSequenceInfo(Transform tran, SpriteRendererSequence refSRS, List<SpriteInfo> refSprites) : base(tran)
        {
            this.concreteCompSRS = refSRS;
            this.refSprites = refSprites;
        }

        public void FillCompElement(bool sync)
        {
            if (this.refSprites != null && this.refSprites.Count > 0)
            {
                for (int i = 0; i < this.refSprites.Count; i++)
                {
                    ABManager.Instance.LoadSpriteToTarget(this.concreteCompSRS, this.refSprites[i].atlasName, this.refSprites[i].spriteName, sync, i);
                }
            }
        }
    }
}