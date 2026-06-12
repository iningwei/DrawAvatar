using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame.EventExt;
namespace ZGame.UGUIExtention
{
    public class RadioButtonGroup : MonoBehaviour
    {
        public int curIndex = 0;
        public Button[] btns;
        public Sprite selectSprite;
        public Sprite unselectSprite;

        public UnityExtEvent onIndexChanged = new UnityExtEvent();

        public int GetCurIndex()
        {
            return this.curIndex;
        }
        private void Start()
        {

            if (btns != null && btns.Length != 0)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    int index = i;
                    btns[i].onClick.AddListener(() =>
                    {
                        this.onRadioBtnClicked(index);
                    });
                }
            }

            this.SetSelection(this.curIndex);
        }

        void onRadioBtnClicked(int index)
        {
            this.SetSelection(index);
        }

        void selectIndex(int index)
        {
            if (btns == null || btns.Length == 0)
            {
                return;
            }

            Sprite target;
            for (int i = 0; i < btns.Length; i++)
            {
                if (i == index)
                {
                    target = selectSprite;
                    btns[i].interactable = false;
                }
                else
                {
                    target = unselectSprite;
                    btns[i].interactable = true;
                }
                btns[i].GetComponent<Image>().sprite = target;

            }

        }


        public void SetSelection(int index)
        {
            this.curIndex = index;
            this.selectIndex(index);
            onIndexChanged.Invoke(index);
        }
    }
}