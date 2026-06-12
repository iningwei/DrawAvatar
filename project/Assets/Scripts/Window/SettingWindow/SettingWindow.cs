 using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Event;
using ZGame.Window;

public class SettingWindow : Window
{
    public Button ui_CloseBtn;
    public Transform ui_GiftAreaContent;
    public GameObject ui_SetAspectArea;
    SetAspectArea setAspectArea;
    public GameObject ui_GiftToggle;
 
    List<Toggle> giftToggles = new List<Toggle>();

    public Toggle ui_DaddyJoinEffectToggle;
    public Toggle ui_DamageTxtToggle; 

    public Toggle ui_MouseCursorToggle;

    public Slider ui_VoiceSlider;//音效
    public Slider ui_BGMSlider;//音乐


    public override void Init(params object[] paras)
    {
        base.Init(paras);
        this.setAspectArea = new SetAspectArea(this.ui_SetAspectArea, this, true);
        this.setAspectArea.Show();
    }

    public override void Show(params object[] paras)
    {
        base.Show(paras);

        //入场特效
        this.setDaddyJoin();
        this.setDamageTxt();

        this.setBGM();//音乐
        this.setVoice();//音效

        // 鼠标指针 
        this.setMouseCursor();




        //礼物
        this.setGiftToggles(); 
    }


     

    private void setDaddyJoin()
    {
        var flag = GameGlobal.Instance.disableEnterEffect;
        if (flag == true)
        {
            //不勾选
            this.ui_DaddyJoinEffectToggle.isOn = false;
        }
        else
        {
            this.ui_DaddyJoinEffectToggle.isOn = true;
        }
    }
    private void setDamageTxt()
    {
        this.ui_DamageTxtToggle.isOn = !GameGlobal.Instance.disableDamageTxt;
    }

    void setVoice()
    {
        var value = Storage.GetAudioValue();
        this.ui_VoiceSlider.value = value;
    }
    void setBGM()
    {
        var value = Storage.GetMusicValue();
        this.ui_BGMSlider.value = value;
    }
    private void setMouseCursor()
    {
        return;
        var flag = GameGlobal.Instance.enableMouseCursor;
        if (flag)
        {
            this.ui_MouseCursorToggle.isOn = true;
        }
        else
        {
            this.ui_MouseCursorToggle.isOn = false;
        }
        GameGlobal.Instance.SetMouseCursor();
    }

    private void setGiftToggles()
    {
        this.ui_GiftToggle.SetActive(false);

        var giftDatas = BeanManager.Instance.GetAllEnabledGiftSummonDatas();
        List<string> disableEffectGiftIds = GameGlobal.Instance.disableEffectGiftIds;
        for (int i = 0; i < giftDatas.Count; i++)
        {
            var giftToggleObj = GameObject.Instantiate(this.ui_GiftToggle);
            giftToggleObj.name = giftDatas[i].giftId.ToString();
            giftToggleObj.SetActive(true);
            giftToggleObj.transform.SetParent(this.ui_GiftAreaContent);
            giftToggleObj.transform.localScale = Vector3.one;

            giftToggleObj.FindChild("Label").GetComponent<TextMeshProUGUI>().text = giftDatas[i].name;
            Toggle giftToggle = giftToggleObj.GetComponent<Toggle>();
            if (disableEffectGiftIds.Contains(giftDatas[i].giftId))
            {
                giftToggle.isOn = false;
            }
            else
            {
                giftToggle.isOn = true;
            }
            giftToggle.onValueChanged.AddListener((v) =>
            {
                Debug.Log("giftToggle:" + giftToggle.name + ", value:" + v.ToString());
                this.setDisableEffectGiftIds();
            });
            this.giftToggles.Add(giftToggle);
        }
    }

    private void setDisableEffectGiftIds()
    {
        List<string> disabledIds = new List<string>();
        for (int i = 0; i < this.giftToggles.Count; i++)
        {
            if (this.giftToggles[i].isOn == false)
            {
                disabledIds.Add(this.giftToggles[i].name);
            }
        }
        GameGlobal.Instance.disableEffectGiftIds = disabledIds;
    }




    int pushConnectionTag = 0;//总连接状态
    int pushTypeCommentTag = 0;//评论
    int pushTypeLikeTag = 0;//点赞
    int pushTypeGiftTag = 0;//礼物

    public SettingWindow(GameObject obj, string windowName, string windowLayer, bool isExclusive, bool neverClose, params object[] paras) : base(obj, windowName, windowLayer, isExclusive, neverClose, paras)
    {
    }

     
    public override void AddEventListener()
    {
        base.AddEventListener();
        this.ui_CloseBtn.onClick.AddListener(this.onCloseBtnClicked);

        this.ui_DaddyJoinEffectToggle.onValueChanged.AddListener(this.onDaddyJoinEffectToggleValueChanged);


        this.ui_DamageTxtToggle.onValueChanged.AddListener(this.onDamageTxtToggleValueChanged);
        //////this.ui_MouseCursorToggle.onValueChanged.AddListener(this.onMouseCursorToggleValueChanged); 
        this.ui_VoiceSlider.onValueChanged.AddListener(this.onVoiceSliderValueChanged);
        this.ui_BGMSlider.onValueChanged.AddListener(this.onBGMSliderValueChanged);


    }




    private void onDaddyJoinEffectToggleValueChanged(bool arg0)
    {
        GameGlobal.Instance.disableEnterEffect = !arg0;
    }



    private void onDamageTxtToggleValueChanged(bool arg0)
    {
        GameGlobal.Instance.disableDamageTxt = !arg0;
    }

    //private void onMouseCursorToggleValueChanged(bool arg0)
    //{
    //    GameGlobal.Instance.enableMouseCursor = arg0;
    //    setMouseCursor();
    //}


    private void onBGMSliderValueChanged(float arg0)
    {
        Storage.SetMusicValue(arg0);
    }

    private void onVoiceSliderValueChanged(float arg0)
    {
        Storage.SetAudioValue(arg0);
    }



    private void onCloseBtnClicked()
    {
        this.Close();
    }

    public override void RemoveEventListener()
    {
        base.RemoveEventListener();
        this.ui_CloseBtn.onClick.RemoveAllListeners();
        for (int i = 0; i < this.giftToggles.Count; i++)
        {
            this.giftToggles[i].onValueChanged.RemoveAllListeners();
        }


        this.ui_VoiceSlider.onValueChanged.RemoveAllListeners();
        this.ui_BGMSlider.onValueChanged.RemoveAllListeners();



        this.ui_DaddyJoinEffectToggle.onValueChanged.RemoveAllListeners();
        this.ui_MouseCursorToggle.onValueChanged.RemoveAllListeners();

    }


}
