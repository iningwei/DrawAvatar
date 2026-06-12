using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{
    public class SwapSprite : MonoBehaviour
    {
        public Sprite[] sprites;
        Image image;
        private void Awake()
        {
            image = GetComponent<Image>();
            if (image == null)
            {
                DebugExt.LogE("error, no Image component attached to:" + this.gameObject.GetHierarchy());
            }
            if (this.sprites == null || this.sprites.Length == 0)
            {
                DebugExt.LogE("error,no sprite set for sprites!");
            }

        }

        public void SetIndex(int i)
        {
            if (this.sprites == null || this.sprites.Length == 0)
            {
                DebugExt.LogE("can not set swapSprite for sprites have no contents.");
                return;
            }
            if (i > this.sprites.Length - 1)
            {
                DebugExt.LogE("index out of range:" + i);
                return;
            }
            var s = this.sprites[i];
            if (s == null)
            {
                DebugExt.LogE($"error sprite {i} is null");
            }
            image.sprite = s;
        }

        public void SetImageNativeSize()
        {
            image.SetNativeSize();
        }
    }
}