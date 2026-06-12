using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZGame.Window;

public class CursorWindow : Window
{
    public RectTransform ui_cursorImg;
    public Canvas rootCanvas;


    public CursorWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }
    public override void Show(params object[] paras)
    {
        base.Show(paras);
        rootCanvas = WindowManager.Instance.GetRootCanvas();
        Cursor.visible = false;//禁用系统内置鼠标指针
    }

    Vector2 localPoint;
    public override void Update(float dt)
    {
        base.Update(dt);
        if (rootCanvas)
        {
            // 将屏幕鼠标位置转换为 Canvas 内的本地位置
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.transform as RectTransform,
                Input.mousePosition,
                rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
                out localPoint
            );

            ui_cursorImg.localPosition = localPoint;
        }
    }
}