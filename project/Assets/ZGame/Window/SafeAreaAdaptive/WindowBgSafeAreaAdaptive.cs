using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Window;


/// <summary>
/// 对窗体的背景进行安全区自适应处理
/// </summary>
public class WindowBgSafeAreaAdaptive : MonoBehaviour
{
    public RectTransform[] bgRectTransforms;

    void Start()
    {
        if (bgRectTransforms == null || bgRectTransforms.Length == 0)
        {
            return;
        }

        int topOffset = WindowAutoFit.GetVerticalAppSafeAreaTopOffset();
        int bottomOffset = WindowAutoFit.GetVerticalAppSafeAreaBottomOffset();

        if (topOffset > 0 || bottomOffset > 0)
        {
            foreach (var rec in bgRectTransforms)
            {
                rec.offsetMin = new Vector2(rec.offsetMin.x, rec.offsetMin.y - bottomOffset);
                rec.offsetMax = new Vector2(rec.offsetMax.x, rec.offsetMax.y + topOffset);
            }
        }

    }
}
