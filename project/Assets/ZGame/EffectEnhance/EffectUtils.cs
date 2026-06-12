using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Ress.AB;
using ZGame.TimerTween;
using ZGame.Window;



public class EffectEntity
{
    public int id;
    public Transform holder;

}

public class EffectUtils : Singleton<EffectUtils>
{

    Queue<int> effectIds = new Queue<int>();

    List<EffectEntity> effectEntities = new List<EffectEntity>();
    public EffectUtils()
    {
        for (int i = 1; i <= 50; i++)
        {
            effectIds.Enqueue(i);
        }
    }

    public void PlayEffectOnce(Transform holder, string effectName, int effectSize, float time, Action onPlayFinished)
    {
        this.playEffect(holder, effectName, effectSize);

        TimerTween.Delay(time, () =>
        {
            //有可能在Delay期间，holder因为各种原因被清除            
            if (holder != null)
            {
                if (holder.childCount > 0)
                {
                    onPlayFinished?.Invoke();
                    this.ClearEffect(holder);
                }
            }
        }).Start();
    }

    public void PlayEffectCircle(Transform holder, string effectName, int effectSize)
    {
        this.playEffect(holder, effectName, effectSize);
    }

    void playEffect(Transform holder, string effectName, int effectSize)
    {
        if (effectIds.Count == 0)
        {
            Debug.LogError("error , no more effect id left");
            return;
        }


        int rtSize = effectSize;
        //全屏播放
        if (effectSize == -1)
        {
            effectSize = 1920;
        }



        if (effectSize > 1024)
        {
            rtSize = 1024;
        }




        int id = effectIds.Dequeue();
        EffectEntity entity = new EffectEntity();
        entity.id = id;
        entity.holder = holder;
        effectEntities.Add(entity);

        Debug.Log($"playEffect,effectName:{effectName},id:{id}");

        //Gen rawImageObj and rawImage
        GameObject rawImageObj = new GameObject();
        rawImageObj.name = "RawImageForRT";
        rawImageObj.transform.parent = holder;
        rawImageObj.transform.localPosition = Vector3.zero;
        rawImageObj.transform.localScale = Vector3.one;
        RawImage rawImage = rawImageObj.AddComponent<RawImage>();
        rawImage.material = Resources.Load("RawImageRenderTexture", typeof(Material)) as Material;


        //Gen Camera for RT
        GameObject cameraObj = new GameObject();
        cameraObj.name = "CameraForRT";
        cameraObj.transform.parent = rawImageObj.transform;
        cameraObj.transform.localPosition = new Vector3(1f, 1f, 0f) * 2000 * (id + 2f);
        cameraObj.transform.localScale = Vector3.one;
        Camera camera = cameraObj.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.orthographic = true;
        camera.orthographicSize = effectSize / 100f / 2f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 100f;
        camera.cullingMask = 1 << 5;




        //Gen rendererTexture
        RenderTexture rt = new RenderTexture(rtSize, rtSize, 0, RenderTextureFormat.ARGB32);
        rt.autoGenerateMips = false;
        rt.useMipMap = false;
        rt.name = effectName + "RT";

        camera.targetTexture = rt;

        rawImage.texture = rt;
        rawImage.raycastTarget = false;
        rawImage.GetComponent<RectTransform>().sizeDelta = new Vector2(effectSize, effectSize);
        rawImageObj.transform.localScale = Vector3.one;
        camera.backgroundColor = new Color(0, 0, 0, 0);

        ABManager.Instance.LoadEffect(effectName, (obj) =>
        {
            GameObject effect = obj as GameObject;
            effect.transform.parent = cameraObj.transform;
            effect.transform.localPosition = new Vector3(0, 0, 3000);
            effect.transform.localScale = Vector3.one * (1f / WindowManager.Instance.GetRootCanvasTran().lossyScale.x);
            effect.name = effectName;
        }, true);






    }
    public void ClearEffect(Transform holder)
    {

        for (int i = 0; i < effectEntities.Count; i++)
        {
            if (effectEntities[i].holder == holder)
            {
                effectIds.Enqueue(effectEntities[i].id);
                effectEntities.RemoveAt(i);
                break;
            }
        }


        if (holder.childCount == 0 || holder.Find("RawImageForRT/CameraForRT") == null)
        {
            return;
        }

        GameObject effObj = holder.Find("RawImageForRT/CameraForRT").GetChild(0).gameObject;
        GameObject.DestroyImmediate(effObj);

        Transform rawImageTran = holder.Find("RawImageForRT");
        var rt = rawImageTran.GetComponent<RawImage>().texture as RenderTexture;


        GameObject.DestroyImmediate(rawImageTran.gameObject);

        GameObject.DestroyImmediate(rt);
        rt = null;


    }


}
