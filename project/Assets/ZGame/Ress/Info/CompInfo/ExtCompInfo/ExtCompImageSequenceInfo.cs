using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ZGame.Ress.AB;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompImageSequenceInfo : ExtCompInfo, IFillCompElement
    {
        public ImageSequence concreteCompImageSequence;
        public List<SpriteInfo> refSprites;
        public ExtCompImageSequenceInfo(Transform tran, ImageSequence refImageSequence, List<SpriteInfo> refSprites) : base(tran)
        {
            this.concreteCompImageSequence = refImageSequence;
            this.refSprites = refSprites;
        }

        public void FillCompElement(bool sync)
        {
            if (this.refSprites != null && this.refSprites.Count > 0)
            {
                for (int i = 0; i < this.refSprites.Count; i++)
                {
                    ABManager.Instance.LoadSpriteToTarget(concreteCompImageSequence, this.refSprites[i].atlasName, this.refSprites[i].spriteName, sync, i);
                }
            }
        }
    }
}