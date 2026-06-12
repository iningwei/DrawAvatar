using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using ZGame;
using ZGame.DanMu;
using ZGame.ZTable;

public class BeanManager : Singleton<BeanManager>
{
    public string GetLanguage(long id, params object[] paras)
    {
        var data = LanguageDataReader.Instance.GetEntity((int)id);

        string result = "";
        if (data != null)
        {
            string appLanguage = Storage.GetAppLanguage();
            if (appLanguage == "CN")//中文简体
            {
                result = data.CN;
            }
            else if (appLanguage == "EN")
            {
                result = data.EN;
            }
            else if (appLanguage == "JA")//日语
            {
                result = data.JA;
            }
            else if (appLanguage == "KO")//韩国
            {
                result = data.KO;
            }
            else if (appLanguage == "VI")//越南
            {
                result = data.VI;
            }
            else if (appLanguage == "TL")//菲律宾
            {
                result = data.TL;
            }
            else if (appLanguage == "TUR")//土耳其语
            {
                result = data.TUR;
            }
            else if (appLanguage == "RI")//印度尼西亚语
            {
                result = data.RI;
            }
            else
            {
                Debug.LogError("not implement :" + appLanguage);
            }
        }

        return Multilingual.FormatStr(id, result, paras);
    }
#if NET_DATA_PB
    public List<ProtobufMsgID> GetSocketIgnoreLogMsgIds()
    {
        var list = new List<ProtobufMsgID>();
        string str = BeanManager.Instance.GetConfigStr(10000);
        if (string.IsNullOrEmpty(str) == false)
        {
            var strs = str.Split(';');
            for (int i = 0; i < strs.Length; i++)
            {
                try
                {
                    var msgId = (ProtobufMsgID)Enum.Parse(typeof(ProtobufMsgID), (string)strs[i]);
                    list.Add(msgId);
                }
                catch (Exception ex)
                {
                    Debug.LogError("add SocketIgnoreLogMsgId fail，type:" + strs[i] + ",  detail msg:" + ex.ToString());
                }

            }
        }

        return list;
    }
#endif

    public ConfigData GetConfigDataById(int id)
    {
        if (id < 1)
        {
            return null;
        }

        return ConfigDataReader.Instance.GetEntity(id);
    }
    public float GetConfigFloat(int configId)
    {
        return float.Parse(BeanManager.Instance.GetConfigDataById(configId).Value);
    }
    public int GetConfigInt(int configId)
    {
        return int.Parse(BeanManager.Instance.GetConfigDataById(configId).Value);
    }
    public string GetConfigStr(int configId)
    {
        return BeanManager.Instance.GetConfigDataById(configId).Value;
    }
    public bool GetConfigBool(int configId)
    {
        return bool.Parse(BeanManager.Instance.GetConfigDataById(configId).Value);
    }
    public int[] GetConfigIntArray(int configId)
    {
        return BeanManager.Instance.GetConfigDataById(configId).Value.Split(',').ToIntArray();
    }
    public int[][] GetConfigIntArrayTwo(int configId)
    {

        var r1 = BeanManager.Instance.GetConfigDataById(configId).Value.Split(',');
        int[][] r2 = new int[r1.Length][];
        for (int i = 0; i < r1.Length; i++)
        {
            r2[i] = r1[i].Split(";").ToIntArray();
        }
        return r2;
    }
    public float[] GetConfigFloatArray(int configId)
    {
        return BeanManager.Instance.GetConfigDataById(configId).Value.Split(',').ToFloatArray();
    }


    public List<SummonData> GetAllEnabledGiftSummonDatas()
    {
        List<SummonData> datas = new List<SummonData>();
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.commandType == DanMuCommandType.Gift)//礼物
            {
                if (item.Value.enableSetting == true)//启用
                {
                    datas.Add(item.Value);
                }
            }
        }
        return datas;
    }

    public string GetJoinCMDStr()
    {
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.commandType == DanMuCommandType.Join)//加入指令
            {
                return item.Value.commandStrs[0];
            }
        }
        Debug.LogError("can not get join cmd ");
        return "";
    }

    public string GetEquipHeroCMDStr()
    {
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.commandType == DanMuCommandType.EquipHero)//上阵英雄
            {
                return item.Value.commandStrs[0];

            }
        }
        Debug.LogError("can not get equipHero cmd  ");
        return "";
    }


    public AudioData GetAudioData(long id)
    {
        if (id < 1)
        {
            return null;
        }
        var data = AudioDataReader.Instance.GetEntity(id);
        if (data == null)
        {
            Debug.LogError("error ,no audioData with id:" + id);
        }
        return data;
    }

    public VideoData GetVideoData(long id)
    {
        var data = VideoDataReader.Instance.GetEntity(id);
        if (data == null)
        {
            Debug.LogError("error,no VideoData with id:" + id);
        }
        return data;
    }

    public ItemData GetItemData(long id)
    {
        var data = ItemDataReader.Instance.GetEntity(id);
        if (data == null)
        {
            Debug.LogError("error, no ItemData with id:" + id);
        }
        return data;
    }






    public RobotData GetRobotData(string id)
    {
        RobotData data = RobotDataReader.Instance.GetEntity(id);

        if (data == null)
        {
            Debug.LogError("error ,no RobotData with id:" + id);
        }
        return data;
    }
    public RobotData GetRobotDataByUID(string uid)
    {
        if (uid.StartsWith(PlayerAvatarManager.Instance.robotUidPrefix) == false)
        {
            Debug.LogError("error, robot uid error:" + uid + ", should start with:" + PlayerAvatarManager.Instance.robotUidPrefix);
            return null;
        }
        long id = long.Parse(uid.Remove(0, PlayerAvatarManager.Instance.robotUidPrefix.Length));
        return GetRobotData(uid);
    }
    public SummonData GetSummonData(int id)
    {
        SummonData data = SummonDataReader.Instance.GetEntity(id);

        if (data == null)
        {
            Debug.LogError("error ,no SummonData with id:" + id);
        }
        return data;
    }
    public SummonData[] GetSummonDataByCommandType(DanMuCommandType commandType)
    {
        List<SummonData> datas = new List<SummonData>();
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.commandType == commandType)
            {
                datas.Add(item.Value);
            }
        }
        if (datas.Count > 0)
        {
            return datas.ToArray();
        }

        Debug.LogError("error ,no SummonData with commandType:" + commandType);
        return null;
    }

    public SummonData GetJoinSummonDataByCmd(string cmd)
    {
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            var cmds = item.Value.commandStrs;
            if (cmds == null)
            {
                continue;
            }
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmds[i] == cmd)
                {
                    return item.Value;
                }
            }
        }

        Debug.LogError("can not get joinSummonData with cmd:" + cmd);
        return null;
    }

    public SummonData GetSummonDataByGiftId(string id)
    {
        var map = SummonDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.giftId == id)
            {
                return item.Value;
            }
        }
        Debug.LogError("error ,no SummonData with giftId:" + id);
        return null;
    }




    List<string> joinCommands;
    public List<string> DanMuJoinCommands
    {
        get
        {
            if (joinCommands == null)
            {
                joinCommands = new List<string>();
                var summonDatas = BeanManager.Instance.GetSummonDataByCommandType(DanMuCommandType.Join);
                if (summonDatas != null)
                {
                    for (int i = 0; i < summonDatas.Length; i++)
                    {
                        var keys = summonDatas[i].commandStrs;
                        if (keys != null)
                        {
                            joinCommands.AddRange(keys);
                        }
                    }
                }
            }

            return joinCommands;
        }
    }
    List<string> queryCommands;
    public List<string> DanMuQueryCommands
    {
        get
        {
            if (queryCommands == null)
            {
                queryCommands = new List<string>();
                var summonDatas = BeanManager.Instance.GetSummonDataByCommandType(DanMuCommandType.Query);
                if (summonDatas != null)
                {
                    for (int i = 0; i < summonDatas.Length; i++)
                    {
                        var keys = summonDatas[i].commandStrs;
                        if (keys != null)
                        {
                            queryCommands.AddRange(keys);
                        }

                    }
                }
            }

            return queryCommands;
        }
    }
    List<string> labaCommands;
    public List<string> DanMuLaBaCommands
    {
        get
        {
            if (labaCommands == null)
            {
                labaCommands = new List<string>();
                var summonDatas = BeanManager.Instance.GetSummonDataByCommandType(DanMuCommandType.LaBa);
                if (summonDatas != null)
                {
                    for (int i = 0; i < summonDatas.Length; i++)
                    {
                        var keys = summonDatas[i].commandStrs;
                        if (keys != null)
                        {
                            labaCommands.AddRange(keys);
                        }

                    }
                }
            }

            return labaCommands;
        }
    }


    List<string> equipHeroCommands;
    public List<string> DanMuEquipHeroCommands
    {
        get
        {
            if (equipHeroCommands == null)
            {
                equipHeroCommands = new List<string>();
                var summonDatas = BeanManager.Instance.GetSummonDataByCommandType(DanMuCommandType.EquipHero);
                if (summonDatas != null)
                {
                    for (int i = 0; i < summonDatas.Length; i++)
                    {
                        var keys = summonDatas[i].commandStrs;
                        if (keys != null)
                        {
                            equipHeroCommands.AddRange(keys);
                        }

                    }
                }
            }

            return equipHeroCommands;
        }
    }

    public SummonData GetSummonDataByKeyShortcut(int keyASCII)
    {
        SummonData data = null;

        var datas = SummonDataReader.Instance.GetEntityMap().Values.ToArray();
        for (int i = 0; i < datas.Length; i++)
        {
            string[] keyStrs = datas[i].keyShortcut.Split(';');
            for (int k = 0; k < keyStrs.Length; k++)
            {
                if (keyStrs[k] == keyASCII.ToString())
                {
                    data = datas[i];
                    return data;
                }
            }
        }
        if (data == null)
        {
            Debug.LogError("error, no summonData with keyShortcut:" + keyASCII + "," + ((KeyCode)keyASCII).ToString());
        }
        return data;
    }
    public GiftData GetGiftData(string id)
    {
        return GiftDataReader.Instance.GetEntity(id);
    }
    public GiftData GetGiftDataBySecId(string sec_gift_id)
    {
        var map = GiftDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.sec_gift_id == sec_gift_id)
            {
                return item.Value;
            }
        }
        Debug.LogError("error ,no GiftData with sec_gift_id:" + sec_gift_id);
        return null;
    }

    public ClientRankData GetClientRankData(int rank)
    {
        var map = ClientRankDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (item.Value.rankRange[0] <= rank && item.Value.rankRange[1] >= rank)
            {
                return item.Value;
            }
        }
        DebugExt.LogE("error, no ClientRankData with rank:" + rank);
        return null;
    }


    public ZhuBoData GetZhuBoData(long id)
    {
        var data = ZhuBoDataReader.Instance.GetEntity(id);
        if (data == null)
        {
            Debug.LogError("error,no ZhuBoData with id:" + id);
        }
        return data;
    }
    public EffectData GetEffectData(long id)
    {
        var data = EffectDataReader.Instance.GetEntity(id);
        if (data == null)
        {
            Debug.LogError("error,no EffectData with id:" + id);
        }
        return data;
    }


    public RankEnterData GetRankEnterData(long rank)
    {
        var map = RankEnterDataReader.Instance.GetEntityMap();
        foreach (var item in map)
        {
            if (rank >= item.Value.rankRange[0] && rank <= item.Value.rankRange[1])
            {
                return item.Value;
            }
        }
        Debug.LogError("error, no RankEnterData with rank:" + rank);
        return null;
    }

}
