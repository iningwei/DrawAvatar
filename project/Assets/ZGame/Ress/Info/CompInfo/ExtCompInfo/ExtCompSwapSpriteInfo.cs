using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.UGUIExtention;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSwapSpriteInfo : ExtCompInfo, IFillCompElement
    {
        public SwapSprite concreteCompSS;
        public List<SpriteInfo> refSprites;
        public ExtCompSwapSpriteInfo(Transform tran, SwapSprite refSS, List<SpriteInfo> refSprites) : base(tran)
        {
            this.concreteCompSS = refSS;
            this.refSprites = refSprites;
        }

        public void FillCompElement(bool sync)
        {
            if (this.refSprites != null && this.refSprites.Count > 0)
            {
                for (int i = 0; i < this.refSprites.Count; i++)
                {
                    ABManager.Instance.LoadSpriteToTarget(this.concreteCompSS, this.refSprites[i].atlasName, this.refSprites[i].spriteName, sync, i);
                }
            }
        }
    }
}