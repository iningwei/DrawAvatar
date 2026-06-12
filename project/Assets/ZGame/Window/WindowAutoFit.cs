using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Window;

public class WindowAutoFit
{
    /// <summary>
    /// UI竖屏适配 安全区
    /// https://blog.csdn.net/qq_39108767/article/details/114396833
    /// https://zhuanlan.zhihu.com/p/124246847
    /// android碎片化，下述代码可能会在有些机型上有问题：https://zhuanlan.zhihu.com/p/126699544
    /// </summary>
    public static void UIVerticalFitSafeArea()
    {
        return;//不需要对层节点设置offset，通过对各个window的unsafeBg来进行偏移实现自适应
               //顶部偏移
        int topOffset = GetVerticalAppSafeAreaTopOffset();
        //底部偏移
        int bottomOffset = GetVerticalAppSafeAreaBottomOffset();


        Debug.Log("safe area, topOffset:" + topOffset + ", bottomOffset:" + bottomOffset);
        if (topOffset > 0 || bottomOffset > 0)
        {
            //设置内置Layer层的上下偏移
            for (int i = 0; i < WindowLayer.LayerList.Count; i++)
            {
                var layerName = WindowLayer.LayerList[i];
                var layerTran = WindowLayer.GetTran(layerName);
                var rectTran = layerTran.GetComponent<RectTransform>();
                rectTran.offsetMin = new Vector2(rectTran.offsetMin.x, rectTran.offsetMin.y + bottomOffset);
                rectTran.offsetMax = new Vector2(rectTran.offsetMax.x, rectTran.offsetMax.y - topOffset);
            }
        }
        else
        {
            Debug.Log("no need set safe area！！");
        }
    }

    static Transform rootCanvasTran => WindowManager.Instance.GetRootCanvasTran();
    /// <summary>
    /// UI竖屏自适应PAD
    /// </summary>
    public static void UIVerticalFitPad()
    {
        //自适应基准分辨率比例为 Config.gameDesignRatio
        //即对于竖屏，当比这个分辨率比例更宽的机型，才对其左右进行黑边填充
        var ratios = ConfigUtility.Data.GameDesignRatio.Split(",");
        float x = float.Parse(ratios[0]);
        float y = float.Parse(ratios[1]);
        float gapRatio = x / y;
        Debug.Log("gameDesignRatio,x:" + x + ", y:" + y);

        //int screenWidth = Screen.width;
        //int screenHeight = Screen.height;
        //这里不能使用屏幕的宽高来算后续偏移，需要取主Canvas的宽高来算
        float screenWidth = rootCanvasTran.GetComponent<RectTransform>().sizeDelta.x;
        float screenHeight = rootCanvasTran.GetComponent<RectTransform>().sizeDelta.y;
        //需要注意的是，在PC上的软件，若分辨率大于屏幕的分辨率，那么会导致部分不显示。那么这里算出来的值实际上是显示区域的值。并不包含未显示区域的。


        float screenRatio = (float)screenWidth / screenHeight;
        if (screenRatio > gapRatio)
        {
            float suitWidth = gapRatio * screenHeight;
            float offsetX = (screenWidth - suitWidth) / 2;
            Debug.LogError($"do vertical fit pad, screenWidth:{screenWidth},screenHeight:{screenHeight},gapRatio:{gapRatio}, suitWidth:{suitWidth},offsetX:{offsetX}");
            //设置内置Layer层的左右偏移
            for (int i = 0; i < WindowLayer.LayerList.Count; i++)
            {
                var layerName = WindowLayer.LayerList[i];
                var layerTran = WindowLayer.GetTran(layerName);
                var rectTran = layerTran.GetComponent<RectTransform>();
                rectTran.offsetMin = new Vector2(offsetX, 0);
                rectTran.offsetMax = new Vector2(-offsetX, 0);
            }


            //添加遮罩层，并设置同样的左右偏移值
            GameObject padFitMaskObj = new GameObject();
            padFitMaskObj.name = "padFitMask";
            padFitMaskObj.transform.SetParent(rootCanvasTran);
            RectTransform padFitRectTran = padFitMaskObj.AddComponent<RectTransform>();
            padFitRectTran.localPosition = Vector3.zero;
            padFitRectTran.localScale = Vector3.one;
            padFitRectTran.anchorMin = Vector2.zero;
            padFitRectTran.anchorMax = Vector2.one;
            padFitRectTran.offsetMin = new Vector2(offsetX, 0);
            padFitRectTran.offsetMax = new Vector2(-offsetX, 0);

            //为遮罩层添加左右两边的黑色遮罩
            GameObject leftObj = new GameObject();
            leftObj.transform.SetParent(padFitRectTran);
            RectTransform leftTran = leftObj.AddComponent<RectTransform>();
            leftTran.localScale = Vector3.one;
            leftTran.localPosition = Vector3.zero;

            leftTran.anchorMin = new Vector2(0, 0);
            leftTran.anchorMax = new Vector2(0, 1);
            leftTran.pivot = new Vector2(1, 0.5f);
            leftTran.sizeDelta = new Vector2(5000, 0);
            leftTran.SetSiblingIndex(0);
            leftTran.name = "leftFitPadTran";
            var leftImg = leftTran.gameObject.AddComponent<Image>();
            leftImg.color = Color.black;

            GameObject rightObj = new GameObject();
            rightObj.transform.SetParent(padFitRectTran);
            RectTransform rightTran = rightObj.AddComponent<RectTransform>();
            rightTran.localScale = Vector3.one;
            rightTran.localPosition = Vector3.zero;

            rightTran.anchorMin = new Vector2(1, 0);
            rightTran.anchorMax = new Vector2(1, 1);
            rightTran.pivot = new Vector2(0, 0.5f);
            rightTran.sizeDelta = new Vector2(5000, 0);
            rightTran.SetSiblingIndex(1);
            rightTran.name = "rightFitPadTran";
            var rightImg = rightTran.gameObject.AddComponent<Image>();
            rightImg.color = Color.black;
        }
        else
        {
            Debug.Log("not do vertical fit pad");
        }

    }


    //经过实测，iphone11顶部非安全区高度为88，底部为68； iphone xs max顶部非安全区高度为132，底部为102
    //特别是iphone xs max若用实际的尺寸顶部会偏移很大，因此这里做个限定:上下非安全区最大高度分别为80、60
    /// <summary>
    /// 对于竖屏应用，获得其安全区顶部偏移
    /// </summary>
    /// <returns></returns>
    public static int GetVerticalAppSafeAreaTopOffset()
    {
        int topOffset = (int)(Screen.height - getSafeArea().yMax);

        if (topOffset < 0)
        {
            topOffset = 0;
        }
        if (topOffset > 80)
        {
            topOffset = 80;
        }
        //模拟顶部有偏移
        //topOffset = 88;
        return topOffset;
    }

    /// <summary>
    /// 对于竖屏应用，获得其安全区底部偏移
    /// </summary>
    /// <returns></returns>
    public static int GetVerticalAppSafeAreaBottomOffset()
    {
        int bottomOffset = (int)getSafeArea().yMin;
        if (bottomOffset < 0)
        {
            bottomOffset = 0;
        }
        if (bottomOffset > 60)
        {
            bottomOffset = 60;
        }
        //模拟底部有偏移
        //bottomOffset = 38;
        return bottomOffset;
    }

    static Rect getSafeArea()
    {
        Rect rec = Screen.safeArea;

        return rec;
    }
}
