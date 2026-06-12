using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems; 

namespace ZGame.UGUIExtention
{
    public class SwipeGesture : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Vector3 offset;
        Vector3 lastPos;

        private void Start()
        {
            offset = Vector3.zero;
        }

 
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 pos = eventData.position;
            offset = pos - lastPos;
            lastPos = pos;
            //Debug.LogError("offset:" + offset);
        }
 
        public void OnPointerDown(PointerEventData eventData)
        {
            offset = Vector3.zero;
            lastPos = Vector3.zero;
        }
 
        public void OnPointerUp(PointerEventData eventData)
        {
            offset = Vector3.zero;
            lastPos = Vector3.zero;
        }
    }
}