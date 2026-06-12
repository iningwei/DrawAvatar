using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialTextureSequence : MonoBehaviour
{
    public Texture[] textures;
    public float duration;

    public int index = 0;

    public MeshRenderer render;

    int count = 0;
    float durationValue = 0;
    public bool useRealtime = false;

    void Awake()
    {
        if (textures != null && textures.Length > 0)
        {
            count = textures.Length;
            if (textures[index]!=null)
            {
                render.material.SetTexture("_MainTex", textures[index]);
            }
            
        }

        durationValue = duration;
        
    }


    void Update()
    {
        if (duration <= 0 || count == 0)
        {
            return;
        }
        durationValue -= useRealtime ? Time.unscaledDeltaTime : Time.deltaTime;
        if (durationValue < 0)
        {
            if (textures[index] != null)
            {
                render.GetMaterial().SetTexture("_MainTex", textures[index]);
            }
            
            if (index + 1 == count)
            {
                index = 0;
            }
            else
            {
                index++;
            }

            durationValue = duration;
        }
    }
}
