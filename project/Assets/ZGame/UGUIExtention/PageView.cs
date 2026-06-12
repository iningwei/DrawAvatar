using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//https://blog.csdn.net/qq_45777409/article/details/132188894
[RequireComponent(typeof(ScrollRect))]
public class PageView : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    private ScrollRect rect;                        //滑动组件  
    private float targethorizontal = 0;             //滑动的起始坐标  
    private bool isDrag = false;                    //是否拖拽结束  
    private List<float> posList = new List<float>();//求出每页的临界角，页索引从0开始  
    private int currentPageIndex = -1;
    public Action<int> OnPageChanged;
    public RectTransform content;
    private bool stopMove = true;
    public float smooting = 2;                      //滑动速度  
    public float sensitivity = 0.3f;
    private float startTime;

    private float startDragHorizontal;
    public Transform toggleList;

    bool isInited = false;
    public void Init()
    {
        rect = transform.GetComponent<ScrollRect>();
        var pageRectTran = GetComponent<RectTransform>();
        var tempWidth = ((float)content.transform.childCount * pageRectTran.rect.width);
        content.sizeDelta = new Vector2(tempWidth, pageRectTran.rect.height);

        //未显示的长度
        float horizontalLength = content.rect.width - pageRectTran.rect.width;
        for (int i = 0; i < rect.content.transform.childCount; i++)
        {
            posList.Add(pageRectTran.rect.width * i / horizontalLength);
        }

        isInited = true;
        this.currentPageIndex = 0;
    }

    public int GetCurrentPageIndex()
    {
        return this.currentPageIndex;
    }

    void Update()
    {
        if (!isDrag && !stopMove)
        {
            startTime += Time.deltaTime;
            float t = startTime * smooting;

            float temp = Mathf.SmoothStep(0, 1, t);
            rect.horizontalNormalizedPosition = Mathf.Lerp(rect.horizontalNormalizedPosition, targethorizontal, temp);
            if (t >= 1)
                stopMove = true;
        }
    }

    public void ScrollToPage(int index, bool immediately = false)
    {
        if (index >= 0 && index < posList.Count)
        {
            if (immediately)
            {
                rect.horizontalNormalizedPosition = posList[index];
            }
            else
            {
                targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值  
                isDrag = false;
                startTime = 0;
                stopMove = false;
            }

            SetPageIndex(index);
        }
    }

    public int GetPageCount()
    {
        return posList.Count;
    }

    private void SetPageIndex(int index)
    {
        if (currentPageIndex != index)
        {
            currentPageIndex = index;
            if (OnPageChanged != null)
                OnPageChanged(index);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isInited)
        {
            return;
        }
        isDrag = true;
        //开始拖动
        startDragHorizontal = rect.horizontalNormalizedPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float posX = rect.horizontalNormalizedPosition;
        posX += ((posX - startDragHorizontal) * sensitivity);
        posX = posX < 1 ? posX : 1;
        posX = posX > 0 ? posX : 0;
        int index = 0;
        float offset = Mathf.Abs(posList[index] - posX);

        for (int i = 1; i < posList.Count; i++)
        {
            float temp = Mathf.Abs(posList[i] - posX);
            if (temp < offset)
            {
                index = i;
                offset = temp;
            }
        }
        SetPageIndex(index);
        targethorizontal = posList[index]; //设置当前坐标，更新函数进行插值  
        isDrag = false;
        startTime = 0;
        stopMove = false;
    }


}
