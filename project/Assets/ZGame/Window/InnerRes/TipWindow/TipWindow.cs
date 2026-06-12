using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.TimerTween;
using ZGame.Window;


public class TipData
{
    public string content;
    public float duration;
    public TipData(string content, float duration)
    {
        this.content = content;
        this.duration = duration;
    }
}

[IgnoreWindowNameGather]
public class TipWindow : Window
{
    public Transform ui_TipTran;

    public TextMeshProUGUI ui_TipTxt;


    Queue<TipData> tipQueue = new Queue<TipData>();

    void showTip(TipData tip)
    {
        this.delay = tip.duration;

        this.ui_TipTran.gameObject.SetActive(true);
        this.ui_TipTxt.text = tip.content;
    }



    public override void Init(params object[] paras)
    {
        base.Init(paras);
        ui_TipTran.gameObject.SetActive(false);
    }
    public override void Show(params object[] paras)
    {
        base.Show(paras);
    }

    float delay = 1f;

    public TipWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        delay -= dt;
        if (delay < 0)
        {
            if (this.ui_TipTxt.gameObject.activeSelf)
            {
                this.ui_TipTran.gameObject.SetActive(false);
            }

            if (this.tipQueue.Count > 0)
            {
                this.showTip(this.tipQueue.Dequeue());
            }
        }
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        if (msgId == WindowMsgID.OnAddTip)
        {
            var tipContent = paras[0].ToString();
            float duration = (float)paras[1];
            bool ignoreWhileSameAsBefore = (bool)paras[2];
            if (ignoreWhileSameAsBefore && this.tipQueue.Count > 0)
            {
                var before = this.tipQueue.Peek().content;
                if (before == tipContent)
                {
                    return;
                }
            }
            this.tipQueue.Enqueue(new TipData(tipContent, duration));
        }
    }
}
