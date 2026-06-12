using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
  
using ZGame.Window;

namespace ZGame.UGUIExtention
{
    public class DragButton : MonoBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler
    {
        public bool isDragPress = false;
        public bool isDragEnd = false;
        public Vector2 startVec;
        public Vector2 direction;
        public Vector3 tempDirection;
        public float distance; 

        public UnityEvent onPointerDown = new UnityEvent();
        public UnityEvent onDragOut = new UnityEvent();
        public UnityEvent onDragEnd = new UnityEvent();

        RectTransform rectTran;
        Slider slider;
        Transform handleTran;
        Transform endDragImgTran;

 

        void Start()
        {
            rectTran = gameObject.GetComponent<RectTransform>();
            slider = gameObject.GetComponent<Slider>();
            handleTran = transform.Find("Handle Slide Area/Handle");
            endDragImgTran = transform.Find("endDragImg");

            direction = (endDragImgTran.position - handleTran.position).normalized;
             
        }
 
        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke();
            startVec = eventData.position;
        }

 
        public void OnDrag(PointerEventData eventData)
        {
            float dis = Vector2.Distance(startVec, eventData.position);
              
            tempDirection = (eventData.position - startVec).normalized;
            float dot = Vector2.Dot(direction, tempDirection);

            float height = rectTran.sizeDelta.y;
            if (dot < 0f)
            { 
                onDragOut.Invoke();
            }
            else
            {
                slider.value = 0;
                if (height > 20)
                {
                    rectTran.sizeDelta = new Vector2(20, rectTran.sizeDelta.y - dis);
                }
                else
                {
                    rectTran.sizeDelta = new Vector2(20, 20);
                }
            }

            startVec = eventData.position;
        }
         
        public void OnEndDrag(PointerEventData eventData)
        {
            distance = Vector3.Distance(endDragImgTran.position, handleTran.position);
            onDragEnd.Invoke();
        }
    }
}
