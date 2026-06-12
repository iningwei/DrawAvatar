using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using UnityEngine.Events;

namespace ZGame.UGUIExtention
{
    public class LongPressButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Image image;
        public float startTime = 0f;
        public float pressTime = 0f;
        public float maxPressTime = 1f;
        public bool isLongPress = false;
        public bool isPressEnd = false;

        public UnityEvent onPointerDown = new UnityEvent();
        public UnityEvent onPointerUp = new UnityEvent();



        // Start is called before the first frame update
        void Start()
        {
            if (image == null)
            {
                image = transform.Find("LongClickProgress").GetComponent<Image>();
            }

        }

        // Update is called once per frame
        void Update()
        {
            if (isLongPress)
            {
                pressTime += Time.deltaTime;
                image.fillAmount = pressTime / maxPressTime;

                if (pressTime >= maxPressTime)
                {
                    isLongPress = false;
                    isPressEnd = true;
                    pressTime = 0;
                }
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {
            onPointerDown?.Invoke();
            isLongPress = true;
            isPressEnd = false;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            onPointerUp?.Invoke();
            pressTime = 0;
            image.fillAmount = 0;
            isLongPress = false;
            isPressEnd = true;
        }
    }
}