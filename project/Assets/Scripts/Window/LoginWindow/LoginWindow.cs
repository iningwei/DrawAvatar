 
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI; 
using ZGame.Event;
using ZGame.SDK;
using ZGame.TimerTween;
using ZGame.Window;
using ZGame.ZTable;

public class LoginWindow : Window
{
    public TMP_InputField ui_RoomIdInputField;
    public TMP_InputField ui_UIDInputField;
    public Toggle ui_UIDToggle;
    public Toggle ui_RoomIdToggle;
    public Transform ui_LoginPanel;
    public Button ui_LoginBtn;


    public Button ui_SetBtn;

    public TextMeshProUGUI ui_KSTipTxt;
    string uid;
    string roomId;
    string ksCODE = "";
    string dyStartToken = "";


    int gameCameraEntityId;
    public LoginWindow(GameObject obj, string windowName, string layerName, bool isExclusive, bool neverClose, params object[] datas) : base(obj, windowName, layerName, isExclusive, neverClose, datas)
    {
    }

    public override void Show(params object[] paras)
    {
        base.Show(paras);

        ui_LoginPanel.gameObject.SetActive(false);
        ui_KSTipTxt.gameObject.SetActive(false);
        ui_LoginBtn.gameObject.SetActive(false);

        if (ConfigUtility.Data.LoginType == LoginType.PUB)//外网的话，从serverList获得真正的LoginType: PUB或者PUBTEST
        {
            string serverListLoginType = ServerList.Instance.AppMsgDic[ConfigUtility.Data.AppVersion].LoginType;
            Debug.Log("local loginType:" + ConfigUtility.Data.LoginType + ", serverList LoginType:" + serverListLoginType);
            ConfigUtility.Data.LoginType = serverListLoginType;
        }

        string uid = PlayerPrefs.GetString("InitUid", "");
        string roomId = PlayerPrefs.GetString("InitRoomId", "");
        if (string.IsNullOrEmpty(uid) == false)
        {
            this.ui_UIDInputField.text = uid;
        }
        if (string.IsNullOrEmpty(roomId) == false)
        {
            this.ui_RoomIdInputField.text = roomId;
        }

        roomIdToggleChanged(false);
        uidToggleChanged(false);

        if (ConfigUtility.Data.GameChannelId == (int)GameChannelId.KuaiShou)//快手
        {
            //快手启动参数文档：https://docs.qingque.cn/d/home/eZQBKcWGn8haL5IaPNM8Xzqsg?identityId=1oEBZ8mXUKh#section=h.yash8uml08pc 
            var param = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < param.Length; i++)
            {
                Debug.LogError($"commandLineArgs,{i}: " + param[i]);
                if (param[i] == "-c" && i + 1 < param.Length)// 确保 -c 后有值
                {
                    //设置快手code
                    ksCODE = param[i + 1]; // 获取 -c 后面的值
                    Debug.LogError("ksCode:" + ksCODE);
                    GameGlobal.Instance.ksCode = ksCODE;
                    break;
                }
            }


#if UNITY_EDITOR
            ui_LoginPanel.gameObject.SetActive(true);
            ui_LoginBtn.gameObject.SetActive(true);
#else
         if (ConfigUtility.Data.LoginType == LoginType.PUB || ConfigUtility.Data.LoginType == LoginType.PUBTEST)
        {
            if (string.IsNullOrEmpty(ksCODE))//未取到ksCODE
            {
                ui_KSTipTxt.gameObject.SetActive(true);
            }
            else
            {
                ui_LoginBtn.gameObject.SetActive(true);
            }
        }
        else
        {
            ui_LoginPanel.gameObject.SetActive(true);
            ui_LoginBtn.gameObject.SetActive(true);
        }
#endif
        }
        else if (ConfigUtility.Data.GameChannelId == (int)GameChannelId.DouYin)//抖音
        {
            var param = System.Environment.GetCommandLineArgs();
            string dyStartTokenHeader = "-token=";
            for (int i = 0; i < param.Length; i++)
            {
                Debug.LogError($"commandLineArgs,{i}: " + param[i]);
                if (param[i].Contains(dyStartTokenHeader))
                {
                    dyStartToken = param[i].Substring(dyStartTokenHeader.Length);
                }
            }

            if (string.IsNullOrEmpty(dyStartToken))
            {
                ui_LoginPanel.gameObject.SetActive(true);
            }
            else
            {
                ui_LoginPanel.gameObject.SetActive(false);
            }
            ui_LoginBtn.gameObject.SetActive(true);
        }


        //GameGlobal.Instance.SetMouseCursor(); 

#if !UNITY_EDITOR
        //显示cursor窗体
        WindowManager.Instance.ShowWindow(WindowNames.CursorWindow, WindowLayer.Top, false, true, false, null);
#endif

        WindowManager.Instance.ShowWindow(WindowNames.DynamicCfgWindow, WindowLayer.Hud, false, false, true, null);


        //GameUtils.PlayBGM(100000);
    }

    private void enterPreloadProcedure()
    {
        SceneUtils.LoadScene("Main", null, () =>
        { 
            this.Close();
        });
    }

   

    public override void AddEventListener()
    {
        base.AddEventListener();
        this.ui_LoginBtn.onClick.AddListener(this.onLoginBtnClicked);

        ui_UIDToggle.onValueChanged.AddListener(uidToggleChanged);
        ui_RoomIdToggle.onValueChanged.AddListener(roomIdToggleChanged);


        this.ui_SetBtn.onClick.AddListener(this.onSetBtnClicked);
        EventDispatcher.Instance.AddListener(EventID.OnKSCodeReceived, this.onKSCodeReceived);
    }

    private void onKSCodeReceived(string evtId, object[] paras)
    {
        string code = paras[0].ToString();
        this.ksCODE = code;
        this.ui_LoginBtn.gameObject.SetActive(true);
        this.ui_KSTipTxt.gameObject.SetActive(false);
    }

    private void onSetBtnClicked()
    {
        WindowManager.Instance.ShowWindow(WindowNames.SettingWindow, WindowLayer.Hud3, false, false, false, null);
    }
    private void roomIdToggleChanged(bool b)
    {
        if (b)
        {
            this.ui_RoomIdInputField.contentType = TMP_InputField.ContentType.Standard;

            this.ui_RoomIdToggle.transform.Find("Background").GetComponent<Image>().enabled = false;
        }
        else
        {
            this.ui_RoomIdInputField.contentType = TMP_InputField.ContentType.Password;

            this.ui_RoomIdToggle.transform.Find("Background").GetComponent<Image>().enabled = true;
        }
        this.ui_RoomIdInputField.ForceLabelUpdate();
    }

    private void uidToggleChanged(bool b)
    {
        if (b)
        {
            this.ui_UIDInputField.contentType = TMP_InputField.ContentType.Standard;

            this.ui_UIDToggle.transform.Find("Background").GetComponent<Image>().enabled = false;
        }
        else
        {
            this.ui_UIDInputField.contentType = TMP_InputField.ContentType.Password;

            this.ui_UIDToggle.transform.Find("Background").GetComponent<Image>().enabled = true;
        }
        this.ui_UIDInputField.ForceLabelUpdate();
    }

    private void onLoginBtnClicked()
    {
        uid = ui_UIDInputField.text.Trim();
        roomId = ui_RoomIdInputField.text.Trim();


        string codeOrToken = string.Empty;
        if (ConfigUtility.Data.GameChannelId == (int)GameChannelId.DouYin)
        {
            codeOrToken = dyStartToken;
        }
        else if (ConfigUtility.Data.GameChannelId == (int)GameChannelId.KuaiShou)
        {
            codeOrToken = ksCODE;
        }


        if (string.IsNullOrEmpty(codeOrToken))
        {
            if (string.IsNullOrEmpty(uid))//|| string.IsNullOrEmpty(roomId)
            {

                WindowUtil.ShowTip(1000);// 请检查输入项是否完备！
                return;
            }
        }
        else
        {
            uid = "";
            roomId = "";
        }

        string version = Application.version;
        long timeStamp = TimeTool.GetNowStamp();
        string deviceId = SDKTools.GetDeviceId();
        string loginType = ConfigUtility.Data.LoginType;



        var loginURL = ConfigUtility.GetLoginURL();
        Debug.Log("login--> loginType:" + loginType + ", version:" + version + ",URL:" + loginURL);
        //ServiceFetch.loginService.DoLogin(loginURL, uid, codeOrToken, version, timeStamp);
    }

    public override void HandleMessage(int msgId, params object[] paras)
    {
        base.HandleMessage(msgId, paras);
        if (msgId == WindowMsgID.OnLoginS2CSuccess)
        {
            this.enterPreloadProcedure();
        }
    }
    public override void RemoveEventListener()
    {
        base.RemoveEventListener();
        this.ui_LoginBtn.onClick.RemoveAllListeners();
        
        ui_UIDToggle.onValueChanged.RemoveAllListeners();
        ui_RoomIdToggle.onValueChanged.RemoveAllListeners();


        this.ui_SetBtn.onClick.RemoveAllListeners();

        EventDispatcher.Instance.RemoveListener(EventID.OnKSCodeReceived, this.onKSCodeReceived);
    }
}
