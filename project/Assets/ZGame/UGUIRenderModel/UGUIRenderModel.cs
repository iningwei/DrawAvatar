using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public static class UGUIRenderModel
{
    static bool isEasyRender = false;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetModelObj"></param>
    /// <param name="rtSize"></param>
    /// <param name="visualRawImg"></param>
    /// <param name="refCamTran"></param>
    /// <param name="updateRef"></param>
    /// <param name="shaderAdditiveOffset">对于某些低饱和度颜色（蓝、绿），使用additive的shader后无法正常显示，这里做一个修正（240-255之间）
    ///  这个值越小，特效的异常度也就越低，但是RT的白色杂边就会越明显，请根据项目实际情况设置此值</param>
    public static void RenderToUGUIRawImage(this GameObject targetModelObj, Vector2 rtSize, RawImage visualRawImg, Transform refCamTran, bool updateRef = false,
        float shaderAdditiveOffset = 255, RenderTextureFormat renderTextureFormat = RenderTextureFormat.ARGBHalf)
    {
        if (isEasyRender)
        {
            var render = targetModelObj.GetOrAddComponent<GRenderEasy>();
            render.StartRender(rtSize, visualRawImg, refCamTran, shaderAdditiveOffset);
        }
        else
        {
            var render = targetModelObj.GetOrAddComponent<GRender>();
            render.StartRender(rtSize, visualRawImg, refCamTran, shaderAdditiveOffset, renderTextureFormat);
            render.SetUpdatePR2RefCam(updateRef);
        }
    }

    public static void RenderToTexture(this GameObject targetModelObj, Vector2 rtSize, Transform refCamTran, out RenderTexture whiteRTexture)
    {
        var render = targetModelObj.GetOrAddComponent<GRender>();
        whiteRTexture = render.StartRenderRT(rtSize, refCamTran);
    }

    public static void RenderToRawImg(this GameObject targetModelObj, RawImage rawImg)
    {
        var render = targetModelObj.GetOrAddComponent<GRender>();
        render.StartRenderRawImg(rawImg);
    }
}
