using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[ExecuteInEditMode]
public class AutoChildHorizentalLayout : MonoBehaviour
{
    public float horizentalOffset = 10f;
    public ChildAlignment alignment = ChildAlignment.Center;

    List<Transform> childs = new List<Transform>();
    float duration = 0.1f;

    void Start()
    {
        this.autoLayout();
    }


    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)//TODO:看下有没有系统的UI界面刷新的监听
        {
            duration = 0.1f;
            this.autoLayout();
        }
    }


    void autoLayout()
    {
        this.transform.GetCompnentsInChildrenExceptSelf<Transform>(false, childs);
        float totalWidth = 0f;
        for (int i = 0; i < childs.Count; i++)
        {
            float width = childs[i].GetComponent<RectTransform>().sizeDelta.x;
            totalWidth += width;
        }
        totalWidth += (childs.Count - 1) * horizentalOffset;

        float startXPos = 0;
        RectTransform targetRect;
        switch (alignment)
        {
            case ChildAlignment.Left:
                Debug.LogError("not implement!");
                break;
            case ChildAlignment.Center:
                startXPos = -totalWidth * 0.5f;
                for (int i = 0; i < childs.Count; i++)
                {
                    targetRect = childs[i].GetComponent<RectTransform>();
                    float xValue = startXPos + targetRect.sizeDelta.x * 0.5f;
                    //refresh startXPos for next targetRect
                    startXPos = startXPos + targetRect.sizeDelta.x + horizentalOffset;

                    targetRect.anchoredPosition = new Vector2(xValue, targetRect.anchoredPosition.y);
                }
                break;
            case ChildAlignment.Right:
                Debug.LogError("not implement!");
                break;
        }


    }
}
