 using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using ZGame;
using ZGame.Event;
using Random = UnityEngine.Random;

public class DaddyManager
{
    public Dictionary<string, Daddy> daddyDic = new Dictionary<string, Daddy>();

    public DaddyManager()
    {
        EventDispatcher.Instance.AddListener(EventID.OnDaddyJoin, this.onDaddyJoin);
        EventDispatcher.Instance.AddListener(EventID.OnKickDaddy, this.onKickDaddy); 
    }

    public void Update()
    {
        foreach (var item in daddyDic)
        {
            item.Value.Update();
        }
    }
    public void Dispose()
    {
        EventDispatcher.Instance.RemoveListener(EventID.OnDaddyJoin, this.onDaddyJoin);
        EventDispatcher.Instance.RemoveListener(EventID.OnKickDaddy, this.onKickDaddy);
    
    }
    

    void clear()
    {
        foreach (var item in daddyDic)
        {
            item.Value.Destroy();
        }
        daddyDic.Clear();
    }
    public Daddy GetDaddy(string uid)
    {
        daddyDic.TryGetValue(uid, out Daddy daddy);
        return daddy;
    }
    public int GetDaddyCountByCampType(long campType)
    {
        int count = 0;
        foreach (var item in daddyDic)
        {
            if (item.Value.campType == campType)
            {
                count++;
            }
        }
        return count;
    }


    public List<string> GetDaddyUidList()
    {
        List<string> uids = new List<string>();
        foreach (var item in this.daddyDic)
        {
            uids.Add(item.Key);
        }
        return uids;
    }




    private void onKickDaddy(string evtId, object[] paras)
    {
        string id = paras[0].ToString();
        if (daddyDic.ContainsKey(id))
        {
            daddyDic[id].Destroy();
            daddyDic.Remove(id);
            Debug.LogError($"remove daddy:{id}");
            EventDispatcher.Instance.DispatchEvent(EventID.OnScrollMsgTip, id, " 退出队伍");
        }
    }

    private void onDaddyJoin(string evtId, object[] paras)
    {
        string uid = paras[0].ToString();
        long campType = (long)paras[1];
        string nick = paras[2].ToString();
        string headUrl = paras[3].ToString();


        PlayerAvatarManager.Instance.AddPlayerAvatarData(uid, nick, headUrl);

        Daddy daddy = this.GetDaddy(uid);
        if (daddy == null)
        {
            daddy = new Daddy(uid, campType, nick, headUrl);
            daddyDic.Add(uid, daddy);
            Debug.Log($"daddy, uid:{uid},nick:{nick},campType:{campType} joined, total：" + daddyDic.Count);
        }
        else
        {
            Debug.LogError("already have daddy:" + uid);
        }

        //更新pet 
        //////daddy.AddPet(petEntityId, petBeanId);
    }

    public bool Exist(string uid)
    {
        return daddyDic.ContainsKey(uid);
    }



    public void CleanAllDaddyDamage()
    {
        foreach (var item in daddyDic)
        {
            item.Value.CleanDamageValue();
        }
    }
}
