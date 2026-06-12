using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZGame.TimerTween;
using ZGame.Window;


[IgnoreWindowNameGather]
public class NetMaskWindow : Window
{
    Timer rollTimer;
    long rollTimerId;

    public Transform ui_bgTran;
    public Transform ui_rollTran;
    float eulerZ = 0f;

    public TextMeshProUGUI ui_versionTxt;
    public NetMaskWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }
    public override void Init(params object[] paras)
    {
        base.Init(paras);
        this.hideRoll();
    }
    public override void Show(params object[] paras)
    {
        base.Show(paras);
        this.ui_bgTran.gameObject.SetActive(false);
        this.ui_rollTran.gameObject.SetActive(false);

        //版本
        GameGlobal.Instance.SetAppVersionDes(this.ui_versionTxt);
    }

    void showBg(string tag)
    {
        //Debug.Log("show netmask bg");
        this.ui_bgTran.name = tag;
        this.ui_bgTran.gameObject.SetActive(true);
    }
    void hideBg()
    {
        //Debug.Log("hide netmask bg");
        this.ui_bgTran.gameObject.SetActive(false);
    }
    void showRoll()
    {
        //Debug.Log("show netmask roll");
        ui_rollTran.gameObject.SetActive(true);

        TimerTween.Cancel(rollTimer, rollTimerId);

        rollTimer = TimerTween.Repeat(0.03f, () =>
        {
            if (eulerZ - 0.03f <= -360f)
            {
                eulerZ += 360f;
            }
            eulerZ -= 2f;

            ui_rollTran.localEulerAngles = new Vector3(0, 0, eulerZ);
            return true;
        }, true).SetTag("NetMaskRollTimer");
        rollTimer.Start(out rollTimerId);

    }
    void hideRoll()
    {
        //Debug.Log("hide roll");
        TimerTween.Cancel(rollTimer, rollTimerId);
        ui_rollTran.gameObject.SetActive(false);
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        if (msgId == WindowMsgID.OnShowNetMask)
        {
            string tag = paras[0].ToString();
            this.showBg(tag);
        }
        else if (msgId == WindowMsgID.OnHideNetMask)
        {
            this.hideBg();
            this.hideRoll();
        }
        else if (msgId == WindowMsgID.OnShowNetMaskWithRoll)
        {
            string tag = paras[0].ToString();
            this.showBg(tag);
            this.showRoll();
        }
    }
    public override void Destroy()
    {
        base.Destroy();
        TimerTween.Cancel(rollTimer, rollTimerId);
    }
}
