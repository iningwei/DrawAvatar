 using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using ZGame;
using ZGame.Event;
using ZGame.TimerTween;

public class GameGlobal : Singleton<GameGlobal>
{
    public string ksCode = "";

    //black intensity0
    public float4 emissionOriginColor = new float4(0f, 0f, 0f, 1f);
    //bf0600 intensity4
    public float4 emissionRedColor = new float4(2.99f, 0.094f, 0f, 1f);
    //0050bf intensity4
    public float4 emissionBlueColor = new float4(0f, 0.32f, 2.08f, 1f);
 
     

    private bool _enableMouseCursor = true;
    public bool enableMouseCursor
    {
        get
        {
            string value = PlayerPrefs.GetString("ENABLE_MOUSECURSOR", _enableMouseCursor.ToString());
            _enableMouseCursor = bool.Parse(value);
            return _enableMouseCursor;
        }
        set
        {
            _enableMouseCursor = value;
            PlayerPrefs.SetString("ENABLE_MOUSECURSOR", _enableMouseCursor.ToString());
            PlayerPrefs.Save();
        }
    }

    private List<string> _disableEffectGiftIds;
    /// <summary>
    /// 禁用特效的礼物id列表(summonData的giftId字段)
    /// </summary>
    public List<string> disableEffectGiftIds
    {
        get
        {
            if (_disableEffectGiftIds == null)
            {
                string _str = PlayerPrefs.GetString("DISABLE_EFFECT_GIFT_IDS", "");
                Debug.Log("get disableEffectGiftIds：" + _str);
                _disableEffectGiftIds = _str.Split("@").ToList();
            }
            return _disableEffectGiftIds;
        }
        set
        {
            _disableEffectGiftIds = value;
            string _str = "";
            for (int i = 0; i < _disableEffectGiftIds.Count; i++)
            {
                if (i == 0)
                    _str += _disableEffectGiftIds[i].ToString();
                else
                    _str += "@" + _disableEffectGiftIds[i].ToString();
            }
            PlayerPrefs.SetString("DISABLE_EFFECT_GIFT_IDS", _str);
            Debug.Log("set disableEffectGiftIds：" + _str);
            PlayerPrefs.Save();
        }
    }

    private bool _disableEnterEffect = true;
    /// <summary>
    /// 是否禁用入场特效，默认 true，即 禁用
    /// </summary>
    public bool disableEnterEffect
    {
        get
        {
            string value = PlayerPrefs.GetString("DISABLE_ENTER_EFFECT", _disableEnterEffect.ToString());
            Debug.Log("get _disableEnterEffect：" + value);
            _disableEnterEffect = bool.Parse(value);
            return _disableEnterEffect;
        }
        set
        {
            _disableEnterEffect = value;
            PlayerPrefs.SetString("DISABLE_ENTER_EFFECT", _disableEnterEffect.ToString());
            Debug.Log("set _disableEnterEffect：" + _disableEnterEffect.ToString());
            PlayerPrefs.Save();
        }
    }

    private bool _disableDamageTxt = false;
    /// <summary>
    /// 是否禁用伤害数字显示，默认 false，即 不禁用
    /// </summary>
    public bool disableDamageTxt
    {
        get
        {
            string value = PlayerPrefs.GetString("DISABLE_DAMAGE_TXT", _disableDamageTxt.ToString());
            _disableDamageTxt = bool.Parse(value);
            return _disableDamageTxt;
        }
        set
        {
            _disableDamageTxt = value;
            PlayerPrefs.SetString("DISABLE_DAMAGE_TXT", _disableDamageTxt.ToString());
            Debug.Log("set _disableDamageTxt：" + _disableDamageTxt.ToString());
            PlayerPrefs.Save();
        }
    }

    // 单位克制表: key = (attackerUnitId << 32) | defenderUnitId, value = 附加倍率
    private Dictionary<long, float> m_UnitCounterMap;

    //特效降频
    public float fxCullNearDistSq = 14400;//近距离120米
    public float fxCullMidDistSq = 25600;//中距离 120米至 160米
    public float fxCullFarDistSq = 52900;//远距离160米至230米
    public float fxNearKeepRatio = 1f;//近距离特效保留比例
    public float fxMidKeepRatio = 0.6f;//中距离特效保留比例
    public float fxFarKeepRatio = 0.3f;//远距离特效保留比例//超过远距离的则不保留特效

    
      

    public GameGlobal()
    {
         

    }
    public void SetAppVersionDes(TextMeshProUGUI targetDesTxt)
    {
        var localResVersion = PlayerPrefs.GetString("resversion_" + ConfigUtility.Data.AppVersion, "-1");
        if (localResVersion == "-1")
        {
            localResVersion = ConfigUtility.Data.ResVersion;
        }
        targetDesTxt.text = "v" + ConfigUtility.Data.AppVersion + "_" + ConfigUtility.Data.ResVersion + "_" + localResVersion + "_" + ConfigUtility.Data.LoginType;
    }

    Texture2D cursorTex;
    public void SetMouseCursor()
    {
        if (GameGlobal.Instance.enableMouseCursor)
        {
            if (GameGlobal.Instance.cursorTex != null)
            {
                Cursor.SetCursor(GameGlobal.Instance.cursorTex, Vector2.zero, CursorMode.Auto);
                Debug.LogError("set mouse cursor tex 1");
            }
            else
            {
                var cursorTex = Resources.Load<Texture2D>("icon_finger_01");
                GameGlobal.Instance.cursorTex = cursorTex;
                Cursor.SetCursor(GameGlobal.Instance.cursorTex, Vector2.zero, CursorMode.Auto);
                Debug.LogError("set mouse cursor tex 2");
            }
        }
        else
        {
            // 恢复默认光标（通常是箭头）
            Debug.LogError("set mouse cursor null");
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        //Cursor.visible = true / false;//设置显示或者隐藏鼠标光标
    }


    public bool CheckContentSuitCmds(string content, List<string> cmds, out string realCmd)
    {
        for (int i = 0; i < cmds.Count; i++)
        {
            if (content.StartsWith(cmds[i]))
            {
                realCmd = cmds[i];
                return true;
            }
        }
        realCmd = "";
        return false;
    }

    


    //////public void HandleWithPetData(string uid, Action<PbUserBase> callback)
    //////{
    //////    var userBase = ServiceFetch.userService.GetPbUserBase(uid);
    //////    if (userBase == null)
    //////    {
    //////        ServiceFetch.userService.ReqUserBase(uid);
    //////        TimerTween.RepeatCount(1f, 10, () =>
    //////        {
    //////            var data = ServiceFetch.userService.GetPbUserBase(uid);
    //////            if (data != null)
    //////            {
    //////                callback?.Invoke(data);
    //////                callback = null;
    //////                return false;
    //////            }
    //////            return true;
    //////        }, true).Start();
    //////    }
    //////    else
    //////    {
    //////        callback?.Invoke(userBase);
    //////        callback = null;
    //////    }
    //////}
    ////////玩家头像
    //////Dictionary<string, int> headRequestDic = new Dictionary<string, int>();
    //////public void SetPlayerHead<T>(string playerId, T target) where T : UnityEngine.Object
    //////{
    //////    if (!string.IsNullOrEmpty(playerId))
    //////    {
    //////        var tex = PlayerAvatarManager.Instance.GetHeadImgByUserID(playerId);
    //////        if (tex != null)
    //////        {
    //////            if (target != null)
    //////            {
    //////                if (target is RawImage rImg)
    //////                {
    //////                    rImg.texture = tex;
    //////                }
    //////                else if (target is MeshRenderer mr)
    //////                {
    //////                    mr.sharedMaterial.SetTexture("_BaseMap", tex);
    //////                }
    //////                else
    //////                {
    //////                    Debug.LogError("error, not implement, TODO:::");
    //////                }
    //////            }
    //////        }
    //////        else
    //////        {
    //////            this.HandleWithPlayerData(playerId, (pbRoomPublic) =>
    //////            {
    //////                this.showHead(playerId, target);
    //////            }, false);
    //////        }
    //////    }
    //////}

    //////Dictionary<long, Action<PbRoomPublic>> handleWithPlayerDataCallbackDic = new Dictionary<long, Action<PbRoomPublic>>();
    //////public void HandleWithPlayerData(string anchorId, Action<PbRoomPublic> callback, bool forceRefresh)
    //////{
    //////    long id = IdAssginer.GetID(IdAssginer.IdType.HandleWithPlayerDataCallback);
    //////    this.handleWithPlayerDataCallbackDic.Add(id, callback);

    //////    var userBase = ServiceFetch.roomService.GetPbRoomPublic(anchorId);

    //////    if (userBase == null)
    //////    {
    //////        ServiceFetch.roomService.ReqRoomPublic(anchorId);
    //////        TimerTween.RepeatCount(1, 10, () =>
    //////        {
    //////            var data = ServiceFetch.roomService.GetPbRoomPublic(anchorId);
    //////            if (data != null)
    //////            {
    //////                if (this.handleWithPlayerDataCallbackDic.ContainsKey(id))
    //////                {
    //////                    this.handleWithPlayerDataCallbackDic[id].Invoke(data);
    //////                    this.handleWithPlayerDataCallbackDic.Remove(id);
    //////                }
    //////                return false;
    //////            }
    //////            return true;
    //////        }, true).Start();
    //////    }
    //////    else
    //////    {
    //////        if (this.handleWithPlayerDataCallbackDic.ContainsKey(id))
    //////        {
    //////            this.handleWithPlayerDataCallbackDic[id].Invoke(userBase);
    //////            this.handleWithPlayerDataCallbackDic.Remove(id);
    //////        }

    //////        if (forceRefresh)
    //////        {
    //////            ServiceFetch.roomService.ReqRoomPublic(anchorId);
    //////        }
    //////    }
    //////}
    //////public void SetPetRoleNameTxt(string uid, TextMeshProUGUI nameTxt)
    //////{
    //////    this.HandleWithPetData(uid, (data) =>
    //////    {
    //////        if (nameTxt != null)
    //////        {
    //////            nameTxt.text = data.NickName.SubWithPoints();
    //////        }
    //////    });
    //////}
    //////public void SetPetHead(string uid, RawImage image)
    //////{
    //////    if (!string.IsNullOrEmpty(uid))
    //////    {
    //////        var tex = PlayerAvatarManager.Instance.GetHeadImgByUserID(uid);
    //////        if (tex != null)
    //////        {
    //////            if (image != null)
    //////            {
    //////                image.texture = tex;
    //////            }
    //////        }
    //////        else
    //////        {
    //////            this.HandleWithPetData(uid, (pbUserBase) =>
    //////            {
    //////                this.showHead(uid, image);
    //////            });

    //////        }
    //////    }
    //////}
    //////public void showHead<T>(string uid, T target) where T : UnityEngine.Object
    //////{
    //////    if (!string.IsNullOrEmpty(uid))
    //////    {
    //////        var tex = PlayerAvatarManager.Instance.GetHeadImgByUserID(uid);
    //////        if (tex != null)
    //////        {
    //////            if (target != null)
    //////            {
    //////                if (target is RawImage rImg)
    //////                {
    //////                    rImg.texture = tex;
    //////                }
    //////                else if (target is MeshRenderer mr)
    //////                {
    //////                    mr.sharedMaterial.SetTexture("_BaseMap", tex);
    //////                }
    //////                else
    //////                {
    //////                    Debug.LogError("error, not implement, TODO::::");
    //////                }

    //////            }
    //////        }
    //////        else
    //////        {
    //////            TimerTween.Delay(1f, () =>
    //////            {
    //////                if (!this.headRequestDic.ContainsKey(uid))
    //////                {
    //////                    headRequestDic[uid] = 1;
    //////                }
    //////                else
    //////                {
    //////                    headRequestDic[uid] += 1;
    //////                }
    //////                if (this.headRequestDic[uid] < 5)
    //////                {
    //////                    showHead(uid, target);
    //////                }
    //////                else
    //////                {
    //////                    //Debug.LogError("player:" + playerId + " 's head image can not download,so quit!  ");
    //////                }
    //////            }).Start();
    //////        }
    //////    }
    //////}

 
}
