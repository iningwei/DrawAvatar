using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//处理左对齐GridLayout 美观问题
//目前只支持x方向 
[ExecuteInEditMode]
public class BeautifyFlexibleGridLayout : MonoBehaviour
{
    public float initSpaceX = 0;

    RectTransform rect;
    GridLayoutGroup glg;
    float gridHolderWidth;
    float cellWidth;


    void Start()
    {
        rect = this.GetComponent<RectTransform>();
        glg = this.GetComponent<GridLayoutGroup>();

        if (glg.constraint != GridLayoutGroup.Constraint.Flexible)
        {
            Debug.LogError("error, this script only used for flexible");
        }
        else
        {
            this.doBeautify();
        }
    }

#if UNITY_EDITOR
    float duration = 0.2f;
    void Update()
    {
        duration -= Time.deltaTime;
        if (duration < 0)
        {
            this.doBeautify();
            duration = 0.2f;
        }
    }
#endif

    public bool avarageGap = false;
    void doBeautify()
    {
        cellWidth = glg.cellSize.x;
        gridHolderWidth = this.rect.rect.width;
        //计算能容纳的cell个数
        float realWidth = (gridHolderWidth - glg.padding.left - glg.padding.right);//减去左右padding
        float countF = (realWidth + initSpaceX) / (cellWidth + initSpaceX);
        //Debug.Log($"gridHolderWidth:{gridHolderWidth},realWidth:{realWidth}, count:{countF}");
        int count = (int)Mathf.Floor(countF);
        //把多余的宽度均分到spaceX上
        if (avarageGap)
        {
            if (count > 1)
            {
                float additionWidth = (countF - count) * cellWidth;//多余的宽度
                float gap = additionWidth / (count - 1);
                this.glg.spacing = new Vector2(initSpaceX + gap, this.glg.spacing.y);
                //Debug.Log($"additionWidth:{additionWidth}, gap:{gap}");
            }
        }
    }
}
