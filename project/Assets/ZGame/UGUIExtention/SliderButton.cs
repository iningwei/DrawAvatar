using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
 

namespace ZGame.UGUIExtention
{
    public class SliderButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public float pressTime = 0f;
        public float sliderMaxTime = 0.5f;
        public bool isSliderPress = false;
        public bool isPressEnd = false;
        public Vector3 offset;
        public Vector3 normalOffset;
        Vector3 lastPos;

        public UnityEvent onPointerDown = new UnityEvent();
        public UnityEvent onPointerUp = new UnityEvent();

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (isSliderPress)
            {
                pressTime += Time.deltaTime;

                if (pressTime >= sliderMaxTime)
                {
                    Vector3 pos = Input.mousePosition;
                    offset = pos - lastPos;
                    normalOffset = offset.normalized;

                    isSliderPress = false;
                    isPressEnd = true;
                    pressTime = 0;
                }

            }
        }

 
        public void OnPointerDown(PointerEventData eventData)
        {
            print("OnPointerDown11");
            onPointerDown.Invoke();
            offset = Vector3.zero;
            normalOffset = offset.normalized;
            lastPos = eventData.position;
            isSliderPress = true;
            isPressEnd = false;
        }
 
        public void OnPointerUp(PointerEventData eventData)
        {
            print("OnPointerUp11");
            if (pressTime == 0) return;

            Vector3 pos = eventData.position;
            offset = pos - lastPos;
            normalOffset = offset.normalized;
            lastPos = pos;
            pressTime = 0;
            isSliderPress = false;
            isPressEnd = true;
            onPointerUp.Invoke();
        }
    }
}
