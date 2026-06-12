using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ZGame.UGUIExtention
{
    public class ButtonPressDownEvent : UnityEvent
    {


    }
    public class ButtonPressUpEvent : UnityEvent
    {

    }

    public class ButtonPressExitEvent : UnityEvent
    {

    }
    public class ButtonLongPressEvent : UnityEvent
    {

    }

    //, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    public class PressButton : Button
    {
        public ButtonPressDownEvent onPressDown = new ButtonPressDownEvent();
        public ButtonPressUpEvent onPressUp = new ButtonPressUpEvent();
        public ButtonPressExitEvent onPressExit = new ButtonPressExitEvent();
        public ButtonLongPressEvent onLongPress = new ButtonLongPressEvent();

        bool isPress = false;
        public float LongPressCheckDuration = 0.2f;
        float longPressedTime;
        bool isLongPress = false;

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);//获得原Button的Transition效果
            if (interactable)
            {
                onPressUp?.Invoke();
                isPress = false;
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
           
                base.OnPointerDown(eventData);//获得原Button的Transition效果
                if (interactable)
                {
                    onPressDown?.Invoke();
                    isPress = true;
                    isLongPress = false;
                    longPressedTime = 0f;
                }
            
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (interactable)
            {
                onPressExit?.Invoke();
            }
        }



        private void Update()
        {
            if (isPress && !isLongPress)
            {
                longPressedTime += Time.deltaTime;
                if (longPressedTime >= LongPressCheckDuration)
                {
                    isLongPress = true;
                    onLongPress?.Invoke();
                }
            }
        }

    }
}