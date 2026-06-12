using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace ZGame.UGUIExtention
{
    public class ShortClickButton : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent onPointerDown = new UnityEvent();


        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown.Invoke();
        }
    }
}
