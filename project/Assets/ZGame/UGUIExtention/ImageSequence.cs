using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageSequence : MonoBehaviour
{
    public Sprite[] sprites;
    public float duration;
    int index = 0;


    public bool isLoop = true;
    public float lastFramePauseDuration = 0;//最后一帧动画播放后，停留的时间。一般在isLoop为true时才使用
    float lastFramePausedTime = 0f;
    public Action playFinishedCallback;//一般非loop状态才用得到

    Image image;
    int _count;
    public int count
    {
        get
        {
            if (sprites != null && sprites.Length > 0)
            {
                _count = sprites.Length;
            }
            return _count;
        }
        set
        {
            _count = value;
        }

    }
    float durationValue = 0;

    bool isQuit = false;
    public bool useRealtime = false;

    void OnEnable()
    {
        if (image == null)
        {
            image = this.gameObject.GetComponent<Image>();
        }

        durationValue = duration;
        isQuit = false;
        if (sprites != null && sprites.Length > 0)
        {
            image.sprite = sprites[index];
        }
    }
    bool flag = true;
    public void Stop()
    {
        flag = false;
    }
    public void ResetSprites(Sprite[] sprites)
    {
        this.sprites = sprites;
        this.count = this.sprites.Length;
        this.index = 0;
    }
    public void Resume()
    {
        flag = true;
    }

    void Update()
    {
        if (duration <= 0 || count == 0 || isQuit || flag == false)
        {
            return;
        }
        durationValue -= useRealtime ? Time.unscaledDeltaTime : Time.deltaTime; ;
        if (durationValue < 0)
        {
            if (index + 1 == count)
            {
                if (isLoop == false)
                {
                    index = 0;
                    isQuit = true;
                    if (this.playFinishedCallback != null)
                    {
                        this.playFinishedCallback();
                    }
                    return;
                }
                else
                {
                    if (lastFramePauseDuration > 0f)
                    {
                        lastFramePausedTime += useRealtime ? Time.unscaledDeltaTime : Time.deltaTime;

                        if (lastFramePausedTime > lastFramePauseDuration)
                        {
                            index = 0;
                            lastFramePausedTime = 0f;
                            durationValue = duration;
                        }
                    }
                    else
                    {
                        index = 0;
                        durationValue = duration;
                    }
                }
            }
            else
            {
                index++;
                durationValue = duration;
            }
            try
            {
                image.sprite = sprites[index];
            }
            catch (Exception ex)
            {
                Debug.LogError("error, index:" + index + ", length:" + sprites.Length + ", ex:" + ex.ToString());
            }

        }
    }
}