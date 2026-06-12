using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.TimerTween;
using ZGame.Window;

/*
-- 逻辑流程
-- loading界面分两个阶段
-- 第一阶段：前N秒表演进度，随机到A%-B%，卡住
-- 第二阶段：确定收到loadingFinish，再用N秒表演完成剩余进度
*/
public class LoadingWindow : Window
{
    public Image ui_SliderImg;
    public TextMeshProUGUI ui_ProgressDesTxt;

    bool isGapFinished;
    bool isProgress1Finished = false;
    bool isProgress2Begin = false;
    float gap = 0;
    float progress1UsedSeconds = 1f * 1f;
    float progress2UsedSeconds = 0.5f * 1f;


    public LoadingWindow(GameObject obj, string windowName, string layerName, bool isExclusive, bool neverClose, params object[] datas) : base(obj, windowName, layerName, isExclusive, neverClose, datas)
    {
    }


    public override void Init(params object[] datas)
    {
        base.Init(datas);

        loadingFinishedCallback = (Action)datas[0];

        this.setProgressImgFillAmount(0);
        this.ui_ProgressDesTxt.text = "0%";
    }


    int fillType = 1;//0:filled;  1:imgWidth
    /// <summary>
    /// 
    /// </summary>
    /// <param name="progress">0-1</param>
    void setProgressImgFillAmount(float progress)
    {
        if (fillType == 0)
        {
            this.ui_SliderImg.fillAmount = progress;
        }
        else if (fillType == 1)
        {
            this.ui_SliderImg.GetComponent<RectTransform>().sizeDelta = new Vector2(progress * 848, 33);
        }
    }

    Action loadingFinishedCallback;
    public override void Show(params object[] paras)
    {
        base.Show(paras);

        isGapFinished = false;
        isProgress1Finished = false;
        isProgress2Begin = false;

        setProgressImgFillAmount(0f);
        this.ui_ProgressDesTxt.text = "0%";

        gap = RandomTool.NextInt(50, 80);
        //Debug.Log("loadingWindow show,gap:" + gap);
        this.showProgress1(progress1UsedSeconds);

        //Debug.LogError("set loadingFinishedCallback");


    }



    Timer progress1Timer;
    long progress1TimerId;
    void showProgress1(float loadingTime)
    {
        progress1Timer = TimerTween.ValueByInterval(0, gap, loadingTime, 0.03f, (v) =>
        {
            setProgressImgFillAmount(v / 100f);
            this.ui_ProgressDesTxt.text = (int)v + "%";
        }, () =>
        {
            isProgress1Finished = true;
            if (isGapFinished)
            {
                this.showProgress2(progress2UsedSeconds);
            }
        });
        progress1Timer.Start(out progress1TimerId);
    }


    Timer progress2Timer;
    long progress2TimerId;



    void showProgress2(float loadingTime)
    {
        if (isProgress2Begin)
        {
            return;
        }
        isProgress2Begin = true;

        progress2Timer = TimerTween.ValueByInterval(gap, 100, loadingTime, 0.03f, (v) =>
        {
            this.setProgressImgFillAmount(v / 100f);
            ui_ProgressDesTxt.text = (int)v + "%";
        }, () =>
        {
            setProgressImgFillAmount(1f);
            this.onLoadingFinished();
        });
        progress2Timer.Start(out progress2TimerId);

    }

    private void onLoadingFinished()
    {
        if (loadingFinishedCallback != null)
        {
            Debug.Log(" call loadingFinishedCallback");
            loadingFinishedCallback.Invoke();
            loadingFinishedCallback = null;
        }
        this.Close();
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        base.HandleMessage(msgId, paras);
        if (msgId == WindowMsgID.OnSceneLoadSuccess)
        {
            isGapFinished = true;
            if (isProgress1Finished)
            {
                this.showProgress2(this.progress2UsedSeconds);
            }
        }
    }

    public override void Destroy()
    {
        base.Destroy();


    }
}
