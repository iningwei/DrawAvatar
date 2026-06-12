using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;
using ZGame;

public class CircleImage : Image
{
    public float fillPercent = 1f;       //填充比例
    public int fillNum = 100;            //填充个数
    private List<Vector2> outerVertexs;  //圆上顶点列表
    public float radiusRatio = 1f;
    protected override void Awake()
    {
        base.Awake();
        outerVertexs = new List<Vector2>();
    }

    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (overrideSprite == null)
        {
            base.OnPopulateMesh(toFill);
            return;
        }

        switch (type)
        {
            case Type.Simple:
                GenerateSimpleSprite(toFill, preserveAspect);
                break;
        }
    }

    void GenerateSimpleSprite(VertexHelper vh, bool lPreserveAspect)
    {
        vh.Clear();
        outerVertexs.Clear();

        //计算准备
        Vector4 uv = (overrideSprite != null) ? DataUtility.GetOuterUV(overrideSprite) : Vector4.zero;

        float degreeDelta = 2 * Mathf.PI / fillNum;
        int curNum = (int)(fillNum * fillPercent);
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height;
        float radius = width * 0.5f * radiusRatio;

        float uvCenterX = (uv.x + uv.z) * 0.5f;
        float uvCenterY = (uv.y + uv.w) * 0.5f;
        float uvScaleX = (uv.z - uv.x) / width;
        float uvScaleY = (uv.w - uv.y) / height;

        //添加第一个点
        UIVertex uiVertex = new UIVertex();
        uiVertex.color = color;
        uiVertex.position = Vector2.zero;
        uiVertex.uv0 = new Vector2(uvCenterX, uvCenterY);
        vh.AddVert(uiVertex);

        //添加圆上的点
        int vertNum = (fillPercent == 1) ? curNum : curNum + 1;
        for (int i = 1; i <= vertNum; i++)
        {
            float curDegree = (i - 1) * degreeDelta;
            float cosA = Mathf.Cos(curDegree);
            float sinA = Mathf.Sin(curDegree);
            Vector2 curVertice = new Vector2(cosA * radius, sinA * radius);

            uiVertex = new UIVertex();
            uiVertex.color = color;
            uiVertex.position = curVertice;
            uiVertex.uv0 = new Vector2(uvCenterX + curVertice.x * uvScaleX, uvCenterY + curVertice.y * uvScaleY);
            vh.AddVert(uiVertex);

            outerVertexs.Add(curVertice);
        }

        //连接点
        for (int i = 1; i < vertNum; i++)
        {
            vh.AddTriangle(0, i + 1, i);
        }
        if (fillPercent == 1)
        {
            vh.AddTriangle(0, 1, curNum);
        }

        //连接点击区域
        if (fillPercent < 1)
        {
            outerVertexs.Add(Vector2.zero);
        }
    }

    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        Sprite sprite = overrideSprite;
        if (sprite == null)
            return true;

        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

        return MathTool.IsPointInPolygon(local, outerVertexs);
    }
}
