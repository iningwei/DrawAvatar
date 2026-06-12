using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Ress.AB;
using ZGame.UGUIExtention;

namespace ZGame.Ress.Info
{
    [System.Serializable]
    public class ExtCompSwitch2ButtonInfo : ExtCompInfo, IFillCompElement
    {
        public Switch2Button concreteCompS2B;
        public List<SpriteInfo> refSprites;
        public ExtCompSwitch2ButtonInfo(Transform tran, Switch2Button refS2B, List<SpriteInfo> refSprites) : base(tran)
        {
            this.concreteCompS2B = refS2B;
            this.refSprites = refSprites;
        }

        public void FillCompElement(bool sync)
        {

            if (this.refSprites != null && this.refSprites.Count > 0)
            {
                for (int i = 0; i < this.refSprites.Count; i++)
                {
                    ABManager.Instance.LoadSpriteToTarget(concreteCompS2B, this.refSprites[i].atlasName, this.refSprites[i].spriteName, sync, i);
                }
            }
        }
    }
}