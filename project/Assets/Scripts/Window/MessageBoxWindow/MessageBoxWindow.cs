using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame.Window;

public enum MessageBoxBtnsLayout
{
    None,//没有按钮。常用于违规行为弹出。
    OnlyConfirm,
    OnlyCancel,
    Both,
}

public class MessageBoxWindow : Window
{
    public Image ui_bg;
    public RectTransform ui_FrameTran;
    public TextMeshProUGUI ui_TitleTxt;
    public TextMeshProUGUI ui_ContentTxt;
    public Button ui_CancelBtn;
    public Button ui_ConfirmBtn;
    public Button ui_CloseBtn;

    Action confirmCallback;
    string tipTitleStr;
    MessageBoxBtnsLayout btnsLayout = MessageBoxBtnsLayout.Both;

    public MessageBoxWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }

    float height;
    public override void Init(params object[] paras)
    {
        base.Init(paras);

        ui_ContentTxt.text = paras[0].ToString();
        confirmCallback = (Action)paras[1];
        btnsLayout = (MessageBoxBtnsLayout)paras[2];
        tipTitleStr = paras[3].ToString();
        height = (float)paras[4];
        float bgAlpha = (float)paras[5];

        ui_bg.color = new Color(0, 0, 0, bgAlpha / 255f);
    }

    public override void Show(params object[] paras)
    {
        base.Show(paras);




        switch (btnsLayout)
        {
            case MessageBoxBtnsLayout.None:
                this.ui_ConfirmBtn.gameObject.SetActive(false);
                this.ui_CancelBtn.gameObject.SetActive(false);
                break;
            case MessageBoxBtnsLayout.OnlyConfirm:
                this.ui_ConfirmBtn.gameObject.SetActive(true);
                this.ui_CancelBtn.gameObject.SetActive(false);
                break;
            case MessageBoxBtnsLayout.OnlyCancel:

                this.ui_ConfirmBtn.gameObject.SetActive(false);
                this.ui_CancelBtn.gameObject.SetActive(true);
                break;
            case MessageBoxBtnsLayout.Both:
                this.ui_ConfirmBtn.gameObject.SetActive(true);
                this.ui_CancelBtn.gameObject.SetActive(true);
                break;
            default:
                break;
        }
        this.ui_TitleTxt.text = tipTitleStr;
        GameUtils.FillLanguageToText(ui_CancelBtn.GetComponentInChildren<TextMeshProUGUI>(), 1000002);
        GameUtils.FillLanguageToText(ui_ConfirmBtn.GetComponentInChildren<TextMeshProUGUI>(), 1000001);
        ui_FrameTran.sizeDelta = new Vector2(792, height);

    }

    public override void AddEventListener()
    {
        base.AddEventListener();
        this.ui_ConfirmBtn.onClick.AddListener(this.onConfirmBtnClicked);
        this.ui_CancelBtn.onClick.AddListener(this.onCancelBtnClicked);
        this.ui_CloseBtn.onClick.AddListener(this.onCloseBtnClicked);
    }

    private void onCloseBtnClicked()
    {
        GameUtils.PlayBtnClickedAudio();
        this.Close();
    }

    private void onCancelBtnClicked()
    {
        GameUtils.PlayBtnClickedAudio();
        this.Close();
    }


    private void onConfirmBtnClicked()
    {
        GameUtils.PlayBtnClickedAudio();
        confirmCallback?.Invoke();
        this.Close();
    }

    public override void RemoveEventListener()
    {
        base.RemoveEventListener();

        this.ui_ConfirmBtn.onClick.RemoveAllListeners();
        this.ui_CancelBtn.onClick.RemoveAllListeners();
        this.ui_CloseBtn.onClick.RemoveAllListeners();
    }

    public override void Destroy()
    {
        base.Destroy();

        confirmCallback = null;
    }

}
