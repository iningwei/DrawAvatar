using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


namespace ZGame.TextMeshProExtention
{

    public class TextMeshProUGUILink : MonoBehaviour, IPointerClickHandler
    {
        TextMeshProUGUI proUGUI;


        public Action<string> onLinkClicked;

        private void Awake()
        {
            proUGUI = this.gameObject.GetComponent<TextMeshProUGUI>();
        }

        //UI相机, 非相机渲染模式，则不用设置
        public Camera cam;
        public void OnPointerClick(PointerEventData eventData)
        {
            Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(proUGUI, pos, cam);
            if (linkIndex > -1)
            {
                TMP_LinkInfo linkInfo = proUGUI.textInfo.linkInfo[linkIndex];
                string linkId = linkInfo.GetLinkID();
                if (onLinkClicked.GetInvocationList() != null)
                {
                    onLinkClicked.Invoke(linkId);
                }

            }


        }
    }
}