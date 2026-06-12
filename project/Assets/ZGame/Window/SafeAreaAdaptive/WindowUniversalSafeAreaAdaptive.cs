using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame;
using ZGame.Window;

/// <summary>
/// 窗体通用的安全区自适应
/// 当窗体有unsafeBg子物体时，会对他们进行自适应处理
/// 该组件会在窗体显示的时候自动添加，无需手动添加
/// </summary>
public class WindowUniversalSafeAreaAdaptive : MonoBehaviour
{
    public RectTransform bgRectTransform;

    void Start()
    {
        int topOffset = WindowAutoFit.GetVerticalAppSafeAreaTopOffset();
        int bottomOffset = WindowAutoFit.GetVerticalAppSafeAreaBottomOffset();

        var unsafeBg = transform.Find("unsafeBg");
        if (unsafeBg != null)
        {
            bgRectTransform = unsafeBg.GetComponent<RectTransform>();
        }

        if (topOffset > 0 || bottomOffset > 0)
        {
            if (bgRectTransform != null)
            {
                if (ConfigUtility.Data.ScreenOrientation == (int)ScreenOrientation.Portrait)
                {
                    bgRectTransform.offsetMin = new Vector2(bgRectTransform.offsetMin.x, bgRectTransform.offsetMin.y + bottomOffset);
                    bgRectTransform.offsetMax = new Vector2(bgRectTransform.offsetMax.x, bgRectTransform.offsetMax.y - topOffset);
                }
                else if (ConfigUtility.Data.ScreenOrientation == (int)ScreenOrientation.Landscape)
                {
                    bgRectTransform.offsetMin = new Vector2(bgRectTransform.offsetMin.x + bottomOffset, bgRectTransform.offsetMin.y);
                    bgRectTransform.offsetMax = new Vector2(bgRectTransform.offsetMax.x - topOffset, bgRectTransform.offsetMax.y);
                }
                else
                {
                    Debug.LogError("Config screenOrientation error, only support Portrait or Landscape");
                }

            }
        }
    }



}
