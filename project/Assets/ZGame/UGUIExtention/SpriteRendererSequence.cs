using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRendererSequence : MonoBehaviour
{
    public Sprite[] sprites;
    public float duration;

    public int index = 0;

    public SpriteRenderer render;

    int count = 0;
    float durationValue = 0;
    public bool useRealtime = false;

    void Awake()
    {
        if (sprites != null && sprites.Length > 0)
        {
            count = sprites.Length;
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
            render.sprite = sprites[index];
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
