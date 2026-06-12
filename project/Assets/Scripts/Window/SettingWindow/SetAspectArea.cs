using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using ZGame.Window;

public class SetAspectArea : Area
{
    public TMP_Dropdown ui_Dropdown;
    public SetAspectArea(GameObject obj, Window window, bool initVisible, params object[] paras) : base(obj, window, initVisible, paras)
    {
    }

    public override void Show(params object[] paras)
    {
        base.Show(paras);

        ui_Dropdown.options.Clear();
        ui_Dropdown.AddOptions(new System.Collections.Generic.List<TMP_Dropdown.OptionData>()
        {
            new TMP_Dropdown.OptionData("540x960"),
            new TMP_Dropdown.OptionData("864x1536"),
            new TMP_Dropdown.OptionData("1080x1920"),
            new TMP_Dropdown.OptionData("1440x2560"),
            new TMP_Dropdown.OptionData("2160x3840")
        });

        int index = PlayerPrefs.GetInt("ScreenAspectIndex", 0);
        ui_Dropdown.value = index;
    }

    private void onDropdownValueChanged(int index)
    {
        string[] strs = ui_Dropdown.options[index].text.Split("x");
        Screen.SetResolution(int.Parse(strs[0]), int.Parse(strs[1]), false);
        Debug.Log("set Resolution \twidth:" + strs[0] + "\theight:" + strs[1]);
        PlayerPrefs.SetInt("ScreenAspectIndex", index);
        PlayerPrefs.Save();
    }

    public override void AddEventListener()
    {
        base.AddEventListener();
        ui_Dropdown.onValueChanged.AddListener(this.onDropdownValueChanged);
    }

    public override void RemoveEventListener()
    {
        base.RemoveEventListener();
        this.ui_Dropdown.onValueChanged.RemoveAllListeners();
    }
}
