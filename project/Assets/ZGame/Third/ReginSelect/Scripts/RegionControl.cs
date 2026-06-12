using BestHTTP.JSON.LitJson;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Window;
using Color = UnityEngine.Color;

/// <summary>
/// 选择的地区
/// </summary>
[System.Serializable]
public class SelectRegion
{
    public static RegionType type; //选择的阶段：省 市 区
    public static string province;
    public static string city;
    public static string area;
}

public class RegionControl : MonoBehaviour
{
    public Button provinceBtn;
    public Button cityBtn;
    public Button areaBtn;
    public Button closeBtn;

    //省名:实际选择市:显示名
    List<string> hotCityNames = new List<string> { "B:北京市@B北京城区@北京市", "S:上海市@S:上海城区@上海市", "G:广东@G:广州@广州市", "G:广东@S:深圳@深圳市", "C:重庆市@C:重庆城区@重庆市", "T:天津市@T:天津城区@天津市", "S:四川省@C:成都市@成都市", "Z:浙江@H:杭州@杭州市", "H:湖北@W:武汉@武汉市", "J:江苏@N:南京@南京市", "J:江苏@S:苏州@苏州市", "H:湖南@C:长沙@长沙市" };
    List<Button> hotBtns = new List<Button>();

    public Transform ui_TitleArea;
    public Transform ui_HotCityArea;
    public Transform ui_SelectMsgArea;
    public Transform ui_SelectScrollView;
    public Transform ui_ScrollContentNode;

    void Start()
    {
        this.fillHotCity();

        provinceBtn.onClick.AddListener(OnClickProvince);
        cityBtn.onClick.AddListener(OnSelectProvinceItem);

        closeBtn.onClick.AddListener(OnClickCancle);
        this.OpenSelectRegion();
    }




    void fillHotCity()
    {
        for (int i = 0; i < 12; i++)
        {
            var btn = this.ui_HotCityArea.transform.Find("HotCity" + (i + 1) + "Btn").GetComponent<Button>();
            hotBtns.Add(btn);

            btn.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>().text = hotCityNames[i].Split('@')[2];

            btn.onClick.RemoveAllListeners();
            int index = i;
            btn.onClick.AddListener(() => { this.onHotBtnClicked(index); });
        }
    }

    void onHotBtnClicked(int index)
    {
        Debug.Log("on hot btn " + index + " clicked");

        var cityNames = hotCityNames[index].Split('@');
        var province = cityNames[0];
        var city = cityNames[1];

        SelectRegion.province = province;
        this.OnSelectProvinceItem();

        SelectRegion.city = city;
        this.OnSelectCityItem();
    }




    private Region read_region;//存放 解析类
    #region 读取本地数据
    /// <summary>
    /// 读取本地数据
    /// </summary>
    /// <param name="filename">Resources文件路径</param>
    public void ReadLocalDate(string filename)
    {
        read_region = new Region();
        TextAsset region_data = Resources.Load(filename) as TextAsset;
        if (!region_data)
        {
            Debug.LogError("找不到本地数据文件！");
        }
        else
        {
            string text_data = region_data.text;
            read_region = JsonMapper.ToObject<Region>(text_data);
        }
    }

    #endregion

    public GameObject ItemTemp;
    private List<GameObject> btn_list;//按钮存放列表

    public void CeratItemNodes(int count)
    {
        ui_ScrollContentNode.DestroyAllChilds(true);

        ItemTemp.SetActive(false);
        btn_list = new List<GameObject>();
        GameObject go;
        for (int i = 0; i < count; i++)
        {
            go = Instantiate(ItemTemp);
            go.transform.SetParent(ui_ScrollContentNode);
            go.transform.localScale = Vector3.one;
            go.transform.rotation = Quaternion.identity;
            btn_list.Add(go);

            go.SetActive(true);
        }

    }



    public void OpenSelectRegion()
    {

        SelectRegion.province = "";
        SelectRegion.city = "";
        SelectRegion.area = "";
        ReadLocalDate("address_china");

        this.OnClickProvince();
    }

    public void OnClickProvince()
    {
        SelectRegion.type = RegionType.Province_Type;
        SelectRegion.city = "";
        SelectRegion.area = "";

        this.provinceBtn.gameObject.SetActive(false);
        this.cityBtn.gameObject.SetActive(false);
        this.areaBtn.gameObject.SetActive(false);
        this.ui_SelectMsgArea.gameObject.SetActive(false);

        this.ui_HotCityArea.gameObject.SetActive(true);
        this.ui_TitleArea.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 1270, 0);
        ShowProvince(read_region);
    }


    public void OnSelectProvinceItem()
    {
        this.ui_HotCityArea.gameObject.SetActive(false);
        this.ui_SelectMsgArea.gameObject.SetActive(true);
        this.ui_TitleArea.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 920, 0);

        this.provinceBtn.gameObject.SetActive(true);
        this.cityBtn.gameObject.SetActive(true);
        this.areaBtn.gameObject.SetActive(false);

        this.provinceBtn.enabled = true;
        this.cityBtn.enabled = false;

        var provinceDes = this.provinceBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        provinceDes.color = ColorTool.HexToRGBA("F9FBFDFF");
        provinceDes.text = SelectRegion.province.Split(":")[1];

        var cityDes = this.cityBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        cityDes.color = Color.green;
        cityDes.text = "请选择";


        SelectRegion.type = RegionType.City_Type;
        for (int i = 0; i < read_region.address.Count; i++)
        {
            if (read_region.address[i].name == SelectRegion.province)
            {
                ShowCity(read_region.address[i]);
            }
        }

    }
    public void OnSelectCityItem()
    {
        this.provinceBtn.gameObject.SetActive(true);
        this.cityBtn.gameObject.SetActive(true);
        this.areaBtn.gameObject.SetActive(true);

        this.provinceBtn.enabled = true;
        this.cityBtn.enabled = true;
        this.areaBtn.enabled = false;

        var provinceDes = this.provinceBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        provinceDes.color = ColorTool.HexToRGBA("F9FBFDFF");
        provinceDes.text = SelectRegion.province.Split(":")[1];

        var cityDes = this.cityBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        cityDes.text = SelectRegion.city.Split(":")[1];
        cityDes.color = ColorTool.HexToRGBA("F9FBFDFF");

        var areaDes = this.areaBtn.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        areaDes.color = Color.green;
        areaDes.text = "请选择";


        SelectRegion.type = RegionType.Area_Type;
        for (int i = 0; i < read_region.address.Count; i++)
        {
            if (read_region.address[i].name == SelectRegion.province)
            {
                for (int j = 0; j < read_region.address[i].city.Count; j++)
                {
                    if (read_region.address[i].city[j].name == SelectRegion.city)
                    {
                        ShowArea(read_region.address[i].city[j]);
                    }
                }
            }
        }
    }

    public void OnSelectAreaItem()
    {
        //TODO:显示街道/乡/镇级别行政单位
    }

    public void SelectFinished()
    {

    }

    private void OnClickCancle()
    {
    }


    public void ShowProvince(Region region)
    {
        CeratItemNodes(region.address.Count);

        //排序
        region.address.Sort((a, b) => a.name.CompareTo(b.name));
        string lastPinYin = "";
        for (int i = 0; i < region.address.Count; i++)
        {
            btn_list[i].GetComponent<RegionItem>().Init(this, region.address[i].name, lastPinYin);
            lastPinYin = region.address[i].name.Split(':')[0];
        }
    }

    public void ShowCity(Province province)
    {
        CeratItemNodes(province.city.Count);

        //排序
        province.city.Sort((a, b) => a.name.CompareTo(b.name));
        string lastPinYin = "";
        for (int i = 0; i < province.city.Count; i++)
        {
            btn_list[i].GetComponent<RegionItem>().Init(this, province.city[i].name, lastPinYin);
            lastPinYin = province.city[i].name.Split(':')[0];
        }
    }

    public void ShowArea(City city)
    {
        CeratItemNodes(city.area.Count);
        //排序
        city.area.Sort();
        string lastPinYin = "";
        for (int i = 0; i < city.area.Count; i++)
        {
            btn_list[i].GetComponent<RegionItem>().Init(this, city.area[i], lastPinYin);
            lastPinYin = city.area[i].Split(":")[0];
        }

    }


    private void OnDestroy()
    {

        hotBtns = null;
    }
}