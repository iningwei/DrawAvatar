using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteExt
{
    public static Texture2D SpriteToTexture2D(this Sprite sprite)
    {
        var targetTex = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
        var pixels = sprite.texture.GetPixels(
            (int)sprite.textureRect.x,
            (int)sprite.textureRect.y,
            (int)sprite.textureRect.width,
            (int)sprite.textureRect.height);
        targetTex.SetPixels(pixels);
        targetTex.Apply();
        return targetTex;
    }
}
