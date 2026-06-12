using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Window;
using Debug = UnityEngine.Debug;

public class WindowUtil
{
    public static void ShowNetMask(string tag)
    {
        if (string.IsNullOrEmpty(tag.Trim()))
        {
            Debug.LogError("error, showNetMask tag should not null");
        }
        //Debug.LogError("ShowNetMask..");
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnShowNetMask, tag);
    }


    public static void ShowNetMaskWithRoll(string tag)
    {
        if (string.IsNullOrEmpty(tag.Trim()))
        {
            Debug.LogError("error, ShowNetMaskWithRoll tag should not null");
        }
        //Debug.LogError("ShowNetMaskWithRoll..");
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnShowNetMaskWithRoll, tag);
    }

    public static void HideNetMask()
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.NetMaskWindow, WindowMsgID.OnHideNetMask);
    }
    public static void ShowTip(int languageDataId, params object[] param)
    {
        string text = BeanManager.Instance.GetLanguage(languageDataId, param);
        ShowTip(text);
    }

    //ignoreWhileSameAsBefore ：如果和之前的提示内容一致，则忽略该提示
    public static void ShowTip(string tipContent, bool ignoreWhileSameAsBefore = false, float duration = 1)
    {
        WindowManager.Instance.SendWindowMessage(WindowNames.TipWindow, WindowMsgID.OnAddTip, tipContent, duration, ignoreWhileSameAsBefore);
    }



    public static void ShowMessageBox(string content, Action confirmCallback, MessageBoxBtnsLayout layout, string tipTitle = "提示", float contentHeight = 480f, float bgAlpha = 210)
    {
        WindowManager.Instance.ShowWindow(WindowNames.MessageBoxWindow, WindowLayer.Hud2, false, false, true, null, content, confirmCallback, layout, tipTitle, contentHeight, bgAlpha);
    }


    public static void SetAppVersionDes(TextMeshProUGUI targetDesTxt)
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }
        targetDesTxt.text = "v:" + ConfigUtility.Data.AppVersion + "_" + ConfigUtility.Data.ResVersion + "_" + localResVersion;
    }
}
