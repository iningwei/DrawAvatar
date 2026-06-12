using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public static class Texture2DExt
{
    public static Sprite Texture2DToSprite(this Texture2D tex)
    {
        int width = tex.width;
        int height = tex.height; 
        Sprite s = Sprite.Create(tex, new Rect(0, 0, width, height), Vector2.zero); 
        return s;
    }


    //扩展方法名为Resize的扩展方法Unity库已经自带了,且其返回类型为bool，并已经属于 Obsolete 方法了
    //官方推荐使用Reinitialize方法来替代
    //因此这里命名为ResizeSelf
    public static Texture2D ResizeSelf(this Texture2D tex, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, tex.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = tex.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    public static void SaveAsPNG(this Texture2D tex, string targetPath)
    {
        var b = tex.EncodeToPNG();
        File.WriteAllBytes(targetPath, b);
    }
}
