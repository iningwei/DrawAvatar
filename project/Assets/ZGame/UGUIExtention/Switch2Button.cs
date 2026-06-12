using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZGame.EventExt;

namespace ZGame.UGUIExtention
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class Switch2Button : Button
    {
       
       public Sprite[] switchSprites;
        [SerializeField]
        int curIndex = 0;

        Image imgComp;
        public UnityExtEvent onIndexChanged = new UnityExtEvent();

        protected override void Awake()
        {
            this.imgComp = this.GetComponent<Image>();
            this.selectIndex(this.curIndex);
        }
        public int GetCurIndex()
        {
            return this.curIndex;
        }

        void selectIndex(int index)
        {
            if (switchSprites == null || switchSprites.Length == 0)
            {
                return;
            }
            for (int i = 0; i < switchSprites.Length; i++)
            {
                if (i == index)
                {
                    this.imgComp.sprite = switchSprites[i];
                    break;
                }
            }
        }

        public void SetSelection(int index, bool triggerIndexChangedCallback = true)
        {
            this.curIndex = index;
            this.selectIndex(index);
            if (triggerIndexChangedCallback)
            {
                onIndexChanged.Invoke(index);
            }

        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (interactable)
            {
                int tmpIndex = curIndex + 1;

                if (tmpIndex >= switchSprites.Length)
                {
                    tmpIndex = 0;
                }

                this.SetSelection(tmpIndex);
            }
        }
    }
}