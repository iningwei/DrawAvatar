using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private RectTransform rectTransform;
    private Vector2 pointerOffset;  // 鼠标点击点与对象中心的偏移

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // 计算鼠标点击点相对于对象的偏移
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out pointerOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 将屏幕坐标转换为本地坐标
        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointerPosition))
        {
            // 更新位置 = 当前鼠标位置 - 初始偏移量
            rectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }
}
