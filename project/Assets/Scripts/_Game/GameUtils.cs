using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.Video;
using ZGame;
using ZGame.Event;
using ZGame.Ress;
using ZGame.Ress.AB;
using ZGame.TimerTween;
using ZGame.Window;
using ZGame.ZTable;

public class GameUtils
{
    public static bool isFirstEnterGame = true;
    //public static bool isFirstWSConnectedGame = true;





    public static void FillSprite(Image img, string atlasAndSpriteStr, bool setNativeSize = false)
    {
        if (string.IsNullOrEmpty(atlasAndSpriteStr))
        {
            return;
        }

        string[] strs = atlasAndSpriteStr.Split(":");
        if (strs.Length < 2)
        {
            Debug.Log("sprite name error:" + atlasAndSpriteStr + ", " + img.transform.GetHierarchy());
            return;
        }

        //if (img.sprite != null && img.sprite.texture != null && img.sprite.texture.name == strs[0] && img.sprite.name == strs[1])//相同的话则返回<该方法无法判断img当前sprite和目标设置是一样的。因为运行时，引擎会自动对图集进行合批处理，导致atlas名和原始名不一致，且sprite.name也是带(Clone)后缀的>；TODO：后续尽量保证每个图片名称的唯一性，这样可以通过sprite.name来判定
        //{
        //    Debug.LogError("same return,atalas:" + strs[0] + ",sprite:" + strs[1]);
        //    return;
        //}

        FillSprite(img, strs[0], strs[1], setNativeSize);
    }


    public static void FillSprite(Image img, string atlasName, string spriteName, bool setNativeSize = false)
    {
        if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
        {
            var sprites = Resources.LoadAll<Sprite>("Sprite/" + atlasName);
            Sprite targetSprite = null;
            foreach (var sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    targetSprite = sprite;
                    break;
                }
            }
            if (targetSprite != null && img != null)
            {
                img.sprite = targetSprite;
            }
        }
        else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
        {
            AAManager.Instance.LoadSprite(atlasName, ".spriteatlasv2", spriteName, (s) =>
            {
                if (img != null)
                {
                    img.sprite = s;
                    if (setNativeSize)
                    {
                        img.SetNativeSize();
                    }
                }

            });
        }

    }
    public static void FillSprite(SpriteRenderer sr, string atlasName, string spriteName)
    {
        if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Resources)
        {
            var sprites = Resources.LoadAll<Sprite>("Sprite/" + atlasName);
            Sprite targetSprite = null;
            foreach (var sprite in sprites)
            {
                if (sprite.name == spriteName)
                {
                    targetSprite = sprite;
                    break;
                }
            }
            if (targetSprite != null && sr != null)
            {
                sr.sprite = targetSprite;
            }
        }
        else if (ConfigUtility.Data.ResLoadType == (int)ResLoadType.Addressable)
        {
            AAManager.Instance.LoadSprite(atlasName, ".spriteatlas", spriteName, (s) =>
            {
                if (sr != null)
                {
                    sr.sprite = s;
                }
            });
        }
    }
    public static void FillSprite(SpriteRenderer sr, string atlasAndSpriteStr)
    {
        string[] strs = atlasAndSpriteStr.Split(":");
        if (strs.Length < 2)
        {
            Debug.Log("sprite name error:" + atlasAndSpriteStr + ", " + sr.transform.GetHierarchy());
            return;
        }
        FillSprite(sr, strs[0], strs[1]);
    }

    public static void ShowProtoCommonError(long result)
    {
        WindowUtil.ShowTip(99999, result);
    }

    /// <summary>
    /// 获得剩余天数的描述，从开始时间
    /// </summary>
    /// <param name="startStamp">开始的时间戳：秒</param>
    /// <returns></returns>
    public static float GetLeftDay(long startStamp, long totalSeconds)
    {
        float leftDay = 0;

        long leftSeconds = totalSeconds - (TimeTool.GetNowSecondStamp() - startStamp);
        if (leftSeconds > 0)
        {
            leftDay = leftSeconds / (3600f * 24);
            return leftDay;
        }

        return leftDay;
    }
    public static long[] GetLeftDHMS(long startStamp, long totalSeconds)
    {
        long day = 0;
        long hour = 0;
        long minute = 0;
        long second = 0;

        long leftSeconds = totalSeconds - (TimeTool.GetNowSecondStamp() - startStamp);
        if (leftSeconds > 0)
        {
            return TimeTool.FormatSecondsToDHMS(leftSeconds);
        }
        else
        {
            return new long[4] { day, hour, minute, second };
        }
    }

    public static long[] GetCountDownDHMS(long targetStamp)
    {
        long day = 0;
        long hour = 0;
        long minute = 0;
        long second = 0;

        long leftSeconds = targetStamp - TimeTool.GetNowSecondStamp();
        if (leftSeconds > 0)
        {
            return TimeTool.FormatSecondsToDHMS(leftSeconds);
        }
        else
        {
            return new long[4] { day, hour, minute, second };
        }
    }

    public static void FillLanguageToText(TextMeshProUGUI targetText, long id, params object[] paras)
    {
        targetText.text = BeanManager.Instance.GetLanguage(id, paras);
    }

    //////public static void FillQualityToText(TextMeshProUGUI targetText, int quality)
    //////{
    //////    QualityData qualityData = BeanManager.Instance.GetQualityData(quality);
    //////    long languageId = qualityData.nameLanguageId;
    //////    string colorCode = qualityData.colorCode;
    //////    string content = $"<color={colorCode}>{BeanManager.Instance.GetLanguage(languageId)}</color>";
    //////    targetText.text = content;
    //////}



    public static void OpenWindowBasically(string windowName, params object[] datas)
    {
        //////GameUtils.PlayAudio(6, false);
        WindowManager.Instance.ShowWindow(windowName, WindowLayer.Hud, false, false, true, null, datas);
    }



    public static async void PlayAudio(long id, bool isLoop)
    {
        var audioData = BeanManager.Instance.GetAudioData(id);
        if (audioData != null)
        {
            await AudioManager.Instance.PlaySound(audioData.name, audioData.extension, audioData.volume, audioData.existCountLimit, isLoop);
        }
    }

    public static void PlayVideo(VideoPlayer player, RawImage img, long id, Action onFinished)
    {
        var videoData = BeanManager.Instance.GetVideoData(id);
        if (videoData != null)
        {
            Debug.LogError("Trans:" + player.transform.name + " PlayVideo:" + videoData.name + ", id:" + id);
            VideoManager.Instance.PlayVideo(player, img, videoData.name, videoData.extension, videoData.volume, onFinished);
        }
        else
        {
            onFinished?.Invoke();
        }
    }

    public static void StopAudio(int audioSourceId)
    {
        AudioManager.Instance.StopSound(audioSourceId);
    }

    public static void PlayBGM(long id)
    {
        var audioData = BeanManager.Instance.GetAudioData(id);
        AudioManager.Instance.PlayBGM(audioData.name, audioData.extension, audioData.volume, true);
    }


    public static void PlayBtnClickedAudio()
    {
        //////PlayAudio(1, false);
    }

    public static void Quit()
    {
        Application.Quit();
    }


    static float rtObjXPosMin = -50000f;
    static float rtObjXPosMax = -3000f;
    static float lastRTXPos = rtObjXPosMin;
    public static void RenderObjToRawImg(GameObject targetObj, float scale, float yPixel, Vector2 rtSize, RawImage targetRawImg, Transform refCamNode)
    {
        targetObj.transform.position = new Vector3(lastRTXPos, 0, 0);
        targetObj.transform.localScale = Vector3.one;
        targetObj.transform.rotation = Quaternion.identity;
        var concrete = targetObj.transform.Find("concrete");
        if (concrete != null)
        {
            concrete.localScale = Vector3.one * scale;
            concrete.localPosition = new Vector3(0, yPixel, 0);
        }
        targetObj.SetActive(true);
        targetObj.name = targetObj.name + "_RTObj";

        targetObj.RenderToUGUIRawImage(rtSize, targetRawImg, refCamNode);

        lastRTXPos += 1500f;
        if (lastRTXPos > rtObjXPosMax)
        {
            lastRTXPos = rtObjXPosMin;
        }
    }

    public static void RenderModelToRawImg(string objName, float scale, float yPixel, Vector2 rtSize, RawImage targetRawImg, Action callback)
    {
        AAManager.Instance.LoadOtherPrefab(objName, (obj) =>
        {
            if (targetRawImg != null)
            {
                RenderObjToRawImg(obj, scale, yPixel, rtSize, targetRawImg, obj.transform.Find("camera"));
                callback?.Invoke();
            }

        });
    }







    public static void SetSequenceSprites(Image img, string atlasAndSpriteStr, int count, string joinStr = "_", int startIndex = 0)
    {
        img.gameObject.SetActive(false);
        string[] strs = atlasAndSpriteStr.Split(":");
        int loadedCount = 0;
        ImageSequence sequence = img.GetComponent<ImageSequence>();
        sequence.ResetSprites(new Sprite[count]);
        for (int i = 0; i < count; i++)
        {
            int index = i;
            AAManager.Instance.LoadSprite(strs[0], ".spriteatlas", strs[1] + joinStr + (i + startIndex).ToString("D3"), (s) =>
            {
                sequence.sprites[index] = s;
                loadedCount++;
                if (loadedCount == count)
                {
                    if (img != null)
                    {
                        img.gameObject.SetActive(true);
                        sequence.enabled = true;
                        sequence.Resume();
                    }
                }
            });
        }

    }

    /// <summary>
    /// 头像框与头衔  通用
    /// </summary>
    /// <param name="img"></param>
    /// <param name="atlasAndSpriteStr"></param>
    /// <param name="count"></param>
    public static void SetHeadSequenceSprites(Image img, string atlasAndSpriteStr, int count)
    {
        img.gameObject.SetActive(false);
        string[] strs = atlasAndSpriteStr.Split(":");
        int loadedCount = 0;
        ImageSequence sequence = img.GetComponent<ImageSequence>();
        sequence.ResetSprites(new Sprite[count]);
        for (int i = 0; i < count; i++)
        {
            int index = i;
            string spriteName = strs[1].Substring(0, strs[1].Length - 2) + i.ToString("D2");
            AAManager.Instance.LoadSprite(strs[0], ".spriteatlas", spriteName, (s) =>
            {
                sequence.sprites[index] = s;
                loadedCount++;
                if (loadedCount == count)
                {
                    if (img != null)
                    {
                        img.gameObject.SetActive(true);
                        sequence.enabled = true;
                        sequence.Resume();
                    }
                }
            });
        }

    }


    public static void SetUICamOverlayAndBelongBaseCam(Camera targetCam, Camera baseCam)
    {
        targetCam.GetUniversalAdditionalCameraData().renderType = CameraRenderType.Overlay;
        var camStack = baseCam.GetUniversalAdditionalCameraData().cameraStack;
        if (!camStack.Contains(targetCam))
        {
            camStack.Add(targetCam);

            EventDispatcher.Instance.DispatchEvent(EventID.OnUICameraOverlayAndStackSetFinished, baseCam);
        }
    }




    public static bool ItemIsFragment(long itemSubType)
    {
        return itemSubType == 2 || itemSubType == 3 || itemSubType == 4;
    }

    public static string FormatQianWan(long value)//格式化 万
    {
        if (value < 10000)
        {
            return value.ToString();
        }
        else if (value >= 10000)
        {
            return (value / 10000f).ToString("0.0") + "万";
        }
        else if (value >= 100000)
        {
            return (value / 100000f).ToString("0.0") + "十万";
        }
        else if (value >= 1000000)
        {
            return (value / 1000000f).ToString("0.0") + "百万";
        }
        return value.ToString();
    }

    public static string GetCareerName(long careerId)
    {
        switch (careerId)
        {
            case 1:
                return "步枪";
            case 2:
                return "狙击";
            case 3:
                return "霰弹";
            default:

                return "错误职业：" + careerId;
        }
    }


}


