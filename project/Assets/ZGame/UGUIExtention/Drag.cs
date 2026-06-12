using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Action onDragBegin;
    public Action<Vector2> onDrag;
    public Action onDragEnd;

    public Action<PointerEventData> onDragBegin_EventData;
    public Action<PointerEventData> onDrag_EventData;
    public Action<PointerEventData> onDragEnd_EventData;

    public void OnBeginDrag(PointerEventData eventData)
    {
        onDragBegin?.Invoke();
        onDragBegin_EventData?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
        onDrag_EventData?.Invoke(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        onDragEnd?.Invoke();
        onDragEnd_EventData?.Invoke(eventData);
    }
}
