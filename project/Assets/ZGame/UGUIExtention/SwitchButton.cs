using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ZGame.EventExt;

namespace ZGame.UGUIExtention
{

    //, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    public class SwitchButton : Button
    {
        [SerializeField]
        Transform[] switchTargets;
        [SerializeField]
        int curIndex = 0;

        public UnityExtEvent onIndexChanged = new UnityExtEvent();
        public int GetCurIndex()
        {
            return this.curIndex;
        }
        protected override void Start()
        {
            base.Start();
            selectIndex(curIndex);
        }

        void selectIndex(int index)
        {
            if (switchTargets == null || switchTargets.Length == 0)
            {
                return;
            }
            for (int i = 0; i < switchTargets.Length; i++)
            {
                if (index == i)
                {
                    switchTargets[i].gameObject.SetActive(true);
                }
                else
                {
                    switchTargets[i].gameObject.SetActive(false);
                }
            }
        }

        public void SetSelection(int index)
        {
            this.curIndex = index;
            this.selectIndex(index);
            onIndexChanged.Invoke(curIndex);
        }
        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            if (interactable)
            {
                int tmpIndex = curIndex + 1;

                if (tmpIndex >= switchTargets.Length)
                {
                    tmpIndex = 0;
                }

                this.SetSelection(tmpIndex);
            }
        }


        //public override void OnPointerUp(PointerEventData eventData)
        //{
        //    base.OnPointerUp(eventData); 
        //    if (interactable)
        //    {
        //    }
        //}
        //public override void OnPointerDown(PointerEventData eventData)
        //{
        //    base.OnPointerDown(eventData); 
        //    if (interactable)
        //    {
        //    }
        //}
        //public override void OnPointerExit(PointerEventData eventData)
        //{
        //    base.OnPointerExit(eventData);
        //    if (interactable)
        //    {
        //    }
        //}
    }
}