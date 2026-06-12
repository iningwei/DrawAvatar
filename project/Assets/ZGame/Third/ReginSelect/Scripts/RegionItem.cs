/*
 * 功能：地区按钮模板  显示地区信息
 */
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class RegionItem : MonoBehaviour
{

    TextMeshProUGUI nameText;
    TextMeshProUGUI pingYinText;

    string content_info;

    RegionControl control;

    Button btn;
    public void Init(RegionControl control, string textInfo, string lastPinYin)
    {
        nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        pingYinText = transform.Find("PingYin").GetComponent<TextMeshProUGUI>();
        pingYinText.gameObject.SetActive(false);

        string curPingYin = textInfo.Split(':')[0];
        if (curPingYin != lastPinYin)
        {
            pingYinText.gameObject.SetActive(true);
            pingYinText.text = curPingYin;
        }
        this.control = control;
        content_info = textInfo;
        nameText.text = textInfo.Split(':')[1];


        btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(OnClickBtn);
    }

    public void OnClickBtn()
    {
        switch (SelectRegion.type)
        {
            case RegionType.Province_Type:
                SelectRegion.province = content_info;
                control.OnSelectProvinceItem();
                break;
            case RegionType.City_Type:
                SelectRegion.city = content_info;
                control.OnSelectCityItem();
                break;
            case RegionType.Area_Type:
                SelectRegion.area = content_info;
                //control.OnSelectAreaItem();
                control.SelectFinished();
                break;
        }
    }


}