using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.TimerTween;
using ZGame.Window;

public class DynamicCfgWindow : Window
{
    //10秒内，依次点 3下 top，1下 bottom，1下 top；则触发打开设置界面
    public Button ui_TopBtn;
    public Button ui_BottomBtn;

    public TextMeshProUGUI ui_curLoginTypeTxt;
    public TextMeshProUGUI ui_curPackTypeTxt;


    public Transform ui_DynamicCfgSet;
    public Button ui_DynamicCfgConfirmBtn;
    public Button ui_DynamicCfgCancelBtn;

    public TMP_InputField ui_LoginTypeInputField;
    public TMP_InputField ui_PackTypeInputField;


    public Button ui_SetWhitelistBtn;
    public Button ui_UnsetWhitelistBtn;
    public TextMeshProUGUI ui_WhitelistStateTxt;


    int[] fingerActions = new int[5];
    int fingerActionIndex = 0;
    Timer checkFingerTimer;
    long checkFingerId;
    bool isStartCheckFingerAction = false;

    public DynamicCfgWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }

    public override void Show(params object[] paras)
    {
        base.Show(paras);

        this.ui_DynamicCfgSet.gameObject.SetActive(false);

        Debug.Log("curLoginType:" + ConfigUtility.Data.LoginType.ToString());
        this.ui_curLoginTypeTxt.text = "curLoginType:" + ConfigUtility.Data.LoginType.ToString();
        this.ui_curPackTypeTxt.text = "curPackType:" + ConfigUtility.Data.PackType.ToString();

        this.fillWhitelistStateTxt();
    }

    void fillWhitelistStateTxt()
    {
        this.ui_WhitelistStateTxt.text = "白名单：" + (PlayerPrefs.GetInt("IsWhitelist", 0) == 0 ? "NO" : "YES");

    }
    public override void AddEventListener()
    {
        base.AddEventListener();
        this.ui_TopBtn.onClick.AddListener(this.onTopBtnClicked);
        this.ui_BottomBtn.onClick.AddListener(this.onBottomBtnClicked);

        this.ui_DynamicCfgConfirmBtn.onClick.AddListener(this.onConfirmBtnClicked);
        this.ui_DynamicCfgCancelBtn.onClick.AddListener(this.onCancelBtnClicked);

        this.ui_SetWhitelistBtn.onClick.AddListener(this.onSetWhitelistBtnClicked);
        this.ui_UnsetWhitelistBtn.onClick.AddListener(this.onUnsetWhitelistBtnClicked);
    }

    private void onUnsetWhitelistBtnClicked()
    {
        PlayerPrefs.SetInt("IsWhitelist", 0);
        PlayerPrefs.Save();

        this.fillWhitelistStateTxt();
    }

    private void onSetWhitelistBtnClicked()
    {
        PlayerPrefs.SetInt("IsWhitelist", 1);
        PlayerPrefs.Save();

        this.fillWhitelistStateTxt();
    }

    private void onBottomBtnClicked()
    {
        if (isStartCheckFingerAction && fingerActionIndex < 5)
        {
            fingerActions[fingerActionIndex] = 1;
            fingerActionIndex++;

            if (fingerActionIndex == 5)
            {
                this.verifyFingerAction();
            }
        }
    }

    private void verifyFingerAction()
    {
        if (fingerActions[0] == 0 &&
            fingerActions[1] == 0 &&
            fingerActions[2] == 0 &&
            fingerActions[3] == 1 &&
            fingerActions[4] == 0)
        {
            this.ui_DynamicCfgSet.gameObject.SetActive(true);
            if (checkFingerTimer != null)
            {
                TimerTween.CancelAndExeculteCompleteCallbackTimer(checkFingerTimer, checkFingerId);
            }
        }
    }


    private void onTopBtnClicked()
    {
        if (fingerActionIndex < 5)
        {
            if (isStartCheckFingerAction == false)
            {
                isStartCheckFingerAction = true;

                checkFingerTimer = TimerTween.Delay(10, () =>
                {
                    isStartCheckFingerAction = false;
                    fingerActionIndex = 0;
                }).SetTag("checkFingerTimer");
                checkFingerTimer.Start(out checkFingerId);
            }

            fingerActions[fingerActionIndex] = 0;
            fingerActionIndex++;
            if (fingerActionIndex == 5)
            {
                verifyFingerAction();
            }
        }

    }

    private void onCancelBtnClicked()
    {
        this.ui_DynamicCfgSet.gameObject.SetActive(false);
    }

    private void onConfirmBtnClicked()
    {
        if (this.saveConfig())
        {
            //////GameUtils.DeleteAccountCache();
            this.ui_DynamicCfgSet.gameObject.SetActive(false);
            WindowUtil.ShowTip("保存成功，重启游戏生效！");
        }
    }


    bool saveConfig()
    {
        string loginType = this.ui_LoginTypeInputField.text.Trim().ToUpper();
        string packType = this.ui_PackTypeInputField.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(loginType) || (loginType != LoginType.PUB && loginType != LoginType.DEV))
        {
            WindowUtil.ShowTip("loginType输入非法");
            return false;
        }
        if (string.IsNullOrEmpty(packType) || (packType != PackType.PUB && packType != PackType.DEV))
        {
            WindowUtil.ShowTip("packType输入非法");
            return false;
        }

        //TODO:save
        //////Config.SaveDynamicConfig(
        //////    loginType,
        //////    packType,
        //////    Config.isRealPurchase.ToString().ToLower(),
        //////    Config.isShowProtoMsgLog.ToString().ToLower(),
        //////    Config.isShowDebugBtn.ToString().ToLower(),
        //////    Config.isEnableLogTrace.ToString().ToLower(),
        //////    Config.isEnableLogRealtimeWriteToLocal.ToString().ToLower(),
        //////    Config.isEnableLogUpdate2Server.ToString().ToLower(),
        //////    Config.isShowReporter.ToString().ToLower()
        //////    );
        return true;
    }
    public override void RemoveEventListener()
    {
        base.RemoveEventListener();

        this.ui_TopBtn.onClick.RemoveAllListeners();
        this.ui_BottomBtn.onClick.RemoveAllListeners();


        this.ui_DynamicCfgConfirmBtn.onClick.RemoveAllListeners();
        this.ui_DynamicCfgCancelBtn.onClick.RemoveAllListeners();

        this.ui_SetWhitelistBtn.onClick.RemoveAllListeners();
        this.ui_UnsetWhitelistBtn.onClick.RemoveAllListeners();
    }
}
